using UnityEngine;
using XNode;

public class SumNoise : NoiseNode 
{
	[Input(connectionType = ConnectionType.Multiple)]//, backingValue = ShowBackingValue.Never)]
	public Object inputs;
	[Output(connectionType = ConnectionType.Override)] 
	public Object output;

	private NodePort inputPort;

	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		if (to.node == this)
			inputPort = GetInputPort(nameof(inputs));
	}

	public override void OnRemoveConnection(NodePort port)
	{
		if (port.fieldName == nameof(inputs))
			inputPort = null;
	}

	public override float Sample(float x, float y)
	{
		NoiseNode inputNode;
		float sum = 0;
		float sumOfAmplitudes = 0;

		for (int i = 0; i < inputPort.ConnectionCount; i++)
		{
			inputNode = inputPort.GetConnection(i).node as NoiseNode;
			if (inputNode.enabled)
			{
				sum += inputNode.Sample(x, y);
				sumOfAmplitudes += inputNode.amplitude;
			}
		}

		return sumOfAmplitudes > amplitude ? sum / sumOfAmplitudes * amplitude : sum * amplitude;
	}
}