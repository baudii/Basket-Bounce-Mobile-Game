using System.Collections.Generic;
using UnityEngine;

public class ReflectionLine : MonoBehaviour
{
    [SerializeField] float ballRadius;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float maxLength;
    [SerializeField] int maxReflections;
    [SerializeField] LayerMask bouncable;

    List<Vector3> positions = new List<Vector3>();

    public void UpdateWorldSpace(Vector2 start, Vector2 dir, float stretchForce)
    {
        float currentLenght = 0;
        float currentMaxLength = maxLength;

        currentMaxLength *= Mathf.Clamp(stretchForce - 1, 0, 1);

        positions.Clear();
        positions.Add(start);

        int reflections = 0;

        if (maxReflections <= 0)
        {
            maxReflections = 4;
        }

        Vector2 bouncePosition;

        while (currentLenght < currentMaxLength)
        {
            RaycastHit2D hit = Physics2D.CircleCast(start, ballRadius, dir, currentMaxLength - currentLenght, bouncable);

            if (hit.collider == null)
            {
                Vector3 lastPos = start + dir * (currentMaxLength - currentLenght);

                positions.Add(lastPos);
                break;
            }

            dir = Vector2.Reflect(dir, hit.normal);
            if (hit.collider.isTrigger)
            {
                if (hit.collider.TryGetComponent(out BouncePad bp))
                {
                    dir = bp.transform.up.normalized;
                }
                else
                {
                    Vector3 lastPos = hit.point;
                    positions.Add(lastPos);
                    break;
                }
            }

            bouncePosition = hit.point + hit.normal * ballRadius;
            currentLenght += Vector2.Distance(start, bouncePosition);
            start = bouncePosition;
            positions.Add(start);

            reflections++;

            if (reflections > maxReflections)
                break;
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}
