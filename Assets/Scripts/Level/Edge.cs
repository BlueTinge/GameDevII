using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a wall, passage, or other edge between nodes or "zones"

public abstract class Edge<C> : ILevelComponent where C : ICoordSystem
{
    public Orientation Or { get; protected set; }

    //any of these can be null
    public Node<C> Forward { get; protected set; }
    public Node<C> Backward { get; protected set; }
    public Edge<C> Left { get; protected set; }
    public Edge<C> Right { get; protected set; }

    public C cs { get; protected set; }
    public List<IEdgeComponent> Components { get; protected set; }

    public Edge(C _coordSystem, Vector3 node1, Vector3 node2)
    {
        cs = _coordSystem;
        cs.AssignEdge<C>(this, node1, node2);
        Or = cs.GetOrientation(node1, node2);

        if(cs.GetForward(Or, node1, node2).Equals(node1)) Forward = cs.GetNode<C>(node1);
        else if(cs.GetForward(Or, node1, node2).Equals(node2)) Forward = cs.GetNode<C>(node2);
        if (cs.GetBackward(Or, node1, node2).Equals(node1)) Backward = cs.GetNode<C>(node1);
        else if (cs.GetBackward(Or, node1, node2).Equals(node2)) Backward = cs.GetNode<C>(node2);

        Left = cs.GetEdgeLeftOf<C>(node1, node2);
        Right = cs.GetEdgeRightOf<C>(node1, node2);

        Components = new List<IEdgeComponent>();
    }

    public Node<C> GetNode(Direction dir)
    {
        if (cs.IsForward(Or, dir)) return Forward;
        else if (cs.IsBackward(Or, dir)) return Backward;

        return null;
    }

    public Edge<C> GetEdge(Direction dir)
    {
        if (cs.IsLeft(Or, dir)) return Left;
        else if (cs.IsRight(Or, dir)) return Right;

        return null;
    }

    public abstract bool IsPassable(Direction dir);

    public bool IsPassable(Direction dir, List<int> keys)
    {
        return IsPassable(dir);
    }

    public abstract bool IsVisible(Direction dir);

    abstract public void Translate();

    public bool AddComponent(IEdgeComponent c)
    {
        if (c.IsOrientationLegal(Or))
        {
            Components.Add(c);
            return true;
        }
        else return false;
    }
}
