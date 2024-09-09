using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitcher : Switcher
{
    [SerializeField] Vector3 posOffset;
    [SerializeField] Vector3 eulerAnglesOffset;
    [SerializeField] Vector3 openScale;
    [SerializeField] bool canClose;

    Vector3 initialPosition, initialRotation, initialScale;

    private void Awake()
    {
        initialPosition = transform.position;
        initialRotation = transform.eulerAngles;
        initialScale = transform.localScale;
    }

    public override void Activation()
    {
        if (IsActivated)
        {
            StartCoroutine(AnimateOpening(true));
        }
        else if (canClose)
        {
            StartCoroutine(AnimateOpening(false));
        }
    }

    IEnumerator AnimateOpening(bool isOpening)
    {
        Vector3 startEulers, startPos, startScale;
        Vector3 targetEulers, targetPos, targetScale;
        if (isOpening)
        {
            targetPos = transform.position + posOffset;
            targetEulers = transform.eulerAngles + eulerAnglesOffset;
            targetScale = openScale;
        }
        else
        {
            print("initial");
            targetPos = initialPosition;
            targetEulers = initialRotation;
            targetScale = initialScale;
        }
        startEulers = transform.eulerAngles;
        startPos = transform.position;
        startScale = transform.localScale;

        float t = 0;

        do
        {
            t = Mathf.Clamp(t + Time.deltaTime, 0, 1);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.eulerAngles = Vector3.Lerp(startEulers, targetEulers, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        } while (t < 1);
    }
}
