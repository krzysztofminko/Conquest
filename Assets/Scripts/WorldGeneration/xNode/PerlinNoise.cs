using UnityEngine;
using UnityEngine.Profiling;

public class PerlinNoise : NoiseNode
{
	[Input] public float scale = 10;
	[Input] public Vector2 offset;
	[Output] public Object noise;

	//public override float Sample(float x, float y) => Mathf.PerlinNoise(offset.x + x * scale, offset.y + y * scale) * amplitude;

	public override float Sample(float x, float y) 
	{
		Profiler.BeginSample("PerlinNoise.Sample");
		float result = Mathf.PerlinNoise(offset.x + x * scale, offset.y + y * scale) * amplitude;
		Profiler.EndSample();
		return result; 
	}
}