using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatable : MonoBehaviour
{
    public abstract void Activate();
    public abstract void Deactivate();

    public abstract bool GetIsActivated();
}
