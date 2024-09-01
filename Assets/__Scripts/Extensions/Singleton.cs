using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                T[] instances = FindObjectsOfType<T>();
                if (instances.Length > 0)
                {
                    instance = instances[0];
                    for (int i = 1; i < instances.Length; i++)
                        Destroy(instances[i].gameObject);
                }
                else
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }
}
