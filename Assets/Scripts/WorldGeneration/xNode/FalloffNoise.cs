using UnityEngine;
using XNode;

public class FalloffNoise : NoiseNode 
{
	[Input(connectionType = ConnectionType.Override)]//, backingValue = ShowBackingValue.Never)]
	public Object input;
	public bool square = true;
	public float size = 1;
	public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	[Output(connectionType = ConnectionType.Override)]
	public Object output;

	private NoiseNode inputNoiseNode;

	public override void OnCreateConnection(NodePort from, NodePort to)
	{
		if (to.node == this)
			inputNoiseNode = from.node as NoiseNode;
	}

	public override void OnRemoveConnection(NodePort port)
	{
		if (port.fieldName == nameof(input))
			inputNoiseNode = null;
	}

	public override float Sample(float x, float y)
	{
		if (inputNoiseNode != null)
		{
			float result = inputNoiseNode.Sample(x, y);

			if (square)
				result *= curve.Evaluate(Mathf.Clamp01(2 - size - Mathf.Max(Mathf.Abs(x - size * 0.5f), Mathf.Abs(y - size * 0.5f)) / Mathf.Max(Mathf.Abs(size), Mathf.Abs(size))));
			else
				result *= curve.Evaluate(Mathf.Clamp01(2 - size - new Vector2(x - size * 0.5f, y - size * 0.5f).magnitude / Mathf.Max(Mathf.Abs(size), Mathf.Abs(size))));

			return result * amplitude;
		}
		return 0;
	}
}