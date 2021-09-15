using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using XNode;

[ExecuteAlways]
public class World : SerializedMonoBehaviour
{
	[SerializeField]
	private bool debugNormals;
	[ShowInInspector, ReadOnly, SuffixLabel("m", Overlay = true)]
	private float _worldSize;
	[ShowInInspector, ReadOnly, SuffixLabel("tx", Overlay = true)]
	private int _mapSize;
	[SerializeField, ReadOnly, SuffixLabel("tx", Overlay = true), Tooltip("If greater than 256, SetHeights duration is too long for one frame.")]
	private int _chunkMapSize = 256;
	[SerializeField, SuffixLabel("m", Overlay = true)]
	private int _chunkWorldSize = 128;
	[SerializeField, SuffixLabel("chunks", Overlay = true)]
	private int _chunksInRow = 10;
	[SerializeField, SuffixLabel("m", Overlay = true)]
	private float _maxWorldHeight = 100;
	[SerializeField, SuffixLabel("deg", Overlay = true)]
	private float transitionRange = 10;
	[SerializeField]
	private TerrainLayer[] terrainLayers;
	[SerializeField]
	private NodeGraph noiseGraph;
	[SerializeField]
	private NoisePreview baseNoise;

	public int ChunkMapSize { get => _chunkMapSize; private set => _chunkMapSize = value; }
	public int ChunkWorldSize { get => _chunkWorldSize; private set => _chunkWorldSize = value; }
	public int ChunksInRow { get => _chunksInRow; private set => _chunksInRow = value; }
	public float WorldSize { get => _worldSize; private set => _worldSize = value; }
	public int MapSize { get => _mapSize; private set => _mapSize = value; }
	public float MaxWorldHeight { get => _maxWorldHeight; private set => _maxWorldHeight = value; }
	public float MapToWorld => 1f * ChunkWorldSize / ChunkMapSize;
	public float WorldToMap => 1f * ChunkMapSize / ChunkWorldSize;

	public Terrain[] chunks;

	public Terrain[] chunksPool;

	private Dictionary<Terrain, EditorCoroutine> generatedChunksEditorCoroutines = new Dictionary<Terrain, EditorCoroutine>();
	private float[,] heights;

	private System.Diagnostics.Stopwatch stopwatch1 = new System.Diagnostics.Stopwatch();
	private System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();

	private EditorCoroutine generatorCoroutine;

	[Min(16)]
	public int chunkWorldSize = 512;
	[Min(16)]
	public int chunkMapSize = 256;
	[Min(1)]
	public int chunksInRow = 1;
	[Min(1)]
	public int splatLayers = 2;
	[ReadOnly]
	public int worldSize;
	[ReadOnly]
	public int worldMapSize;
	[ReadOnly]
	public int totalChunks;
	[ReadOnly]
	public int totalMapSamples;
	[ReadOnly]
	public float heightmapsMB;
	[ReadOnly]
	public float splatmapsMB;
	[ReadOnly]
	public float ssdLoadingTime;
	[ReadOnly]
	public float hddLoadingTime;

	private void OnValidate()
	{
		ChunkWorldSize = ChunkWorldSize / 2 * 2;
		ChunkMapSize = ChunkWorldSize * 2;
		WorldSize = ChunksInRow * ChunkWorldSize;
		MapSize = ChunksInRow * ChunkMapSize;

		totalChunks = chunksInRow * chunksInRow;
		worldSize = chunkWorldSize * chunksInRow;
		worldMapSize = chunkMapSize * chunksInRow;
		totalMapSamples = worldMapSize * worldMapSize;
		heightmapsMB = (float)sizeof(byte) * totalMapSamples / 1024 / 1024;
		splatmapsMB = (float)sizeof(byte) * splatLayers * totalMapSamples / 1024 / 1024;
		ssdLoadingTime = (heightmapsMB + splatmapsMB) / 500;
		hddLoadingTime = (heightmapsMB + splatmapsMB) / 60;


	}

	private void OnEnable()
	{
		if (!Application.isPlaying)
			EditorApplication.update += Update;
	}

	private void OnDisable()
	{
		if (!Application.isPlaying)
			EditorApplication.update -= Update;
	}

	//TODO: Create PlayMode coroutines
	//TODO: Replace with generation requests system in separate class, without coroutines
	private void Update()
	{
		Camera camera = Camera.current;
		if (camera && chunksPool != null && chunksPool.Length > 0)
		{
			Vector2Int cameraChunkPosition = new Vector2Int((int)(camera.transform.position.x / ChunkWorldSize), (int)(camera.transform.position.z / ChunkWorldSize));

			//Deactivate
			for (int i = 0; i < chunksPool.Length; i++) 
			{ 
				Terrain chunk = chunksPool[i];
				Vector2Int chunkPosition = new Vector2Int((int)(chunk.transform.position.x / ChunkWorldSize), (int)(chunk.transform.position.z / ChunkWorldSize));
				int range = ChunksInRow / 2;

				if (chunk.enabled && !new RectInt(cameraChunkPosition.x - range, cameraChunkPosition.y - range, range * 2 + 1, range * 2 + 1).Contains(chunkPosition))
				{
					chunk.enabled = false;
					Debug.Log($"Disabled {chunk}, {cameraChunkPosition} : {chunkPosition}", chunk);
				}
			}
			
			//Activate
			for (int cx = 0; cx < ChunksInRow; cx++)
				for (int cz = 0; cz < ChunksInRow; cz++)
				{
					Vector2Int chunkLocalPosition = new Vector2Int(cx - ChunksInRow / 2, cz - ChunksInRow / 2);
					Vector2Int chunkAbsolutePosition = cameraChunkPosition + chunkLocalPosition;

					if (chunkAbsolutePosition.x >= 0 && chunkAbsolutePosition.y >= 0)
					{
						if (!chunksPool.FirstOrDefault(t => t.enabled && Distance.Manhattan2D((int)(t.transform.position.x / ChunkWorldSize), (int)(t.transform.position.z / ChunkWorldSize), chunkAbsolutePosition.x, chunkAbsolutePosition.y) < 1))
						{
							Terrain chunk = chunksPool.FirstOrDefault(t => !t.enabled);
							if (chunk)
							{
								chunk.transform.position = new Vector3(chunkAbsolutePosition.x * ChunkWorldSize, -1000, chunkAbsolutePosition.y * ChunkWorldSize);
								chunk.enabled = true;
								if (generatedChunksEditorCoroutines.ContainsKey(chunk))
								{
									Debug.LogWarning("Clearing older (not finished) coroutine.", chunk);
									//EditorCoroutineUtility.StopCoroutine(generatedChunksEditorCoroutines[chunk]); 
									//BUG: When moving really fast, coroutine's callback, MoveNext, is not always properly removed from EditorApplication.update, causing null reference error
									//isGenerating = false;
									//TODO: Look for this bug with PlayMode coroutines
									generatedChunksEditorCoroutines.Remove(chunk);
								}
								generatedChunksEditorCoroutines.Add(chunk, EditorCoroutineUtility.StartCoroutineOwnerless(GenerateChunkCoroutine(chunkAbsolutePosition.x, chunkAbsolutePosition.y, chunk)));
								Debug.Log($"Enabled {chunk}, {cameraChunkPosition} + {chunkLocalPosition} = {chunkAbsolutePosition}", chunk);
							}
							else
							{
								Debug.LogWarning($"No inactive chunk available, {cameraChunkPosition} + {chunkLocalPosition} = {chunkAbsolutePosition}");
							}
						}
					}
				}
		}
	}

	[Button]
	private void CreateTerrainChunks()
	{
		if (chunks != null)
			ClearTerrainChunks();

		stopwatch1.Restart();

		TerrainData terrainData;
		Terrain terrain;
		chunks = new Terrain[ChunksInRow * ChunksInRow];
		chunksPool = new Terrain[ChunksInRow * ChunksInRow];
		for (int x = 0; x < ChunksInRow; x++)
			for (int z = 0; z < ChunksInRow; z++)
			{
				terrainData = new TerrainData()
				{
					name = $"TerrainData {x} {z}",
					heightmapResolution = ChunkMapSize + 1,
					alphamapResolution = ChunkMapSize,
					baseMapResolution = ChunkMapSize / 4,
					size = new Vector3(ChunkWorldSize, MaxWorldHeight, ChunkWorldSize),
					terrainLayers = terrainLayers
				};
				terrain = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
				terrain.name = $"TerrainChunk {x} {z}";
				terrain.gameObject.layer = gameObject.layer;
				terrain.transform.parent = transform;
				terrain.transform.localPosition = new Vector3(x * ChunkWorldSize, 0, z * ChunkWorldSize);
				terrain.drawTreesAndFoliage = false;
				terrain.bakeLightProbesForTrees = false;
				terrain.basemapDistance = 100;

				terrain.gameObject.AddComponent<NavMeshSurface>().collectObjects = CollectObjects.Children;
				
				chunks[x * ChunksInRow + z] = terrain;
				chunksPool[x * ChunksInRow + z] = terrain;
			}

		stopwatch1.Stop();
		Debug.Log($"Created chunks in {stopwatch1.Elapsed.TotalMilliseconds}ms");
	}

	[Button]
	private void ClearTerrainChunks()
	{
		if (chunks != null)
		{
			for (int i = 0; i < chunks.Length; i++)
				{ 
					if (chunks[i])
					{
						if (Application.isPlaying)
							Destroy(chunks[i].gameObject);
						else
							DestroyImmediate(chunks[i].gameObject);
					}
				}
			chunks = null;
		}
		else
		{
			if (!Application.isPlaying)
				for (int i = transform.childCount - 1; i >= 0; i--)
					DestroyImmediate(transform.GetChild(0).gameObject);
		}
	}

	[Button]
	private void GenerateTerrainHeightmaps()
	{
		if (generatorCoroutine != null)
			EditorCoroutineUtility.StopCoroutine(generatorCoroutine);
		generatorCoroutine = EditorCoroutineUtility.StartCoroutine(GenerateTerrainHeightmapsCoroutine(), this);
	}

	[Button]
	private void GenerateTerrainSplatmaps()
	{
		if (generatorCoroutine != null)
			EditorCoroutineUtility.StopCoroutine(generatorCoroutine);
		generatorCoroutine = EditorCoroutineUtility.StartCoroutine(GenerateTerrainSplatmapsCoroutine(), this);
	}

	[Button]
	private void GenerateNavMeshes()
	{
		if (generatorCoroutine != null)
			EditorCoroutineUtility.StopCoroutine(generatorCoroutine);
		generatorCoroutine = EditorCoroutineUtility.StartCoroutine(GenerateNavMeshesCoroutine(), this);
	}

	private IEnumerator GenerateNavMeshesCoroutine()
	{
		stopwatch1.Restart();

		for (int cx = 0; cx < ChunksInRow; cx++)
			for (int cz = 0; cz < ChunksInRow; cz++)
			{
				stopwatch2.Restart();

				chunks[cx * ChunksInRow + cz].GetComponent<NavMeshSurface>().BuildNavMesh();

				stopwatch2.Stop();
				Debug.Log($"BuildNavMesh in {stopwatch2.Elapsed.TotalMilliseconds}ms");

				yield return null;
			}

		stopwatch1.Stop();
		Debug.Log($"Generated all navMeshes in {stopwatch1.Elapsed.TotalMilliseconds}ms");
	}

	private bool isGenerating;
	private IEnumerator GenerateChunkCoroutine(int cx, int cz, Terrain chunk)
	{
		while (isGenerating)
			yield return null;

		isGenerating = true;

		Debug.Log($"Generate {chunk} at {cx}, {cz}", chunk);

		float[,] heights = new float[ChunkMapSize + 1, ChunkMapSize + 1];

		int counter = 0;
		for (int z = 0; z <= ChunkMapSize; z++)
			for (int x = 0; x <= ChunkMapSize; x++)
			{
				counter++;
				if (counter > Mathf.Pow(ChunkMapSize + 1, 2) / 4)
				{
					counter = 0;
					yield return null;
				}

				heights[z, x] = baseNoise.noise.Sample((cx * ChunkMapSize + x) * MapToWorld, (cz * ChunkMapSize + z) * MapToWorld, (int)WorldSize);
			}
		yield return null;

		chunk.terrainData.SetHeights(0, 0, heights);

		yield return null;

		float steepness;
		float transitionStart = 40;
		float[,,] splatmap = new float[ChunkMapSize, ChunkMapSize, terrainLayers.Length];

		counter = 0;
		for (int x = 0; x < ChunkMapSize; x++)
			for (int z = 0; z < ChunkMapSize; z++)
			{
				counter++;
				if (counter > Mathf.Pow(ChunkMapSize, 2) / 8)
				{
					counter = 0;
					yield return null;
				}

				float mapx = cx * (ChunkMapSize - 1) + x;
				float mapz = cz * (ChunkMapSize - 1) + z;
				float slopeX = MaxWorldHeight * (baseNoise.noise.Sample((mapx + 1) * MapToWorld, mapz * MapToWorld, (int)WorldSize) - baseNoise.noise.Sample((mapx - 1) * MapToWorld, mapz * MapToWorld, (int)WorldSize));
				float slopeZ = MaxWorldHeight * (baseNoise.noise.Sample(mapx * MapToWorld, (mapz + 1) * MapToWorld, (int)WorldSize) - baseNoise.noise.Sample(mapx * MapToWorld, (mapz - 1) * MapToWorld, (int)WorldSize));
				
				Vector3 normal = new Vector3(-slopeX * MapToWorld, MapToWorld, -slopeZ * MapToWorld);
				normal.Normalize();

				steepness = Mathf.Acos(Vector3.Dot(normal, Vector3.up)) * 57.29578f;

				if (debugNormals && (x == 0 || z == 0) && x < 100 && z < 100)
				{
					Vector3 point = new Vector3(mapx * MapToWorld, 0, mapz * MapToWorld);
					point.y = MaxWorldHeight * baseNoise.noise.Sample(mapx * MapToWorld, mapz * MapToWorld, (int)WorldSize);
					Debug.DrawLine(point, point + normal, Color.red, 4);
				}

				splatmap[z, x, 0] = Mathf.Clamp(steepness - transitionStart + transitionRange * 0.5f, 0f, transitionRange * 0.5f) / transitionRange;
				splatmap[z, x, 1] = 1f - Mathf.Clamp(steepness - transitionStart + transitionRange * 0.5f, 0f, transitionRange * 0.5f) / transitionRange;
			}

		yield return null;

		chunk.terrainData.SetAlphamaps(0, 0, splatmap);

		chunk.transform.position = new Vector3(chunk.transform.position.x, 0, chunk.transform.position.z);

		stopwatch1.Restart();
		yield return EditorCoroutineUtility.StartCoroutineOwnerless(chunk.GetComponent<NavMeshSurface>().BuildNavMeshAsync());
		stopwatch1.Stop();
		Debug.Log($"BuildNavMesh in {stopwatch1.Elapsed.TotalMilliseconds}ms");



		generatedChunksEditorCoroutines.Remove(chunk);
		isGenerating = false;
	}

	private IEnumerator GenerateTerrainHeightmapsCoroutine()
	{
		if (chunks == null)
		{
			ClearTerrainChunks();
			CreateTerrainChunks();
		}

		heights = new float[ChunkMapSize + 1, ChunkMapSize + 1];
		
		stopwatch1.Restart();

		for (int cx = 0; cx < ChunksInRow; cx++)
			for (int cz = 0; cz < ChunksInRow; cz++)
			{				
				stopwatch2.Restart();
				int counter = 0;
				for (int z = 0; z <= ChunkMapSize; z++)
					for (int x = 0; x <= ChunkMapSize; x++)
					{
						counter++;
						if(counter > Mathf.Pow(ChunkMapSize + 1, 2) / 4)
						{
							counter = 0;
							stopwatch2.Stop();
							Debug.Log($"Finished iteration in {stopwatch2.Elapsed.TotalMilliseconds}ms");
							yield return null;
							stopwatch2.Restart();
						}

						heights[z, x] = baseNoise.noise.Sample((cx * ChunkMapSize + x) * MapToWorld, (cz * ChunkMapSize + z) * MapToWorld, (int)WorldSize);
					}

				stopwatch2.Stop();
				Debug.Log($"Finished iteration in {stopwatch2.Elapsed.TotalMilliseconds}ms");

				yield return null;

				stopwatch2.Restart();

				chunks[cx * ChunksInRow + cz].terrainData.SetHeights(0, 0, heights);
				
				stopwatch2.Stop();
				Debug.Log($"SetHeights in {stopwatch2.Elapsed.TotalMilliseconds}ms");

				yield return null;
			}

		stopwatch1.Stop();
		Debug.Log($"Generated all heightmaps in {stopwatch1.Elapsed.TotalMilliseconds}ms");
	}


	private IEnumerator GenerateTerrainSplatmapsCoroutine()
	{
		if (chunks == null)
		{
			ClearTerrainChunks();
			CreateTerrainChunks();
		}

		stopwatch1.Restart();
		OutputNoise outputNoise = noiseGraph.nodes.Find(n => n is OutputNoise) as OutputNoise;
		Terrain chunk;
		float steepness;
		float transitionStart = 40;
		float[,,] splatmap = new float[ChunkMapSize, ChunkMapSize, terrainLayers.Length];
		for (int cx = 0; cx < ChunksInRow; cx++)
			for (int cz = 0; cz < ChunksInRow; cz++)
			{
				chunk = chunks[cx * ChunksInRow + cz];

				stopwatch2.Restart();
				int counter = 0;
				for (int x = 0; x < ChunkMapSize; x++)
					for (int z = 0; z < ChunkMapSize; z++)
					{
						counter++;
						if (counter > Mathf.Pow(ChunkMapSize + 1, 2) / 8)
						{
							counter = 0;
							stopwatch2.Stop();
							Debug.Log($"Finished iteration in {stopwatch2.Elapsed.TotalMilliseconds}ms");
							yield return null;
							stopwatch2.Restart();
						}

						float mapx = cx * (ChunkMapSize - 1) + x;
						float mapz = cz * (ChunkMapSize - 1) + z;
						float slopeX = MaxWorldHeight * (baseNoise.noise.Sample((mapx + 1) * MapToWorld , mapz * MapToWorld, (int)WorldSize) - baseNoise.noise.Sample((mapx - 1) * MapToWorld, mapz * MapToWorld, (int)WorldSize));
						float slopeZ = MaxWorldHeight * (baseNoise.noise.Sample(mapx * MapToWorld, (mapz + 1) * MapToWorld, (int)WorldSize) - baseNoise.noise.Sample(mapx * MapToWorld, (mapz - 1) * MapToWorld, (int)WorldSize));
						//float slopeX = MaxWorldHeight * (outputNoise.Sample((mapx + 1) * MapToWorld / WorldSize, mapz * MapToWorld / WorldSize) - outputNoise.Sample((mapx - 1) * MapToWorld / WorldSize, mapz * MapToWorld / WorldSize));
						//float slopeZ = MaxWorldHeight * (outputNoise.Sample(mapx * MapToWorld / WorldSize, (mapz + 1) * MapToWorld / WorldSize) - outputNoise.Sample(mapx * MapToWorld / WorldSize, (mapz - 1) * MapToWorld / WorldSize));

						Vector3 normal = new Vector3(-slopeX * MapToWorld, MapToWorld, -slopeZ * MapToWorld);					
						normal.Normalize();

						steepness = Mathf.Acos(Vector3.Dot(normal, Vector3.up)) * 57.29578f;

						if (debugNormals && (x == 0 || z == 0) && x < 100 && z < 100)
						{
							Vector3 point = new Vector3(mapx * MapToWorld, 0, mapz * MapToWorld);
							point.y = MaxWorldHeight * baseNoise.noise.Sample(mapx * MapToWorld, mapz * MapToWorld, (int)WorldSize);
							Debug.DrawLine(point, point + normal, Color.red, 4);
						}

						splatmap[z, x, 0] = Mathf.Clamp(steepness - transitionStart + transitionRange * 0.5f, 0f, transitionRange * 0.5f) / transitionRange;
						splatmap[z, x, 1] = 1f - Mathf.Clamp(steepness - transitionStart + transitionRange * 0.5f, 0f, transitionRange * 0.5f)/ transitionRange;
					}

				stopwatch2.Stop();
				Debug.Log($"Finished iteration in {stopwatch2.Elapsed.TotalMilliseconds}ms");

				yield return null;

				stopwatch2.Restart();

				chunk.terrainData.SetAlphamaps(0, 0, splatmap);

				stopwatch2.Stop();
				Debug.Log($"SetAlphamaps in {stopwatch2.Elapsed.TotalMilliseconds}ms");

				yield return null;
			}

		stopwatch1.Stop();
		Debug.Log($"Generated all splatmaps in {stopwatch1.Elapsed.TotalMilliseconds}ms");
	}

}
