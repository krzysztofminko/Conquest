using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using XNode;
using Application = UnityEngine.Application;

[ExecuteAlways]
public class World : SerializedMonoBehaviour
{
	public static World Instance;

	[SerializeField]
	private bool debugNormals;
	[ShowInInspector, ReadOnly, SuffixLabel("m", Overlay = true)]
	private float _worldSize;
	[ShowInInspector, ReadOnly, SuffixLabel("tx", Overlay = true)]
	private int _mapSize;
	[SerializeField, SuffixLabel("tx", Overlay = true), Tooltip("If greater than 256, SetHeights duration is too long for one frame.")]
	private int _chunkMapSize = 128;
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
	public int Layers => terrainLayers.Length;

	public Terrain[] chunks;

	public Terrain[] chunksPool;

	private Dictionary<Terrain, EditorCoroutine> generatedChunksEditorCoroutines = new Dictionary<Terrain, EditorCoroutine>();
	private float[,] heights;

	private System.Diagnostics.Stopwatch stopwatch1 = new System.Diagnostics.Stopwatch();
	private System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();

	private EditorCoroutine generatorCoroutine;	

	private void OnValidate()
	{
		ChunkWorldSize = ChunkWorldSize / 2 * 2;
		WorldSize = ChunksInRow * ChunkWorldSize;
		MapSize = ChunksInRow * ChunkMapSize;
	}

	private void OnEnable()
	{
		if (!Application.isPlaying)
			EditorApplication.update += Update;
		Instance = this;
	}

	private void OnDisable()
	{
		if (!Application.isPlaying)
			EditorApplication.update -= Update;
	}

	private void Update()
	{
		
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
				terrain.basemapDistance = 800;
				terrain.drawInstanced = true;

				NavMeshSurface nms = terrain.gameObject.AddComponent<NavMeshSurface>();
				nms.collectObjects = CollectObjects.Children;
				nms.overrideTileSize = true;
				nms.tileSize = 64;

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

		generatedChunksEditorCoroutines.Clear();
		isGenerating = false;
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
		float transitionStart = 60;
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

	private IEnumerator LoadChunkCoroutine(int cx, int cz, Terrain chunk)
	{
		stopwatch1.Restart();
		ChunkLoader.Load(cx, cz, out float[,] heights, out float[,,] layers);
		stopwatch1.Stop();
		Debug.Log($"Loaded in {stopwatch1.Elapsed.TotalMilliseconds}ms");

		yield return null;

		stopwatch1.Restart();
		chunk.terrainData.SetHeights(0, 0, heights);
		stopwatch1.Stop();
		Debug.Log($"SetHeights in {stopwatch1.Elapsed.TotalMilliseconds}ms");

		yield return null;

		stopwatch1.Restart();
		chunk.terrainData.SetAlphamaps(0, 0, layers);
		stopwatch1.Stop();
		Debug.Log($"SetAlphamaps in {stopwatch1.Elapsed.TotalMilliseconds}ms");

		chunk.transform.position = new Vector3(chunk.transform.position.x, 0, chunk.transform.position.z);

		stopwatch1.Restart();
		yield return EditorCoroutineUtility.StartCoroutineOwnerless(chunk.GetComponent<NavMeshSurface>().BuildNavMeshAsync());
		stopwatch1.Stop();
		Debug.Log($"BuildNavMesh in {stopwatch1.Elapsed.TotalMilliseconds}ms");
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

		stopwatch1.Restart();
		ChunkLoader.Save(cx, cz, heights, splatmap);
		stopwatch1.Stop();
		Debug.Log($"Saved in {stopwatch1.Elapsed.TotalMilliseconds}ms");

		generatedChunksEditorCoroutines.Remove(chunk);
		isGenerating = false;
	}

}
