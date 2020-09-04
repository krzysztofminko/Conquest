using UnityEngine;

[ExecuteAlways]
public class RotateWithMainCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;        
    }
}
