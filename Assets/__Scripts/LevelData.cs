using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] Finish fin;
    [SerializeField] int bounces3star;
    [SerializeField] int bounces2star;

    public void OnRestart()
    {
        if (fin == null)
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out fin))
                {
                    this.SmartLog("Found finish");
                    fin.finished = false;
                    return;
                }
            }
            this.SmartLog("Didn't find finish", fin);

        }
        fin.finished = false;
    }

    public Vector3 GetFinPos()
    {
        if (fin == null)
            return new Vector3(0, 4, 0);
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
