using UnityEngine;

[ExecuteAlways]
public class PositionToTerrain : MonoBehaviour
{
	private void Update()
	{
		foreach (Terrain terrain in Terrain.activeTerrains)
		{
			if (new Rect(terrain.transform.position.x, terrain.transform.position.z, terrain.terrainData.size.x, terrain.terrainData.size.z).Contains(new Vector2(transform.position.x, transform.position.z)))
			{
				transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position) + terrain.transform.position.y, transform.position.z);
				break;
			}
		}
	}
}
