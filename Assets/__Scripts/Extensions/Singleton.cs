using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	static T instance;
	public static T Instance => instance;
	private void Awake()
	{
		if (instance != null)
			Destroy(gameObject);
		else
		{
			instance = this as T;
			DontDestroyOnLoad(gameObject);
			OnAwake();
		}
	}

	protected virtual void OnAwake() { }
}
