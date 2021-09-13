using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

public abstract class NoiseNode : Node
{
	public bool enabled = true;
	[Range(0f, 1f)]
	public float amplitude = 1;

	public abstract float Sample(float x, float y);

	//public IEnumerable<NoiseNode> GetConnectedNodes(string portName) => GetInputPort(portName)?.GetConnections().Where(c => (c.node as NoiseNode).enabled).Select(p => p.node as NoiseNode);

	public IEnumerable<NoiseNode> GetConnectedNodes(string portName)
	{
		return GetInputPort(portName)?.GetConnections().Where(c => (c.node as NoiseNode).enabled).Select(p => p.node as NoiseNode);
	}
	
}