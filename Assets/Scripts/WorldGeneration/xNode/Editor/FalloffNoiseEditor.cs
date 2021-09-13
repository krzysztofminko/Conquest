using XNodeEditor;

[CustomNodeEditor(typeof(FalloffNoise))]
public class FalloffNoiseEditor : NoiseNodeEditor 
{
    private FalloffNoise node;

    public override void OnCreate()
    {
        base.OnCreate();
        node = target as FalloffNoise;
    }

    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NoiseNodeBodyGUI();
        NodeEditorGUILayout.PortPair(node.GetInputPort(nameof(node.input)), node.GetOutputPort(nameof(node.output)));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.square)));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.size)));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.curve)));

        serializedObject.ApplyModifiedProperties();
    }
}