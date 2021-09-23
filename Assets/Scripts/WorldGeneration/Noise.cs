using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;

//TODO: Make Noise or ISampler to ScriptableObject
//TODO: Merge SamplerMap with ISampler
[Serializable]
public class Noise : ISampler
{
	public enum FalloffType { None, Square, Round }

	public Vector2 offset;
	public float scale = 10;

	[Range(1, 16)]
	public int octaves = 4;
	[ShowIf(nameof(octavesMoreThan1))]
	public float lacunarity = 2;
	[ShowIf(nameof(octavesMoreThan1))]
	public float persistence = 0.5f;

	public AnimationCurve heightOverride = AnimationCurve.Linear(0, 0, 1, 1);

	public FalloffType falloffType;
	[HideIf(nameof(falloffTypeEqualsNone))]
	public AnimationCurve falloffCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	[HideIf(nameof(falloffTypeEqualsNone)), Range(0, 2)]
	public float falloffSize = 1;

	public float perturbFreq = 32;
	public float perturbAmp = 32;

	private bool falloffTypeEqualsNone => falloffType == FalloffType.None;
	private bool octavesMoreThan1 => octaves > 1;

	public float Sample(float x, float y, int size)
	{
		//Perturbation
		float result = Perturb(x, y, perturbFreq, perturbAmp, size);

		//Falloff
		if (falloffType == FalloffType.Round)
			result *= falloffCurve.Evaluate(Mathf.Clamp01(2 - falloffSize - new Vector2(x - size * 0.5f, y - size * 0.5f).magnitude / Mathf.Max(Mathf.Abs(size), Mathf.Abs(size))));
		else if (falloffType == FalloffType.Square)
			result *= falloffCurve.Evaluate(Mathf.Clamp01(2 - falloffSize - Mathf.Max(Mathf.Abs(x - size * 0.5f), Mathf.Abs(y - size * 0.5f)) / Mathf.Max(Mathf.Abs(size), Mathf.Abs(size))));

		//Height curve
		result = heightOverride.Evaluate(result);

		return result;
	}

	private float SampleOctaves(float x, float y, int size)
	{
		float result = 0; 
		float frequency = 1;
		float amplitude = 1;
		float sumOfAmplitudes = 0;

		//Octaves
		for (int octave = 0; octave < octaves; octave++)
		{
			frequency *= lacunarity;
			amplitude *= persistence;
			result += Mathf.PerlinNoise(offset.x + x * frequency * scale / size, offset.y + y * frequency * scale / size) * amplitude;
			sumOfAmplitudes += amplitude;
		}

		//Normalize
		result /= sumOfAmplitudes;

		return result;
	}
	
	public float[,] Generate(int size)
	{
		float[,] result = new float[size, size];
		for (int x = 0; x < size; x++)
			for (int y = 0; y < size; y++)
				result[x,y] = Sample(x, y, size);

		return result;
	}

	public float Perturb(float x, float y, float frequency, float amplitude, int size)
	{				
		return SampleOctaves(x + Mathf.PerlinNoise(frequency * x / size, frequency * y / size) * amplitude, y + Mathf.PerlinNoise(frequency * x / size, frequency * y / size) * amplitude, size);
	}

	public float[,] Perturb(float[,] noise, float f, float d)
	{
		int u, v;
		int size = noise.GetLength(0);
		Assert.IsTrue(size == noise.GetLength(1));

		float[,] result = new float[size, size];
		for (int x = 0; x < size; ++x)
			for (int y = 0; y < size; ++y)
			{
				u = x + (int)(Mathf.PerlinNoise(f * x / size, f * y / size) * d);
				v = y + (int)(Mathf.PerlinNoise(f * x / size, f * y / size) * d);
				if (u < 0) u = 0; if (u >= size) u = size - 1;
				if (v < 0) v = 0; if (v >= size) v = size - 1;

				result[x, y] = noise[u, v];
			}

		return result;
	}


	public float[,] Erode(float[,] noise, float smoothness)
	{
		int size = noise.GetLength(0);
		Assert.IsTrue(size == noise.GetLength(1));

		for (int x = 1; x < size - 1; x++)
			for (int y = 1; y < size - 1; y++)
			{
				float d_max = 0.0f;
				int[] match = { 0, 0 };

				for (int u = -1; u <= 1; u++)
					for (int v = -1; v <= 1; v++)
					{
						if (Math.Abs(u) + Math.Abs(v) > 0)
						{
							float d_i = noise[x, y] - noise[x + u, y + v];
							if (d_i > d_max)
							{
								d_max = d_i;
								match[0] = u;
								match[1] = v;
							}
						}
					}

				if (0 < d_max && d_max <= (smoothness / size))
				{
					float d_h = 0.5f * d_max;
					noise[x, y] -= d_h;
					noise[x + match[0], y + match[1]] += d_h;
				}
			}

		return noise;
	}


	public float[,] Smoothen(float[,] noise)
	{
		int size = noise.GetLength(0);
		Assert.IsTrue(size == noise.GetLength(1));

		for (int x = 1; x < size - 1; ++x)
			for (int y = 1; y < size - 1; ++y)
			{
				float total = 0.0f;
				for (int u = -1; u <= 1; u++)
					for (int v = -1; v <= 1; v++)
						total += noise[x + u, y + v];

				noise[x, y] = total / 9.0f;
			}
		return noise;
	}

}
