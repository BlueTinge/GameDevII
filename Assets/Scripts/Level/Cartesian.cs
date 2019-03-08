using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cartesian : ICoordSystem
{
    public Transform Origin;

    public Node[,,] Nodes;
    public Edge[,,,] Edges;

    public int XDim;
    public int YDim;
    public int ZDim;

    public Cartesian(Transform origin, int numX, int numY, int numZ, int xDim, int yDim, int zDim)
    {
        Origin = origin;
        Nodes = new Node[numX, numY, numZ];
        Edges = new Edge[numX + 1, numY + 1, numZ + 1, 3];
        XDim = xDim;
        YDim = yDim;
        ZDim = zDim;

        //make node grid
        for(int x = 0; x < numX; x++)
        {
            for(int y = 0; y < numY; y++)
            {
                for(int z = 0; z < numZ; z++)
                {
                    Nodes[x, y, z] = new Node(this, new Vector3(x,y,z));
                }
            }
        }

        //make edge grid
        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                for (int z = 0; z < numZ; z++)
                {
                    Edges[x, y, z, ToInt(Orientation.WEST_TO_EAST)] = new Edge(this, new Vector3(x-1, y, z), new Vector3(x, y, z));
                    Edges[x, y, z, ToInt(Orientation.SOUTH_TO_NORTH)] = new Edge(this, new Vector3(x, y, z), new Vector3(x, y, z-1));
                    Edges[x, y, z, ToInt(Orientation.DOWN_TO_UP)] = new Edge(this, new Vector3(x, y, z), new Vector3(x, y-1, z));
                }
            }
        }
    }

    public Direction[] GetLegalDirections()
    {
        return new Direction[] { Direction.NORTH, Direction.SOUTH, Direction.WEST, Direction.EAST, Direction.UP, Direction.DOWN };
    }

    public Orientation[] GetLegalOrientations()
    {
        return new Orientation[] { Orientation.WEST_TO_EAST, Orientation.SOUTH_TO_NORTH, Orientation.DOWN_TO_UP };
    }

    //Convert direction or orientation to int
    public int ToInt(Direction dir)
    {
        switch (dir)
        {
            case Direction.NORTH: return 0;
            case Direction.EAST: return 1;
            case Direction.SOUTH: return 2;
            case Direction.WEST: return 3;
            case Direction.UP: return 4;
            case Direction.DOWN: return 5;
        }
        return -1;
    }

    public int ToInt(Orientation o)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST: return 0;
            case Orientation.SOUTH_TO_NORTH: return 1;
            case Orientation.DOWN_TO_UP: return 2;
        }
        return -1;
    }

    public Direction ToDir(int d)
    {
        switch (d)
        {
            case 0: return Direction.NORTH;
            case 1: return Direction.EAST;
            case 2: return Direction.SOUTH;
            case 3: return Direction.WEST;
            case 4: return Direction.UP;
            case 5: return Direction.DOWN;
        }
        return Direction.MISCELLANEOUS;
    }

    public Orientation ToOr(int o)
    {
        switch (o)
        {
            case 0: return Orientation.WEST_TO_EAST;
            case 1: return Orientation.SOUTH_TO_NORTH;
            case 2: return Orientation.DOWN_TO_UP;
        }
        return Orientation.MISCELLANEOUS;
    }

    //Update node -- ensure coords points to node, and node is pointed to correct edges
    public void UpdateNode(Node node, Vector3 coords)
    {
        Nodes[(int)coords.x, (int)coords.y, (int)coords.z] = node;
        for(int d = 0; d < GetLegalDirections().Length; d++)
        {
            node.Edges[d] = GetEdge(coords, ToDir(d));
        }
        node.Coords = coords;

        Edges[(int)coords.x, (int)coords.y, (int)coords.z, ToInt(Orientation.WEST_TO_EAST)].Forward = node;
        Edges[(int)coords.x+1, (int)coords.y, (int)coords.z, ToInt(Orientation.WEST_TO_EAST)].Backward = node;
        Edges[(int)coords.x, (int)coords.y, (int)coords.z, ToInt(Orientation.DOWN_TO_UP)].Forward = node;
        Edges[(int)coords.x, (int)coords.y+1, (int)coords.z, ToInt(Orientation.DOWN_TO_UP)].Backward = node;
        Edges[(int)coords.x, (int)coords.y, (int)coords.z, ToInt(Orientation.SOUTH_TO_NORTH)].Forward = node;
        Edges[(int)coords.x, (int)coords.y, (int)coords.z+1, ToInt(Orientation.SOUTH_TO_NORTH)].Backward = node;
    }

    //Update edge -- ensure node1/node2 points to edge, edge links to node1/node2, and nodes1/2 link to edge
    public void UpdateEdge(Edge edge, Vector3 node1, Vector3 node2)
    {
        Orientation or = GetOrientation(node1, node2);
        Vector3 coords = GetForward(or, node1, node2);
        Vector3 backward_coords = GetBackward(or, node1, node2);

        Edges[(int)coords.x, (int)coords.y, (int)coords.z, ToInt(or)] = edge;
        edge.Forward = Nodes[(int)coords.x, (int)coords.y, (int)coords.z];
        edge.Backward = Nodes[(int)backward_coords.x, (int)backward_coords.y, (int)backward_coords.z];

        edge.node1 = backward_coords;
        edge.node2 = coords;

        Nodes[(int)coords.x, (int)coords.y, (int)coords.z].Edges[ToInt(GetBackwardDirection(or))] = edge;
        Nodes[(int)backward_coords.x, (int)backward_coords.y, (int)backward_coords.z].Edges[ToInt(GetForwardDirection(or))] = edge;
    }

    //Get the node at these co-ordinates
    public Node GetNode(Vector3 coords)
    {
        return Nodes[(int)coords.x, (int)coords.y, (int)coords.z];
    }

    //Get the edge between these two node co-ordinates
    public Edge GetEdge(Vector3 node1, Vector3 node2)
    {
        Orientation or = GetOrientation(node1, node2);
        Vector3 coords = GetForward(or, node1, node2);
        return Edges[(int)coords.x, (int)coords.y, (int)coords.z, ToInt(or)];
    }

    //Get the edge from this node in this direction
    public Edge GetEdge(Vector3 node1, Direction dir)
    {
        Orientation or = GetOrientation(dir);
        if (IsForward(or, dir))
        {
            Vector3 coords = Translate(node1, dir, 1f);
            return Edges[(int)coords.x, (int)coords.y, (int)coords.z, ToInt(or)];
        }
        else return Edges[(int)node1.x, (int)node1.y, (int)node1.z, ToInt(or)];
    }
    //Get the edge just left (relative to its orientation) of these two node co-ordinates
    public Edge GetEdgeLeftOf(Vector3 node1, Vector3 node2)
    {
        Orientation o = GetOrientation(node1, node2);
        return GetEdge(Translate(GetForward(o, node1, node2), GetLeftDirection(o), 1f), GetForwardDirection(o));
    }
    //Get the edge just right (relative to its orientation) of these two node co-ordinates
    public Edge GetEdgeRightOf(Vector3 node1, Vector3 node2)
    {
        Orientation o = GetOrientation(node1, node2);
        return GetEdge(Translate(GetForward(o, node1, node2), GetRightDirection(o), 1f), GetForwardDirection(o));
    }

    //Translate origin in dir by distance
    public Vector3 Translate(Vector3 origin, Direction dir, float distance)
    {
        switch (dir)
        {
            case Direction.NORTH: return new Vector3(origin.x, origin.y, origin.z+distance);
            case Direction.EAST: return new Vector3(origin.x+distance, origin.y, origin.z);
            case Direction.SOUTH: return new Vector3(origin.x, origin.y, origin.z-distance);
            case Direction.WEST: return new Vector3(origin.x-distance, origin.y, origin.z);
            case Direction.UP: return new Vector3(origin.x, origin.y+distance, origin.z);
            case Direction.DOWN: return new Vector3(origin.x, origin.y-distance, origin.z);
        }
        return new Vector3(origin.x, origin.y, origin.z);
    }

    //Get Unity co-ordinates of the lower-lefthand corner of the node at coords relative to this objects Transform. 
    public Vector3 GetUnityCoordinates(Vector3 coords)
    {
        return new Vector3(coords.x * XDim + Origin.localPosition.x, coords.y * YDim + Origin.localPosition.y, coords.z * ZDim + Origin.localPosition.z);
    }

    //Get orientation that corresponds to this direction
    public Orientation GetOrientation(Direction dir)
    {
        switch (dir)
        {
            case Direction.NORTH: return Orientation.SOUTH_TO_NORTH;
            case Direction.EAST: return Orientation.WEST_TO_EAST;
            case Direction.SOUTH: return Orientation.SOUTH_TO_NORTH;
            case Direction.WEST: return Orientation.WEST_TO_EAST;
            case Direction.UP: return Orientation.DOWN_TO_UP;
            case Direction.DOWN: return Orientation.DOWN_TO_UP;
        }
        return Orientation.MISCELLANEOUS;
    }
    //Get orientation corresponding to these two coordinate sets
    public Orientation GetOrientation(Vector3 node1, Vector3 node2)
    {
        if (node1.x != node2.x && node1.y == node2.y && node1.z == node2.z) return Orientation.WEST_TO_EAST;
        if (node1.x == node2.x && node1.y != node2.y && node1.z == node2.z) return Orientation.DOWN_TO_UP;
        if (node1.x == node2.x && node1.y == node2.y && node1.z != node2.z) return Orientation.SOUTH_TO_NORTH;
        return Orientation.MISCELLANEOUS;
    }
    //Get direction, from node1 to node2
    public Direction GetDirection(Vector3 node1, Vector3 node2)
    {
        if (node1.x != node2.x && node1.y == node2.y && node1.z == node2.z)
        {
            if (node1.x < node2.x) return Direction.EAST;
            else return Direction.WEST;
        }
        if (node1.x == node2.x && node1.y != node2.y && node1.z == node2.z)
        {
            if (node1.y < node2.y) return Direction.UP;
            else return Direction.DOWN;
        }
        if (node1.x == node2.x && node1.y == node2.y && node1.z != node2.z)
        {
            if (node1.z < node2.z) return Direction.NORTH;
            else return Direction.SOUTH;
        }
        return Direction.MISCELLANEOUS;
    }

    //Get forward/backward direction corresponding to o
    public Direction GetForwardDirection(Orientation o)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST: return Direction.EAST;
            case Orientation.SOUTH_TO_NORTH: return Direction.NORTH;
            case Orientation.DOWN_TO_UP: return Direction.UP;
        }
        return Direction.MISCELLANEOUS;
    }
    public Direction GetBackwardDirection(Orientation o)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST: return Direction.WEST;
            case Orientation.SOUTH_TO_NORTH: return Direction.SOUTH;
            case Orientation.DOWN_TO_UP: return Direction.DOWN;
        }
        return Direction.MISCELLANEOUS;
    }
    //Get left/right direction corresponding to o, if it exists.
    public Direction GetLeftDirection(Orientation o)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST: return Direction.NORTH;
            case Orientation.SOUTH_TO_NORTH: return Direction.WEST;
        }
        return Direction.MISCELLANEOUS;
    }
    public Direction GetRightDirection(Orientation o)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST: return Direction.SOUTH;
            case Orientation.SOUTH_TO_NORTH: return Direction.EAST;
        }
        return Direction.MISCELLANEOUS;
    }

    //Get direction immediately rotated in clockwise/counterclockwise according to normal vector o
    public Direction GetClockwise(Direction d, Direction n)
    {
        switch (n)
        {
            case Direction.DOWN:
                switch (d)
                {
                    case Direction.NORTH:
                        return Direction.EAST;
                    case Direction.EAST:
                        return Direction.SOUTH;
                    case Direction.SOUTH:
                        return Direction.WEST;
                    case Direction.WEST:
                        return Direction.NORTH;
                }
                break;
            case Direction.UP:
                switch (d)
                {
                    case Direction.NORTH:
                        return Direction.WEST;
                    case Direction.WEST:
                        return Direction.SOUTH;
                    case Direction.SOUTH:
                        return Direction.EAST;
                    case Direction.EAST:
                        return Direction.NORTH;
                }
                break;
            case Direction.EAST:
                switch (d)
                {
                    case Direction.NORTH:
                        return Direction.UP;
                    case Direction.UP:
                        return Direction.SOUTH;
                    case Direction.SOUTH:
                        return Direction.DOWN;
                    case Direction.DOWN:
                        return Direction.NORTH;
                }
                break;
            case Direction.WEST:
                switch (d)
                {
                    case Direction.NORTH:
                        return Direction.DOWN;
                    case Direction.DOWN:
                        return Direction.SOUTH;
                    case Direction.SOUTH:
                        return Direction.UP;
                    case Direction.UP:
                        return Direction.NORTH;
                }
                break;
            case Direction.NORTH:
                switch (d)
                {
                    case Direction.WEST:
                        return Direction.UP;
                    case Direction.UP:
                        return Direction.EAST;
                    case Direction.EAST:
                        return Direction.DOWN;
                    case Direction.DOWN:
                        return Direction.WEST;
                }
                break;
            case Direction.SOUTH:
                switch (d)
                {
                    case Direction.WEST:
                        return Direction.DOWN;
                    case Direction.DOWN:
                        return Direction.EAST;
                    case Direction.EAST:
                        return Direction.UP;
                    case Direction.UP:
                        return Direction.WEST;
                }
                break;
        }
        return Direction.MISCELLANEOUS;
    }
    public Direction GetWiddershins(Direction d, Direction n)
    {
        return GetClockwise(GetInverse(d), n);
    }

    //Get reversed direction in orientation
    public Direction GetInverse(Direction dir)
    {
        switch (dir)
        {
            case Direction.NORTH: return Direction.SOUTH;
            case Direction.EAST: return Direction.WEST;
            case Direction.SOUTH: return Direction.NORTH;
            case Direction.WEST: return Direction.EAST;
            case Direction.UP: return Direction.DOWN;
            case Direction.DOWN: return Direction.UP;
        }
        return Direction.MISCELLANEOUS;
    }

    //Given two Vector3s, returns the one that is forward or backward according to o
    public Vector3 GetForward(Orientation o, Vector3 node1, Vector3 node2)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST:
                if (node1.x > node2.x) return node1;
                else return node2;
            case Orientation.SOUTH_TO_NORTH:
                if (node1.z > node2.z) return node1;
                else return node2;
            case Orientation.DOWN_TO_UP:
                if (node1.y > node2.y) return node1;
                else return node2;
        }
        float xdist = node2.x - node1.x;
        float ydist = node2.y - node1.y;
        float zdist = node2.z - node1.z;

        if(Mathf.Abs(xdist) > Mathf.Abs(ydist) && Mathf.Abs(xdist) > Mathf.Abs(zdist))
        {
            if (xdist > 0) return node2;
            else return node1;
        }
        else if (Mathf.Abs(ydist) > Mathf.Abs(xdist) && Mathf.Abs(ydist) > Mathf.Abs(zdist))
        {
            if (ydist > 0) return node2;
            else return node1;
        }
        else 
        {
            if (zdist > 0) return node2;
            else return node1;
        }

    }
    public Vector3 GetBackward(Orientation o, Vector3 node1, Vector3 node2)
    {
        if (GetForward(o, node1, node2).Equals(node1)) return node2;
        else return node1;
    }

    //Returns TRUE if this is a correct dir/orientation correspondence, false otherwise.
    public bool IsForward(Orientation o, Direction dir)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST:
                if (dir.Equals(Direction.EAST)) return true;
                else return false;
            case Orientation.SOUTH_TO_NORTH:
                if (dir.Equals(Direction.NORTH)) return true;
                else return false;
            case Orientation.DOWN_TO_UP:
                if (dir.Equals(Direction.UP)) return true;
                else return false;
        }
        return false;
    }
    public bool IsBackward(Orientation o, Direction dir)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST:
                if (dir.Equals(Direction.WEST)) return true;
                else return false;
            case Orientation.SOUTH_TO_NORTH:
                if (dir.Equals(Direction.SOUTH)) return true;
                else return false;
            case Orientation.DOWN_TO_UP:
                if (dir.Equals(Direction.DOWN)) return true;
                else return false;
        }
        return false;
    }
    public bool IsLeft(Orientation o, Direction dir)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST:
                if (dir.Equals(Direction.NORTH)) return true;
                else return false;
            case Orientation.SOUTH_TO_NORTH:
                if (dir.Equals(Direction.WEST)) return true;
                else return false;
        }
        return false;
    }
    public bool IsRight(Orientation o, Direction dir)
    {
        switch (o)
        {
            case Orientation.WEST_TO_EAST:
                if (dir.Equals(Direction.SOUTH)) return true;
                else return false;
            case Orientation.SOUTH_TO_NORTH:
                if (dir.Equals(Direction.EAST)) return true;
                else return false;
        }
        return false;
    }
}
