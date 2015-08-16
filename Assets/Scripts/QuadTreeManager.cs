using UnityEngine;
using System;
using System.Collections.Generic;

// The QuadTreeManager allow us to create only one tree per frame
public class QuadTreeManager<T> where T : HasRect
{

    // We need a static instance
    private static QuadTree<T> INSTANCE;
    // We need the last frame drawn
    private static int oldFrame = int.MinValue;

    public QuadTreeManager()
    {
        // If it is the first time we build it, we will instanciate the instance
        if (INSTANCE == null)
        {
            // The area we assume things will happend in is r
            // This could probably be made better, but still, it will work fine
            Rect r = new Rect(-250f, -250f, 1000f, 500f);
            // We create our tree in the area r
            INSTANCE = new QuadTree<T>(r);
        }
    }

    // All the items must be pushed together at once
    // The first to push in the tree during the current frame wins
    public QuadTree<T> pushIntoTree(List<T> item)
    {
        // If we haven't already pushed items this frame
        if (oldFrame < Time.frameCount)
        {
            // We clear the tree
            INSTANCE.Clear();

            // Then refill it
            foreach (T tItem in item)
            {
                INSTANCE.Insert(tItem);
            }

            // Finally we indicate we have pushed this frame
            oldFrame = Time.frameCount;
        }

        // The tree is returned to make it simplier
        return INSTANCE;
    }
}