using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] Finish fin;
    [SerializeField] int bounces3star;
    [SerializeField] int bounces2star;

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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ball ball))
        {
            ball.Die();
        }
    }
}
