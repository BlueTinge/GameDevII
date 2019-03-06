using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ICoordSystems are used to allow for nodes to be placed in different configurations (e.g. grid pattern, cylindrical, hexagonal, etc.)

//#1 rule of programming is KISS (Keep It Simple Stupid)
//Every part of the procedural level gen system, but especially this class, flagrantly violates this rule.
//I'm sorry.

//All directions are in here, legal directions are specified per system
public enum Direction {NORTH, SOUTH, WEST, EAST, UP, DOWN};
public enum Orientation {MISCELLANEOUS, NORTH_TO_SOUTH, WEST_TO_EAST, DOWN_TO_UP};

public interface ICoordSystem
{
    Direction[] GetLegalDirections();

    void AssignNode<C>(Node<C> node, Vector3 coords) where C : ICoordSystem;
    void AssignEdge<C>(Edge<C> edge, Vector3 node1, Vector3 node2) where C : ICoordSystem;

    Node<C> GetNode<C>(Vector3 coords) where C: ICoordSystem;
    Edge<C> GetEdge<C>(Vector3 node1, Vector3 node2) where C : ICoordSystem;
    Edge<C> GetEdgeLeftOf<C>(Vector3 node1, Vector3 node2) where C : ICoordSystem;
    Edge<C> GetEdgeRightOf<C>(Vector3 node1, Vector3 node2) where C : ICoordSystem;

    Orientation GetOrientation(Vector3 node1, Vector3 node2);
    Vector3 GetForward(Orientation o, Vector3 node1, Vector3 node2);
    Vector3 GetBackward(Orientation o, Vector3 node1, Vector3 node2);
    bool IsForward(Orientation o, Direction dir);
    bool IsBackward(Orientation o, Direction dir);
    bool IsLeft(Orientation o, Direction dir);
    bool IsRight(Orientation o, Direction dir);
}
