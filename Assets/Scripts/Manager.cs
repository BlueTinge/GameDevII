using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Global variable manager
//Not actually a singleton, just uses static variables
//Also doesnt need to be attached to anything

public class Manager : MonoBehaviour
{


    private static Dictionary<string, bool> Checkpoints = null;
    private static bool IsInitialized = false;

    void Awake()
    {
        if (!IsInitialized)
        {
            Reset();
        }
    }

    //Reset all values to default
     public static void Reset()
     {
        Checkpoints = new Dictionary<string, bool>();

        IsInitialized = true;
     }

    //Edit this checkpoint value
    public static void SetCheckpoint(string sceneName, bool isCheckpointSet)
    {
        if (!IsInitialized)
        {
            Reset();
        }

        Checkpoints.Remove(sceneName);
        Checkpoints.Add(sceneName, isCheckpointSet);
    }

    //Retrieve this checkpoint value, false if checkpoint not set
    public static bool GetCheckpoint(string sceneName)
    {
        if (!IsInitialized)
        {
            Reset();
        }

        if (Checkpoints.TryGetValue(sceneName, out bool isCheckpointSet)){
            return isCheckpointSet;
        }
        return false;
    }

}
