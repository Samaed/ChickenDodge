using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// The QuadTreeNode
/// </summary>
/// <typeparam name="T"></typeparam>
public class QuadTreeNode<T> where T : HasRect
{
    /// <summary>
    /// Construct a quadtree node with the given bounds 
    /// </summary>
    /// <param name="area"></param>
    public QuadTreeNode(Rect bounds)
    {
        m_bounds = bounds;
    }

    /// <summary>
    /// The area of this node
    /// </summary>
    Rect m_bounds;

    /// <summary>
    /// The contents of this node.
    /// Note that the contents have no limit: this is not the standard way to impement a QuadTree
    /// </summary>
    List<T> m_contents = new List<T>();

    /// <summary>
    /// The child nodes of the QuadTree
    /// </summary>
    List<QuadTreeNode<T>> m_nodes = new List<QuadTreeNode<T>>(4);

    /// <summary>
    /// Is the node empty
    /// </summary>
    public bool IsEmpty { get { return m_nodes.Count == 0; } }

    /// <summary>
    /// Area of the quadtree node
    /// </summary>
    public Rect Bounds { get { return m_bounds; } }

    /// <summary>
    /// Total number of nodes in the this node and all SubNodes
    /// </summary>
    public int Count
    {
        get
        {
            int count = 0;

            foreach (QuadTreeNode<T> node in m_nodes)
                count += node.Count;

            count += this.Contents.Count;

            return count;
        }
    }

    /// <summary>
    /// Return the contents of this node and all subnodes in the true below this one.
    /// </summary>
    public List<T> SubTreeContents
    {
        get
        {
            List<T> results = new List<T>();

            foreach (QuadTreeNode<T> node in m_nodes)
                results.AddRange(node.SubTreeContents);

            results.AddRange(this.Contents);
            return results;
        }
    }

    public List<T> Contents { get { return m_contents; } }

    /// <summary>
    /// Query the QuadTree for items that are in the given area
    /// </summary>
    /// <param name="queryArea"></pasram>
    /// <returns></returns>
    public List<T> Query(Rect queryArea)
    {
        // create a list of the items that are found
        List<T> results = new List<T>();

        // this quad contains items that are not entirely contained by
        // it's four sub-quads. Iterate through the items in this quad 
        // to see if they intersect.
        foreach (T item in this.Contents)
        {
            if (queryArea.Overlaps(item.rectangle))
                results.Add(item);
        }

        foreach (QuadTreeNode<T> node in m_nodes)
        {
            if (node.IsEmpty)
                continue;

            // Case 1: search area completely contained by sub-quad
            // if a node completely contains the query area, go down that branch
            // and skip the remaining nodes (break this loop)
            if (Contains(node.Bounds, queryArea))
            {
                results.AddRange(node.Query(queryArea));
                break;
            }

            // Case 2: Sub-quad completely contained by search area 
            // if the query area completely contains a sub-quad,
            // just add all the contents of that quad and it's children 
            // to the result set. You need to continue the loop to test 
            // the other quads
            if (Contains(queryArea, node.Bounds))
            {
                results.AddRange(node.SubTreeContents);
                continue;
            }

            // Case 3: search area intersects with sub-quad
            // traverse into this quad, continue the loop to search other
            // quads
            if (node.Bounds.Overlaps(queryArea))
            {
                results.AddRange(node.Query(queryArea));
            }
        }


        return results;
    }

    /// <summary>
    /// Insert an item to this node
    /// </summary>
    /// <param name="item"></param>
    public void Insert(T item)
    {
        // if the item is not contained in this quad, there's a problem
        if (!Contains(m_bounds, item.rectangle))
        {
            Trace.TraceWarning("feature is out of the bounds of this quadtree node");
            return;
        }

        // if the subnodes are null create them. may not be sucessfull: see below
        // we may be at the smallest allowed size in which case the subnodes will not be created
        if (m_nodes.Count == 0)
            CreateSubNodes();

        // for each subnode:
        // if the node contains the item, add the item to that node and return
        // this recurses into the node that is just large enough to fit this item
        foreach (QuadTreeNode<T> node in m_nodes)
        {
            if (Contains(node.Bounds, item.rectangle))
            {
                node.Insert(item);
                return;
            }
        }

        // if we make it to here, either
        // 1) none of the subnodes completely contained the item. or
        // 2) we're at the smallest subnode size allowed 
        // add the item to this node's contents.
        this.Contents.Add(item);
    }

    public void ForEach(QuadTree<T>.QTAction action)
    {
        action(this);

        // draw the child quads
        foreach (QuadTreeNode<T> node in this.m_nodes)
            node.ForEach(action);
    }

    public void Clear()
    {
        foreach (QuadTreeNode<T> node in this.m_nodes)
            node.Clear();
        m_contents.Clear();
        m_nodes.Clear();
    }

    /// <summary>
    /// Internal method to create the subnodes (partitions space)
    /// </summary>
    private void CreateSubNodes()
    {
        // the smallest subnode has an area 
        if ((m_bounds.height * m_bounds.width) <= 10)
            return;

        float halfWidth = (m_bounds.width / 2f);
        float halfHeight = (m_bounds.height / 2f);

        m_nodes.Add(new QuadTreeNode<T>(new Rect(m_bounds.xMin, m_bounds.yMin, halfWidth, halfHeight)));
        m_nodes.Add(new QuadTreeNode<T>(new Rect(m_bounds.xMin, m_bounds.yMin + halfHeight, halfWidth, halfHeight)));
        m_nodes.Add(new QuadTreeNode<T>(new Rect(m_bounds.xMin + halfWidth, m_bounds.yMin, halfWidth, halfHeight)));
        m_nodes.Add(new QuadTreeNode<T>(new Rect(m_bounds.xMin + halfWidth, m_bounds.yMin + halfHeight, halfWidth, halfHeight)));
    }

    public void DebugDraw()
    {
        UnityEngine.Debug.DrawLine(Bounds.position, Bounds.position + Vector2.right * Bounds.width);
        UnityEngine.Debug.DrawLine(Bounds.position + Vector2.right * Bounds.width, Bounds.position + Vector2.right * Bounds.width - Vector2.up * Bounds.height);
        UnityEngine.Debug.DrawLine(Bounds.position + Vector2.right * Bounds.width - Vector2.up * Bounds.height, Bounds.position - Vector2.up * Bounds.height);
        UnityEngine.Debug.DrawLine(Bounds.position - Vector2.up * Bounds.height, Bounds.position);
        m_nodes.ForEach(e => e.DebugDraw());
    }

    private static bool Contains(Rect container, Rect rect)
    {
        return container.Contains(rect.position) &&
        container.Contains(rect.position - rect.height * Vector2.up) &&
            container.Contains(rect.position + rect.width * Vector2.right) &&
            container.Contains(rect.position - rect.height * Vector2.up + rect.width * Vector2.right);
    }

}
