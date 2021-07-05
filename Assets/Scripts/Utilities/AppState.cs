using UnityEngine;

public class AppState : MonoBehaviour
{
	public static bool IsQuitting { get; private set; } = false;
	public static bool IsFocused { get; private set; }
	public static bool IsPaused { get; private set; }

	private void OnApplicationQuit() => IsQuitting = true;
	private void OnApplicationFocus(bool focus) => IsFocused = focus;
	private void OnApplicationPause(bool pause) => IsPaused = pause;
}
