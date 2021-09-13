using UnityEngine;
using UnityEngine.Profiling;
using XNode;

public class OutputNoise : NoiseNode 
{
	[Input(connectionType = ConnectionType.Override, backingValue = ShowBackingValue.Never)]
	public Object inputNoise;

	private NoiseNode inputNoiseNode;

	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		if (to.node == this)
			inputNoiseNode = from.node as NoiseNode;
	}

	public override void OnRemoveConnection(NodePort port)
	{
		if (port.fieldName == nameof(inputNoise))
			inputNoiseNode = null;
	}

	public override float Sample(float x, float y)
	{
		Profiler.BeginSample("OutputNoise.Sample");
		 
		float result = inputNoiseNode != null && inputNoiseNode.enabled ? inputNoiseNode.Sample(x, y) * amplitude : 0;

		Profiler.EndSample();
		return result;
	}
}