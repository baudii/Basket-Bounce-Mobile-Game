using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Gradient lowStretchGradient;
    float minSpeed;
    Gradient normalGradient;
    bool isLowStretch;

    public void Init(float minSpeed)
    {
        this.minSpeed = minSpeed;
        normalGradient = lineRenderer.colorGradient;
    }

    public void UpdateState(float scale, Vector2 direction)
    {
        if (scale < minSpeed && !isLowStretch)
        {
            lineRenderer.colorGradient = lowStretchGradient;
            isLowStretch = true;
        }
        else if (scale >= minSpeed && isLowStretch)
        {
			lineRenderer.colorGradient = normalGradient;
			isLowStretch = false;
		}
        transform.localScale = new Vector3(1, scale * 1.5f, 1);

        var ang = Vector2.SignedAngle(direction, transform.up);
        transform.eulerAngles -= new Vector3(0, 0, ang);
    }
}
