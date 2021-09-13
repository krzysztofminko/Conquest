using XNodeEditor;

[CustomNodeEditor(typeof(SumNoise))]
public class SumNoiseEditor : NoiseNodeEditor 
{
	private SumNoise node;

    public override void OnCreate()
    {
        base.OnCreate();
        node = target as SumNoise;
    }

    public override void OnBodyGUI() 
	{
        serializedObject.Update();

        NoiseNodeBodyGUI();
        NodeEditorGUILayout.PortPair(node.GetInputPort(nameof(node.inputs)), node.GetOutputPort(nameof(node.output)));

        serializedObject.ApplyModifiedProperties();
    }
}