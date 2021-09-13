using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class NoisePreview
{
	const int PREVIEW_SIZE = 256;

	[PreviewField(200, ObjectFieldAlignment.Center), ShowInInspector, ReadOnly, HideLabel]
	private Texture2D preview;

	[HideLabel, InlineProperty, OnValueChanged(nameof(OnValidateSettings), IncludeChildren = true)]
	public Noise noise;

	private void OnValidateSettings()
	{
		noise.range = new Vector2((float)Math.Round(noise.range.x, 2), (float)Math.Round(noise.range.y, 2));

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
				value = noise.Sample(x, y, PREVIEW_SIZE);
				colorMap[x * PREVIEW_SIZE + y] = new Color(value, value, value);
			}

		preview.SetPixels(colorMap);
		preview.Apply();
	}
}
