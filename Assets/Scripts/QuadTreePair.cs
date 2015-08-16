using UnityEngine;
using System.Collections.Generic;

public class QuadTreePair : HasRect
{

    public GameObject gameObject { get; private set; }
    public Rect rectangle { get; private set; }

    public QuadTreePair(GameObject gObj, Rect rect)
    {
        gameObject = gObj;
        rectangle = rect;
    }
}
