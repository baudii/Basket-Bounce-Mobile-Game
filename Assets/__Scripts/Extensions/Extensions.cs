using UnityEngine;
using System;
using System.Runtime.CompilerServices;

public static class Extensions
{
    public static Vector3 WhereX(this Vector3 vector, float x)
    {
        vector.x = x;
        return vector;
    }
    public static Vector3 WhereY(this Vector3 vector, float y)
    {
        vector.y = y;
        return vector;
    }
    public static Vector3 WhereZ(this Vector3 vector, float z)
    {
        vector.z = z;
        return vector;
    }
    public static Vector3 AddTo(this Vector3 vector, float x = 0, float y = 0, float z = 0)
    {
        vector.x += x;
        vector.y += y;
        vector.z += z;
        return vector;
    }
    public static Vector3 Multiply(this Vector3 vector, float x = 1, float y = 1, float z = 1)
    {
        vector.x *= x;
        vector.y *= y;
        vector.z *= z;
        return vector;
    }    
    public static Vector3 Multiply(this Vector3 vector, Vector3 otherVector)
    {
        vector.x *= otherVector.x;
        vector.y *= otherVector.y;
        vector.z *= otherVector.z;
        return vector;
    }
    
    public static Vector3 Divide(this Vector3 vector, float x = 1, float y = 1, float z = 1)
    {
        vector.x /= x;
        vector.y /= y;
        vector.z /= z;
        return vector;
    }
    public static Vector3 Divide(this Vector3 vector, Vector3 otherVector)
    {
        vector.x /= otherVector.x;
        vector.y /= otherVector.y;
        vector.z /= otherVector.z;
        return vector;
    }

    public static Vector2 WhereX(this Vector2 vector, float x)
    {
        vector.x = x;
        return vector;
    }
    public static Vector2 WhereY(this Vector2 vector, float y)
    {
        vector.y = y;
        return vector;
    }

    public static Vector2 AddTo(this Vector2 vector, float x = 0, float y = 0)
    {
        vector.x += x;
        vector.y += y;
        return vector;
    }
    public static Vector2 Multiply(this Vector2 vector, float x = 1, float y = 1)
    {
        vector.x *= x;
        vector.y *= y;
        return vector;
    }

    public static Vector2 Direction(this Vector2 from, Vector2 to)
    {
        var heading = to - from;
        return heading.normalized;
    }    
    public static Vector3 Direction(this Vector3 from, Vector3 to)
    {
        var heading = to - from;
        return heading.normalized;
    }
    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    #region String


    public static bool ContainsSpecialChar(this string mainString, string chars)
    {
        foreach (var ch in chars)
        {
            if (mainString.Contains(ch))
                return true;
        }
        return false;
    }

    #endregion

    #region Coroutine
    /// <summary> Calls function with delay (seconds). Coroutine </summary>
    public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, float delay, bool scaledTime = false) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, delay, scaledTime));

    /// <summary> Calls function with delay (Update frames). Coroutine </summary>
    public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, int framesDelay) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, framesDelay));

    /// <summary> Calls function when "predicate" function evaluates to true. Coroutine </summary>
    public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, Func<bool> predicate, float minTime = 0) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, predicate, minTime));

    #endregion

    /// <summary> Logs out a message to the Console in format: "[callerScript.callerMethodName()]: message" </summary>
    /// <param name="message">Log message</param>
    /// <param name="callerMethodName">Leave empty for automatic recognition of method name.</param>
    public static void SmartLog(this MonoBehaviour callerScript, params object[] messages)
    {
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        System.Reflection.MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();

        string log = "[" + callerScript.GetType().ToString() + "." + methodBase.Name + "()]";
        foreach (var message in messages)
            log += (" " + message);
        Debug.Log(log);
    }
    /// <summary> Logs out a message to the Console in format: "[callerScript.callerMethodName()]: message" </summary>
    /// <param name="message">Log message</param>
    /// <param name="callerMethodName">Leave empty for automatic recognition of method name.</param>
    public static void SmartError(this MonoBehaviour callerScript, object message, [CallerMemberName] string callerMethodName = "")
    {
        string log = "[" + callerScript.GetType().ToString() + "." + callerMethodName + "()]: " + message;
        Debug.LogError(log);
    }
    /// <summary> Logs out a message to the Console in format: "[callerScript.callerMethodName()]: message" </summary>
    /// <param name="message">Log message</param>
    /// <param name="callerMethodName">Leave empty for automatic recognition of method name.</param>
    public static void SmartWarning(this MonoBehaviour callerScript, object message, [CallerMemberName] string callerMethodName = "")
    {
        string log = "[" + callerScript.GetType().ToString() + "." + callerMethodName + "()]: " + message;
        Debug.LogWarning(log);
    }
    /// <summary>
    /// Perform action on every child including root parent
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="action"></param>
    public static void ForEachChild(this Transform parent, System.Action<Transform> action)
    {
        action?.Invoke(parent);

        foreach (Transform child in parent)
        {
            ForEachChild(child, action);
        }
    }
}
