using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class SamplerMap
{
	const int PREVIEW_SIZE = 256;

	[PreviewField(200, ObjectFieldAlignment.Center), ShowInInspector, ReadOnly, HideLabel]
	private Texture2D preview;

	[HideLabel, InlineProperty, OnValueChanged(nameof(OnValidateSettings), IncludeChildren = true), SerializeReference]
	public ISampler sampler;

	private void OnValidateSettings()
	{
		GeneratePreview();
	}

	private void GeneratePreview()
	{
		preview = new Texture2D(PREVIEW_SIZE, PREVIEW_SIZE);
		Color[] colorMap = new Color[PREVIEW_SIZE * PREVIEW_SIZE];
		float value;

		for (int x = 0; x < PREVIEW_SIZE; x++)
			for (int y = 0; y < PREVIEW_SIZE; y++)
			{
				value = sampler.Sample(x, y, PREVIEW_SIZE);
				colorMap[x * PREVIEW_SIZE + y] = new Color(value, value, value);
			}

		preview.SetPixels(colorMap);
		preview.Apply();
	}
}
