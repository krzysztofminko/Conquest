using XNodeEditor;

[CustomNodeEditor(typeof(PerlinNoise))]
public class PerlinNoiseEditor : NoiseNodeEditor
{
	private PerlinNoise node;

	public override void OnCreate()
	{
		base.OnCreate();
		node = target as PerlinNoise;
	}

	public override void OnBodyGUI() 
	{
        serializedObject.Update();

		NoiseNodeBodyGUI();
		NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.scale)));
		NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.offset)));
		NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.noise)));

		serializedObject.ApplyModifiedProperties();
	}
}