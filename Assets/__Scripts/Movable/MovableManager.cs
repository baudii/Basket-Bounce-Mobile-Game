using System.Collections.Generic;
using UnityEngine;

public class MovableManager : MonoBehaviour
{
    List<IMovableItem> movables;

    private void Awake()
    {
        movables = new List<IMovableItem>();
        foreach (Transform t in transform)
        {
            var imovables = t.GetComponents<IMovableItem>();
            foreach (var imovable in imovables)
                movables.Add(imovable);
        }
    }

    public void ResetMovables()
    {
        foreach (var mov in movables)
        {
            mov.ResetState();
        }
    }
}
