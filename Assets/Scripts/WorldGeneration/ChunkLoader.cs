using System;
using System.Collections;
using System.IO;
using UnityEngine;

public static class ChunkLoader
{
	//byte[] chunk heights
	//byte[] chunk layers

	public static void Load(int chunkx, int chunkz, out float[,] heights, out float[,,] layers)
	{
		heights = new float[World.Instance.ChunkMapSize + 1, World.Instance.ChunkMapSize + 1];
		layers = new float[World.Instance.ChunkMapSize, World.Instance.ChunkMapSize, World.Instance.Layers];

		using (BinaryReader reader = new BinaryReader(File.Open($"{Application.persistentDataPath}/chunk{chunkx}_{chunkz}.data", FileMode.Open)))
		{
			for (int x = 0; x < World.Instance.ChunkMapSize + 1; x++)
				for (int z = 0; z < World.Instance.ChunkMapSize + 1; z++)
					heights[x, z] = reader.ReadSingle();

			for (int x = 0; x < World.Instance.ChunkMapSize; x++)
				for (int z = 0; z < World.Instance.ChunkMapSize; z++)
					for (int l = 0; l < World.Instance.Layers; l++)
						layers[x, z, l] = reader.ReadSingle();
		}
	}

	public static void Save(int chunkx, int chunkz, float[,] heights, float[,,] layers)
	{
		using(BinaryWriter writer = new BinaryWriter(File.Open($"{Application.persistentDataPath}/chunk{chunkx}_{chunkz}.data", FileMode.Create)))
		{
			for (int x = 0; x < World.Instance.ChunkMapSize + 1; x++)
				for (int z = 0; z < World.Instance.ChunkMapSize + 1; z++)
					writer.Write(heights[x, z]);

			for (int x = 0; x < World.Instance.ChunkMapSize; x++)
				for (int z = 0; z < World.Instance.ChunkMapSize; z++)
					for (int l = 0; l < World.Instance.Layers; l++)
						writer.Write(layers[x, z, l]);
		}
	}

}