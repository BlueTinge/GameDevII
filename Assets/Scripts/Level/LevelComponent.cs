using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelComponent
{
    public bool IsTranslated { get; set; }

    /**
     *  Translation is the act of turning this LevelComponent into an actual level object in the world.
     *  Be sure to set IsTranslated immediately when overriding this method, to avoid infinite loops.
     */
    public abstract void Translate();
}

public abstract class NodeComponent : LevelComponent
{
    public Node Parent { get; set; }
}

public abstract class EdgeComponent : LevelComponent
{
    public Edge Parent { get; set; }

    public abstract bool IsOrientationLegal(Orientation o);

    public abstract bool IsPassable(Direction dir, List<int> keys);

    public abstract bool IsVisible(Direction dir);
}
