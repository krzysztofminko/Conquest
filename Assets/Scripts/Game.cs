using UnityEngine;

[DisallowMultipleComponent]
public class Game : MonoBehaviour
{
    public static bool IsQuitting { get; private set; }

    private void OnApplicationQuit()
    {
        IsQuitting = true;
    }
}
