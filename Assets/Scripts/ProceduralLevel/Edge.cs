using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a wall, passage, or other edge between nodes or "zones"

public class Edge : LevelComponent
{
    public Orientation Or { get; set; }

    //any of these can be null
    public Node Forward { get; set; }
    public Node Backward { get; set; }
    //relative coords
    public Vector3 node1;
    public Vector3 node2;

    //wall: edge should only have one wall, if any.
    public Wall Wall { get; set; }

    public ICoordSystem CS { get; protected set; }
    public List<EdgeComponent> Components { get; set; }

    public Edge(ICoordSystem _coordSystem, Vector3 node1, Vector3 node2)
    {
        CS = _coordSystem;
        CS.UpdateEdge(this, node1, node2);
        Or = CS.GetOrientation(node1, node2);

        if(CS.GetForward(Or, node1, node2).Equals(node1)) Forward = CS.GetNode(node1);
        else if(CS.GetForward(Or, node1, node2).Equals(node2)) Forward = CS.GetNode(node2);
        if (CS.GetBackward(Or, node1, node2).Equals(node1)) Backward = CS.GetNode(node1);
        else if (CS.GetBackward(Or, node1, node2).Equals(node2)) Backward = CS.GetNode(node2);

        Components = new List<EdgeComponent>();
    }

    //Return array of unity coords clockwise from bottom-left corner
    public Vector3[] GetUnityCoords()
    {
        Direction d = CS.GetLeftDirection(Or);
        List<Vector3> ls = new List<Vector3>();
        ls.Add(CS.GetUnityCoordinates(CS.Translate(node2, d, 1f)));

        while (d != CS.GetWiddershins(CS.GetLeftDirection(Or), CS.GetForwardDirection(Or)))
        {
            d = CS.GetClockwise(d, CS.GetForwardDirection(Or));
            ls.Add(CS.GetUnityCoordinates(CS.Translate(ls[ls.Count-1], d, 1f)));
        }

        return ls.ToArray();
    }

    public Node GetNode(Direction dir)
    {
        if (CS.IsForward(Or, dir)) return Forward;
        else if (CS.IsBackward(Or, dir)) return Backward;

        return null;
    }

    public Edge GetEdge(Direction dir)
    {
        if (CS.IsLeft(Or, dir)) return CS.GetEdgeLeftOf(node1,node2);
        else if (CS.IsRight(Or, dir)) return CS.GetEdgeRightOf(node1, node2);

        return null;
    }

    //Return direction-edge pairs, in clockwise order, intersecting with this edge at dir
    public Tuple<Direction, Edge>[]GetEdgesAt(Direction dir)
    {
        return CS.GetEdgesAt(node1,node2,dir);
    }

    public bool IsPassable(Direction dir, List<int> keys)
    {
        foreach (EdgeComponent c in Components)
        {
            if(c.IsPassable(dir,keys)) return false;
        }
        return true;
    }

    public bool IsVisible(Direction dir)
    {
        foreach (EdgeComponent c in Components)
        {
            if (c.IsVisible(dir)) return false;
        }
        return true;
    }

    override public void Translate()
    {
        IsTranslated = true;
        foreach (EdgeComponent c in Components)
        {
            if(!c.IsTranslated) c.Translate();
        }
    }

    public bool AddComponent(EdgeComponent c)
    {
        if (c.IsOrientationLegal(Or))
        {
            if (c as Wall != null)
            {
                if (Wall == null) Wall = c as Wall;
                else Debug.Log("WARNING: More than one Wall assigned to an Edge.");
            }

            c.Parent = this;
            Components.Add(c);
            return true;
        }
        else return false;
    }
}
