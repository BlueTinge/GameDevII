using System;
using System.Collections.Generic;
using UnityEngine;

//A basic wall. Add to an edge to create a wall in the world when it is translated.

public class Wall : EdgeComponent
{
    public WallObject WallObject;
    public float Thickness;

    public Wall(WallObject wallObject, float thickness)
    {
        WallObject = wallObject;
        Thickness = thickness;
    }

    public override bool IsOrientationLegal(Orientation o)
    {
        if (o == Orientation.WEST_TO_EAST || o == Orientation.SOUTH_TO_NORTH) return true;
        else return false;
    }

    public override bool IsPassable(Direction dir, List<int> keys)
    {
        return false;
    }

    public override bool IsVisible(Direction dir)
    {
        return false;
    }

    public override void Translate()
    {
        IsTranslated = true;

        Direction forwardDir = Parent.CS.GetForwardDirection(Parent.Or);
        Direction backwardDir = Parent.CS.GetBackwardDirection(Parent.Or);
        Direction leftDir = Parent.CS.GetLeftDirection(Parent.Or);
        Direction rightDir = Parent.CS.GetRightDirection(Parent.Or);

        //get vertexes in world space
        Vector3[] Coords = Parent.GetUnityCoords();
        Vector3 LowerLeftIn = Parent.CS.Translate(Coords[0], backwardDir, Thickness / 2);
        Vector3 UpperLeftIn = Parent.CS.Translate(Coords[1], backwardDir, Thickness / 2);
        Vector3 UpperRightIn = Parent.CS.Translate(Coords[2], backwardDir, Thickness / 2);
        Vector3 LowerRightIn = Parent.CS.Translate(Coords[3], backwardDir, Thickness / 2);
        Vector3 LowerLeftOut = Parent.CS.Translate(Coords[0], forwardDir, Thickness / 2);
        Vector3 UpperLeftOut = Parent.CS.Translate(Coords[1], forwardDir, Thickness / 2);
        Vector3 UpperRightOut = Parent.CS.Translate(Coords[2], forwardDir, Thickness / 2);
        Vector3 LowerRightOut = Parent.CS.Translate(Coords[3], forwardDir, Thickness / 2);

        //find intersecting edges
        //left side first
        Tuple<Direction,Edge>[] leftEdges = Parent.GetEdgesAt(leftDir);
        Wall left = null;
        Wall straight = null;
        Wall right = null;
        bool leftIsStraightEdge = false;

        foreach (Tuple<Direction, Edge> t in leftEdges)
        {
            if (t.Item1 == backwardDir) left = t.Item2.Wall;
            else if (t.Item1 == leftDir) straight = t.Item2.Wall;
            else if (t.Item1 == forwardDir) right = t.Item2.Wall;
        }

        if(left != null)
        {
            //inside corner
            LowerLeftIn = Parent.CS.Translate(LowerLeftIn, rightDir, left.Thickness / 2);
            UpperLeftIn = Parent.CS.Translate(UpperLeftIn, rightDir, left.Thickness / 2);

            if (right == null && straight == null)
            {
                //outside corner
                LowerLeftOut = Parent.CS.Translate(LowerLeftOut, leftDir, left.Thickness / 2);
                UpperLeftOut = Parent.CS.Translate(UpperLeftOut, leftDir, left.Thickness / 2);
            }
        }
        if(right != null)
        {
            //inside corner
            LowerLeftOut = Parent.CS.Translate(LowerLeftOut, rightDir, right.Thickness / 2);
            UpperLeftOut = Parent.CS.Translate(UpperLeftOut, rightDir, right.Thickness / 2);

            if (left == null && straight == null)
            {
                //outside corner
                LowerLeftIn = Parent.CS.Translate(LowerLeftIn, leftDir, right.Thickness / 2);
                UpperLeftIn = Parent.CS.Translate(UpperLeftIn, leftDir, right.Thickness / 2);
            }
        }
        if(left == null && right == null && straight == null)
        {
            //straight edge
            leftIsStraightEdge = true;
        }


        //right side last
        Tuple<Direction, Edge>[] rightEdges = Parent.GetEdgesAt(rightDir);
        left = null;
        straight = null;
        right = null;
        bool rightIsStraightEdge = false;

        foreach (Tuple<Direction, Edge> t in rightEdges)
        {
            if (t.Item1 == forwardDir) left = t.Item2.Wall;
            else if (t.Item1 == rightDir) straight = t.Item2.Wall;
            else if (t.Item1 == backwardDir) right = t.Item2.Wall;
        }

        if (left != null)
        {
            //inside corner
            LowerRightOut = Parent.CS.Translate(LowerRightOut, leftDir, left.Thickness / 2);
            UpperRightOut = Parent.CS.Translate(UpperRightOut, leftDir, left.Thickness / 2);

            if (right == null && straight == null)
            {
                //outside corner
                LowerRightIn = Parent.CS.Translate(LowerRightIn, rightDir, left.Thickness / 2);
                UpperRightIn = Parent.CS.Translate(UpperRightIn, rightDir, left.Thickness / 2);
            }
        }
        if (right != null)
        {
            //inside corner
            LowerRightIn = Parent.CS.Translate(LowerRightIn, leftDir, right.Thickness / 2);
            UpperRightIn = Parent.CS.Translate(UpperRightIn, leftDir, right.Thickness / 2);

            if (left == null && straight == null)
            {
                //outside corner
                LowerRightOut = Parent.CS.Translate(LowerRightOut, rightDir, right.Thickness / 2);
                UpperRightOut = Parent.CS.Translate(UpperRightOut, rightDir, right.Thickness / 2);
            }
        }
        if (left == null && right == null && straight == null)
        {
            //straight edge
            rightIsStraightEdge = true;
        }

        //now actually assign the damn triangles
        AddQuad(new Vector3[] { LowerLeftIn, UpperLeftIn, LowerRightIn, UpperRightIn });
        AddQuad(new Vector3[] { LowerRightOut, UpperRightOut, LowerLeftOut, UpperLeftOut });
        if(leftIsStraightEdge) AddQuad(new Vector3[] { LowerLeftOut, UpperLeftOut, LowerLeftIn, UpperLeftIn });
        if(rightIsStraightEdge) AddQuad(new Vector3[] { LowerRightIn, UpperRightIn, LowerRightOut, UpperRightOut });
    }

    //1 3
    //0 2
    public void AddQuad(Vector3[] coords)
    {
        WallObject.AddTriangle(coords[0],coords[1],coords[2]);
        WallObject.AddTriangle(coords[2], coords[1], coords[3]);
    }
}
