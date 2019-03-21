using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ICoordSystems are used to allow for nodes to be placed in different configurations (e.g. grid pattern, cylindrical, hexagonal, etc.)

//#1 rule of programming is KISS (Keep It Simple Stupid)
//Every part of the procedural level gen system, but especially this class, flagrantly violates this rule.
//I'm sorry.

//All directions are in here, legal directions are specified per system
public enum Direction {NORTH, SOUTH, WEST, EAST, UP, DOWN, MISCELLANEOUS};
public enum Orientation {MISCELLANEOUS, SOUTH_TO_NORTH, WEST_TO_EAST, DOWN_TO_UP};

public interface ICoordSystem
{
    //Return array of all directions or orientations understood by this coordinate system, in no particular order
    Direction[] GetLegalDirections();
    Orientation[] GetLegalOrientations();
    //Convert direction or orientation to int, and vice versa
    int ToInt(Direction d);
    int ToInt(Orientation d);
    Direction ToDir(int d);
    Orientation ToOr(int o);

    //Update node -- ensure coords points to node, and node is pointed to correct edges, and edges point to correct node
    void UpdateNode(Node node, Vector3 coords);
    //Update edge -- ensure node1/node2 points to edge, edge links to node1/node2, and nodes1/2 link to edge
    void UpdateEdge(Edge edge, Vector3 node1, Vector3 node2);

    //Get the node at these co-ordinates
    Node GetNode(Vector3 coords);
    //Get the edge between these two node co-ordinates
    Edge GetEdge(Vector3 node1, Vector3 node2);
    //Get the edge from this node in this direction
    Edge GetEdge(Vector3 node1, Direction d);
    //Get the edge just left (relative to its orientation) of these two node co-ordinates
    Edge GetEdgeLeftOf(Vector3 node1, Vector3 node2);
    //Get the edge just right (relative to its orientation) of these two node co-ordinates
    Edge GetEdgeRightOf(Vector3 node1, Vector3 node2);
    //Return direction-edge pairs, in clockwise order, intersecting with this edge at dir
    Tuple<Direction, Edge>[] GetEdgesAt(Vector3 node1, Vector3 node2, Direction dir);

    /// <summary>
    /// True if the node co-ordinates point to a valid node, false otherwise
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    bool IsValidNode(Vector3 node);
    bool IsValidEdge(Vector3 node1, Vector3 node2);

    //Translate origin in dir by distance
    Vector3 Translate(Vector3 origin, Direction dir, float distance);

    //Get Unity co-ordinates of the lower-lefthand corner of the node at coords relative to this objects Transform. 
    Vector3 GetUnityCoordinates(Vector3 coords);

    //Get orientation that corresponds to this direction
    Orientation GetOrientation(Direction d);
    //Get orientation or direction corresponding to these two coordinate sets
    Orientation GetOrientation(Vector3 node1, Vector3 node2);
    Direction GetDirection(Vector3 node1, Vector3 node2);

    //Get forward/backward direction corresponding to o
    Direction GetForwardDirection(Orientation o);
    Direction GetBackwardDirection(Orientation o);
    //Get left/right direction corresponding to o, if it exists
    Direction GetLeftDirection(Orientation o);
    Direction GetRightDirection(Orientation o);

    //Get direction immediately rotated in clockwise/counterclockwise according to normal vector o
    Direction GetClockwise(Direction d, Direction n);
    Direction GetWiddershins(Direction d, Direction n);

    //Get reversed direction in orientation
    Direction GetInverse(Direction d);

    //Given two Vector3s, returns the one that is forward or backward according to o
    Vector3 GetForward(Orientation o, Vector3 node1, Vector3 node2);
    Vector3 GetBackward(Orientation o, Vector3 node1, Vector3 node2);

    //Returns TRUE if this is a correct dir/orientation correspondence, false otherwise.
    bool IsForward(Orientation o, Direction dir);
    bool IsBackward(Orientation o, Direction dir);
    bool IsLeft(Orientation o, Direction dir);
    bool IsRight(Orientation o, Direction dir);
}
