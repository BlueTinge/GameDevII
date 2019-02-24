using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I hate the fact that this script needs to exist.
//All it does is delegate animation events to the PlayerController.
//Ideally, the Animator would be on the Player root gameobject instead of the player model,
//but moving it there would be such an extreme pain in the ass that I really, really can't be bothered.

public class PlayerAnimationEventProxy : MonoBehaviour
{
    public PlayerController player;

    public void MakeLightAttack(float ttl)
    {
        StartCoroutine(player.MakeLightAttack(ttl));
    }

    public void MakeHeavyAttack(float ttl)
    {
        StartCoroutine(player.MakeHeavyAttack(ttl));
    }

}
