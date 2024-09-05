using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
public class GFX_Shadow : MonoBehaviour
{
    [SerializeField] float xOffset = 0.05f;
    [SerializeField] float yOffset = -0.1f;
    [SerializeField] SpriteRenderer mainSr;
    [SerializeField] bool validate;
    SpriteRenderer sr;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (validate)
        {
            Adjust();
        }
    }

    [ContextMenu("Adjust")]
    public void Adjust()
    {
        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (mainSr == null)
            return;

        sr.sprite = mainSr.sprite;
        sr.size = new Vector2(mainSr.size.x, mainSr.size.y);

        transform.localPosition = new Vector2(xOffset, yOffset);
    }
#endif
}
