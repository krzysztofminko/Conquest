using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class ComposedSampler : ISampler
{
	public SamplerMap sampler1;
	public SamplerMap sampler2;
	public SamplerMap blend;
	public bool rangeModifier;
	[Range(0f, 1f), ShowIf(nameof(rangeModifier))]
	public float blendModifier;

	public float Sample(float x, float y, int size)
	{
		float sample1 = sampler1.sampler.Sample(x, y, size);
		float sample2 = sampler2.sampler.Sample(x, y, size);
		return rangeModifier ? sample1 * blendModifier + sample2 * (1f - blendModifier) : sample1 * blend.sampler.Sample(x, y, size) + sample2 * (1f - blend.sampler.Sample(x, y, size));
		
	}
}