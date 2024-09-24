using UnityEngine;

public class LineController : MonoBehaviour
{
    public void UpdateState(float scale, Vector2 direction)
    {
        transform.localScale = new Vector3(1, scale * 1.5f, 1);

        var ang = Vector2.SignedAngle(direction, transform.up);
        transform.eulerAngles -= new Vector3(0, 0, ang);
    }
}
