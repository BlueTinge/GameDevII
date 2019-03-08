using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Represents a "zone" of a level, whether that is a room, corridor, or designated space
//Adjacent to other zones via edges
//Nodes are intended to be "holders" of other LevelComponents

public class Node : LevelComponent
{
    public ICoordSystem CS { get; protected set; }
    public Vector3 Coords { get; set; }
    public Edge[] Edges; //invariant: none of these are null (once they have been assigned)
    public List<LevelComponent> Components { get; protected set; }

    public Node(ICoordSystem _coordSystem, Vector3 coords)
    {
        CS = _coordSystem;
        Edges = new Edge[CS.GetLegalDirections().Length];
        Components = new List<LevelComponent>();
    }

    public List<Node> GetAdjNodes()
    {
        List<Node> ls = new List<Node>();
        foreach (Direction d in CS.GetLegalDirections())
        {
            if(Edges[CS.ToInt(d)].GetNode(d) != null)
            {
                ls.Add(Edges[CS.ToInt(d)].GetNode(d));
            }
        }
        return ls;
    }

    public List<Node> GetPassableNodes()
    {
        return GetPassableNodes(new List<int>());
    }

    public List<Node> GetPassableNodes(List<int> keys)
    {
        List<Node> ls = new List<Node>();
        foreach (Direction d in CS.GetLegalDirections())
        {
            if (Edges[CS.ToInt(d)].GetNode(d) != null && Edges[CS.ToInt(d)].IsPassable(d, keys))
            {
                ls.Add(Edges[CS.ToInt(d)].GetNode(d));
            }
        }
        return ls;
    }

    public List<Edge> GetAdjEdges()
    {
        return new List<Edge>(Edges);
    }

    public Node GetNode(Direction dir)
    {
        foreach (Direction d in CS.GetLegalDirections()){
            if (d == dir)
            {
                //is valid direction
                return Edges[CS.ToInt(dir)].GetNode(dir);
            }
        }
        return null;
    }

    public Edge GetEdge(Direction dir)
    {
        foreach (Direction d in CS.GetLegalDirections())
        {
            if (d == dir)
            {
                //is valid direction
                return Edges[CS.ToInt(dir)];
            }
        }
        return null;
    }

    public bool AddComponent(LevelComponent c)
    {
        c.Parent = this;
        Components.Add(c);
        return true;
    }

    public List<LevelComponent> GetComponents()
    {
        return Components;
    }

    public override void Translate()
    {
        IsTranslated = true;

        foreach (LevelComponent c in Components)
        {
            if (!c.IsTranslated) c.Translate();
        }
    }
}
