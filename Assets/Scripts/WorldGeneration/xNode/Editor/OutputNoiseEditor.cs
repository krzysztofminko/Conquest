using XNodeEditor;

[CustomNodeEditor(typeof(OutputNoise))]
public class OutputNoiseEditor : NoiseNodeEditor 
{
    private OutputNoise node;

    public override void OnCreate()
    {
        base.OnCreate();
        node = target as OutputNoise;
        alwaysEnabled = true;
    }

    public override void OnBodyGUI() 
	{
        serializedObject.Update();

        NoiseNodeBodyGUI();
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.inputNoise)));

        serializedObject.ApplyModifiedProperties();
    }
}