using Sirenix.OdinInspector;
using UnityEngine;

public class WorldSizeCalculator : MonoBehaviour
{
	[SerializeField, Min(16)]
	public int chunkWorldSize = 512;
	[SerializeField, Min(16)]
	public int chunkMapSize = 256;
	[SerializeField, Min(1)]
	public int chunksInRow = 1;
	[SerializeField, Min(1)]
	public int splatLayers = 2;
	[SerializeField, ReadOnly]
	public int worldSize;
	[SerializeField, ReadOnly]
	public int worldMapSize;
	[SerializeField, ReadOnly]
	public int totalChunks;
	[SerializeField, ReadOnly]
	public int totalMapSamples;
	[SerializeField, ReadOnly]
	public float heightmapsMB;
	[SerializeField, ReadOnly]
	public float splatmapsMB;
	[SerializeField, ReadOnly]
	public float ssdLoadingTime;
	[SerializeField, ReadOnly]
	public float hddLoadingTime;

	private void OnValidate()
	{
		totalChunks = chunksInRow * chunksInRow;
		worldSize = chunkWorldSize * chunksInRow;
		worldMapSize = chunkMapSize * chunksInRow;
		totalMapSamples = worldMapSize * worldMapSize;
		heightmapsMB = (float)sizeof(byte) * totalMapSamples / 1024 / 1024;
		splatmapsMB = (float)sizeof(byte) * splatLayers * totalMapSamples / 1024 / 1024;
		ssdLoadingTime = (heightmapsMB + splatmapsMB) / 500;
		hddLoadingTime = (heightmapsMB + splatmapsMB) / 60;
	}
}