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
    public static readonly string[] LEVEL_ORDER = { "1st Level", "2nd Level", "Boss Room"};
    public static readonly int INITIAL_POTION_NUM = 3;

    private static Dictionary<string, bool> Checkpoints = null;
    private static HashSet<Vector3>[] DeadEnemies = null;
    private static int NumPotions;
    private static float PlayerHealth;
    private static bool IsInitialized = false;
    private static GameObject Weapon = null;

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
        DeadEnemies = new HashSet<Vector3>[LEVEL_ORDER.Length];
        for(int i = 0; i < LEVEL_ORDER.Length; i++)
        {
            DeadEnemies[i] = new HashSet<Vector3>();
        }
        NumPotions = INITIAL_POTION_NUM;
        PlayerHealth = 0;
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

    //load correct level assuming we died on current level
    public static void LoadFromCheckpoint()
    {
        if (!IsInitialized)
        {
            Reset();
        }

        NumPotions = INITIAL_POTION_NUM;
        PlayerHealth = -1;

        foreach (string SceneName in LEVEL_ORDER)
        {
            if (GetCheckpoint(SceneName) && !SceneName.Equals(SceneManager.GetActiveScene().name)) continue;
            else
            {
                if(SceneName.Equals(LEVEL_ORDER[0])) Destroy(Weapon);
                Time.timeScale = 1f;
                if (!GetCheckpoint(SceneName))
                {
                    for (int i = 0; i < LEVEL_ORDER.Length; i++)
                    {
                        if (LEVEL_ORDER[i].Equals(SceneName))
                        {
                            DeadEnemies[i].Clear();
                            break;
                        }
                    }
                }
                SceneManager.LoadScene(SceneName);
                break;
            }
        }
    }

    public static void SaveValues(PlayerController player)
    {
        if (!IsInitialized)
        {
            Reset();
        }

        NumPotions = player.NumPotions;
        PlayerHealth = player.gameObject.GetComponent<HealthStats>().CurrentHealth;
        //theoretically weapons can be saved as well
        Weapon = Instantiate(player.GetComponent<Equipment>().CurrentWeapon,new Vector3(-1000,-100000,-10000),Quaternion.identity,null);

        DontDestroyOnLoad(Weapon);
    }

    internal static float GetPlayerHealth()
    {
        if (!IsInitialized)
        {
            Reset();
        }

        return PlayerHealth;
    }

    internal static int GetNumPotions()
    {
        if (!IsInitialized)
        {
            Reset();
        }

        //when you begin the game, you have INITIAL_POTION_NUM.
        //when you go to a new level, you have however many potions you had before.
        //if you die without hitting any checkpoints, you have INITIAL_POTION_NUM again.
        //what if you did hit a checkpoint? do your potions come back on the 2nd level? I think so

        return NumPotions;
    }

    internal static GameObject GetWeapon()
    {
        if (!IsInitialized)
        {
            Reset();
        }
        return Weapon;
    }

    public static void RecordDeath(Vector3 position)
    {
        if (!IsInitialized)
        {
            Reset();
        }

        for (int i = 0; i < LEVEL_ORDER.Length; i++)
        {
            if (LEVEL_ORDER[i].Equals(SceneManager.GetActiveScene().name))
            {
                DeadEnemies[i].Add(position);
                break;
            }
        }
    }

    public static bool IsEnemyDead(Vector3 position)
    {
        for (int i = 0; i < LEVEL_ORDER.Length; i++)
        {
            if (LEVEL_ORDER[i].Equals(SceneManager.GetActiveScene().name))
            {
                if (GetCheckpoint(LEVEL_ORDER[i]))
                {
                    return DeadEnemies[i].Contains(position);
                }
                else return false;
            }
        }

        return false;
    }
}
