using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a "zone" of a level, whether that is a room, corridor, or designated space
//Adjacent to other zones via edges

public abstract class Node<C> : ILevelComponent where C : ICoordSystem
{
    public C cs { get; protected set; }
    public Vector3 Coords { get; protected set; }
    public Edge<C>[] Edges; //invariant: none of these are null (once they have been assigned)
    public List<ILevelComponent> Components { get; protected set; }

    public Node(C _coordSystem, Vector3 coords)
    {
        cs = _coordSystem;
        cs.AssignNode<C>(this, coords);
        Edges = new Edge<C>[cs.GetLegalDirections().Length];
        Components = new List<ILevelComponent>();
    }

    public List<Node<C>> GetAdjNodes()
    {
        List<Node<C>> ls = new List<Node<C>>();
        foreach (Direction d in cs.GetLegalDirections())
        {
            if(Edges[(int)d].GetNode(d) != null)
            {
                ls.Add(Edges[(int)d].GetNode(d));
            }
        }
        return ls;
    }

    public List<Node<C>> GetPassableNodes()
    {
        return GetPassableNodes(new List<int>());
    }

    public List<Node<C>> GetPassableNodes(List<int> keys)
    {
        List<Node<C>> ls = new List<Node<C>>();
        foreach (Direction d in cs.GetLegalDirections())
        {
            if (Edges[(int)d].GetNode(d) != null && Edges[(int)d].IsPassable(d, keys))
            {
                ls.Add(Edges[(int)d].GetNode(d));
            }
        }
        return ls;
    }

    public List<Edge<C>> GetAdjEdges()
    {
        return new List<Edge<C>>(Edges);
    }

    public Node<C> GetNode(Direction dir)
    {
        foreach (Direction d in cs.GetLegalDirections()){
            if (d == dir)
            {
                //is valid direction
                return Edges[(int)dir].GetNode(dir);
            }
        }
        return null;
    }

    public Edge<C> GetEdge(Direction dir)
    {
        foreach (Direction d in cs.GetLegalDirections())
        {
            if (d == dir)
            {
                //is valid direction
                return Edges[(int)dir];
            }
        }
        return null;
    }

    public bool AddComponent(ILevelComponent c)
    {
        Components.Add(c);
        return true;
    }

    public List<ILevelComponent> GetComponents()
    {
        return Components;
    }

    public abstract void Translate();
}
