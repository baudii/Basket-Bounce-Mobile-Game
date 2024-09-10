using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] Finish fin;
    [SerializeField] int bounces3star;
    [SerializeField] int bounces2star;

    public void OnRestart()
    {
        fin.finished = false;
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
