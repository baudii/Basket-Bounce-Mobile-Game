using UnityEngine;
using UnityEngine.UI;

public class LevelData : MonoBehaviour
{
    [SerializeField] Finish fin;
    [SerializeField] int bounces3star;
    [SerializeField] int bounces2star;
    [SerializeField] MovableManager movableManager;

    [SerializeField] bool validateShadows;
    [SerializeField] bool shadowsEnabled;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (validateShadows)
        {
            AdjustShadows(shadowsEnabled);
        }
    }

    public void AdjustShadows(bool enable = true)
    {
        transform.ForAllDescendants(child =>
        {
            if (child.TryGetComponent(out GFX_Shadow shadow))
            {
                shadow.Adjust();
                shadow.gameObject.SetActive(enable);
            }
        });
    }
#endif

    public void OnRestart()
    {
        fin.finished = false;
        if (movableManager != null)
        {
            movableManager.ResetMovables();
        }
    }

    public Vector3 GetFinPos()
    {
        return fin.transform.position;
    }

    public int ConvertToStars(int bounces)
    {
        if (bounces <= bounces3star)
            return 3;
        if (bounces <= bounces2star)
            return 2;
        return 1;
    }
}
