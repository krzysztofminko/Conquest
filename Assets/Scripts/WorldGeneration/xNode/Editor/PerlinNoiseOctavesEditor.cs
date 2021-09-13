using XNodeEditor;

[CustomNodeEditor(typeof(PerlinNoiseOctaves))]
public class PerlinNoiseOctavesEditor : NoiseNodeEditor
{
    private PerlinNoiseOctaves node;

	public override void OnCreate()
	{
        base.OnCreate();
        node = target as PerlinNoiseOctaves;
    }

	public override void OnBodyGUI()
    {
        serializedObject.Update();

        NoiseNodeBodyGUI();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.scale)));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.offset)));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.octaves)));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.lacunarity)));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.persistence)));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.noise)));

        serializedObject.ApplyModifiedProperties();
    }
}