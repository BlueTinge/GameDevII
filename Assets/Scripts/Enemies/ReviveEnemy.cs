using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveEnemy : MonoBehaviour
{
    private Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        if (Manager.IsEnemyDead(pos)) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Manager.RecordDeath(pos);
    }
}
