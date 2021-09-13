using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(NoiseNode))]
public class NoiseNodeEditor : NodeEditor 
{
	const int PREVIEW_SIZE = 175;

	private Texture2D preview;
	private Color[] colorMap;
	private NoiseNode node;
	public bool alwaysEnabled;

	public override void OnCreate()
	{
		base.OnCreate();
		preview = new Texture2D(PREVIEW_SIZE, PREVIEW_SIZE);
		colorMap = new Color[PREVIEW_SIZE * PREVIEW_SIZE];
		node = target as NoiseNode;
		GeneratePreview(PREVIEW_SIZE);
		onUpdateNode += delegate { GeneratePreview(PREVIEW_SIZE); };
	}

	public override void OnHeaderGUI()
	{
		if (!alwaysEnabled)
		{
			GUILayout.BeginHorizontal();
			node.enabled = GUILayout.Toggle(node.enabled, string.Empty, new GUIStyle(GUI.skin.toggle) { fixedHeight = 30, fixedWidth = 30 });
		}
		GUILayout.Label(target.name, new GUIStyle(NodeEditorResources.styles.nodeHeader) { normal = new GUIStyleState() { textColor = node.enabled ? Color.white : Color.black } }, GUILayout.Height(30));
		if (!alwaysEnabled)
		{
			GUILayout.Space(30);
			GUILayout.EndHorizontal();
		}
	}

	public void NoiseNodeBodyGUI()
	{
		if (GUILayout.Button(new GUIContent(preview, "Click to refresh"), new GUIStyle() { margin = new RectOffset(0, 0, 0, 0) }, GUILayout.Width(175)) || !preview || serializedObject.hasModifiedProperties)
			GeneratePreview(PREVIEW_SIZE);
		NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.amplitude)));
	}

	private void GeneratePreview(int size)
	{
		float value;

		for (int y = 0; y < size; y++)
			for (int x = 0; x < size; x++)
			{
				value = node.Sample((float)x / size, (float)y / size);
				colorMap[y * size + x] = new Color(value, value, value);
			}
		
		preview.SetPixels(colorMap);
		preview.Apply(false);
	}
}