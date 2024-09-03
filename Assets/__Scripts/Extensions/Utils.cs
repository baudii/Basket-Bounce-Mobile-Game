using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using UnityEngine;

public static class Utils
{
    public static IEnumerator Co_DelayedExecute(Action action, int framesDelay)
    {
        for (int i = 0; i < framesDelay; i++)
        {
            yield return null;
        }

        action();
    }
    public static IEnumerator Co_DelayedExecute(Action action, float delay, bool scaledTime = true)
    {
        if (scaledTime)
            yield return new WaitForSeconds(delay);
        else
            yield return new WaitForSecondsRealtime(delay);

        action();
    }
    public static IEnumerator Co_DelayedExecute(Action action, Func<bool> predicate, float minTime)
    {
        if (minTime > 0)
            yield return new WaitForSeconds(minTime);
        yield return new WaitUntil(predicate);
        action();
    }

    public static async Task DelayedExecuteAsync(Action task, int milliseconds, CancellationToken token = default)
    {
        try
        {
            await Task.Delay(milliseconds, token);
            task();
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { throw e; }
    }
    public static async Task DelayedExecuteAsync(Action action, Func<CancellationToken, Task<bool>> predicate, CancellationToken token = default)
    {
        try
        {
            if (await predicate(token))
                action();
        }
        catch (OperationCanceledException) { }
        catch (Exception e) { throw e; }
    }
    public static TextMesh CreateWorldText(string text, Vector3 localPosition, bool is3D = false, int fontSize = 40)
    {
        var localEulerAngles = Vector3.zero;
        if (is3D)
            localEulerAngles.x = 90;
        return CreateWorldText(null, text, localPosition, Vector3.one / 20, localEulerAngles, fontSize, Color.white, TextAnchor.UpperLeft, TextAlignment.Left, 100);
    }
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, Vector3 scale, Vector3 localEulerAngles, int fontSize, Color color, TextAnchor textAnchor, TextAlignment alignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.localScale = scale;
        transform.localEulerAngles = localEulerAngles;

        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = alignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

        return textMesh;
    }

    public static bool DoesElementExistInArray<T>(T element, T[] array) where T: IEquatable<T>
    {
        foreach (T el in array)
        {
            if (el.Equals(element))
                return true;
        }

        return false;
    }
}
