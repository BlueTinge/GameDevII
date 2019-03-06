using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelComponent
{
    void Translate();
}

public interface IEdgeComponent : ILevelComponent
{
    bool IsOrientationLegal(Orientation o);
}
