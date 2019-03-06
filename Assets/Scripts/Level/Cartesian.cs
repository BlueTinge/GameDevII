using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cartesian : ICoordSystem
{
    public Transform Origin;

    public Node<Cartesian>[,,] Grid;

    public Cartesian(Transform origin, int numX, int numY, int numZ)
    {
        Origin = origin;
        Grid = new Node<Cartesian>[numX, numY, numZ];

        //make node grid
        for(int x = 0; x < numX; x++)
        {
            for(int y = 0; y < numY; y++)
            {
                for(int z = 0; z < numZ; z++)
                {
                    //Grid[x, y, z] = new Node<Cartesian>();
                }
            }
        }
    }

    public Direction[] GetLegalDirections()
    {
        return new Direction[] { Direction.NORTH, Direction.SOUTH, Direction.WEST, Direction.EAST, Direction.UP, Direction.DOWN };
    }

    public void AssignNode<C>(Node<C> node, Vector3 coords) where C : ICoordSystem
    {
        throw new System.NotImplementedException();
    }

    public void AssignEdge<C>(Edge<C> edge, Vector3 node1, Vector3 node2) where C : ICoordSystem
    {
        throw new System.NotImplementedException();
    }

    public Orientation GetOrientation(Vector3 node1, Vector3 node2)
    {
        return Orientation.MISCELLANEOUS;
    }

    public Node<C> GetNode<C>(Vector3 coords) where C : ICoordSystem
    {
        throw new System.NotImplementedException();
    }

    public Edge<C> GetEdge<C>(Vector3 node1, Vector3 node2) where C : ICoordSystem
    {
        throw new System.NotImplementedException();
    }

    public Edge<C> GetEdgeLeftOf<C>(Vector3 node1, Vector3 node2) where C : ICoordSystem
    {
        throw new System.NotImplementedException();
    }

    public Edge<C> GetEdgeRightOf<C>(Vector3 node1, Vector3 node2) where C : ICoordSystem
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetForward(Orientation o, Vector3 node1, Vector3 node2)
    {
        throw new System.NotImplementedException();
    }

    public Vector3 GetBackward(Orientation o, Vector3 node1, Vector3 node2)
    {
        throw new System.NotImplementedException();
    }

    public bool IsForward(Orientation o, Direction dir)
    {
        throw new System.NotImplementedException();
    }

    public bool IsBackward(Orientation o, Direction dir)
    {
        throw new System.NotImplementedException();
    }

    public bool IsLeft(Orientation o, Direction dir)
    {
        throw new System.NotImplementedException();
    }

    public bool IsRight(Orientation o, Direction dir)
    {
        throw new System.NotImplementedException();
    }
}
