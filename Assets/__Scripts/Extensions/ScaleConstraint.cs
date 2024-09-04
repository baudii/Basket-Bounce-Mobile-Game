using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleConstraint : MonoBehaviour
{
    [SerializeField] Transform source;

    Vector3 initialScale;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        transform.localScale = new Vector3(initialScale.x * source.localScale.x, initialScale.y * source.localScale.y, initialScale.z * source.localScale.z);
    }
}
