using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField] int bounces3star;
    [SerializeField] int bounces2star;

    public int ConvertToStars(int bounces)
    {
        if (bounces <= bounces3star)
            return 3;
        if (bounces <= bounces2star)
            return 2;
        return 1;
    }
}
