using UnityEngine;
using XNode;

public class PerlinNoiseOctaves : NoiseNode 
{
	[Input] public float scale = 1;
	[Input] public Vector2 offset;
	[Range(1, 16)]
	[Input] public int octaves = 4;
	[Input] public float lacunarity = 2;
	[Input] public float persistence = 0.5f;
	[Output] public Object noise;

	public override float Sample(float x, float y)
	{
		float tmpFrequency;
		float tmpAmplitude;
		float sumOfAmplitudes = 0;
		float result = 0;

		//Octaves
		for (int octave = 0; octave < octaves; octave++)
		{
			tmpFrequency = Mathf.Pow(lacunarity, octave);
			tmpAmplitude = Mathf.Pow(persistence, octave);
			sumOfAmplitudes += tmpAmplitude;
			result += Mathf.PerlinNoise(offset.x + tmpFrequency * x * scale, offset.y + tmpFrequency * y * scale) * tmpAmplitude;
		}

		//Normalize
		result /= sumOfAmplitudes;

		return result * amplitude;
	}
}