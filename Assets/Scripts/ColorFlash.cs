using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ColorFlash : MonoBehaviour
{
    [SerializeField]private Color[] colors;
    [SerializeField]private float switchDelay = 1/60.0f;
    private int index;
    private MeshRenderer mr;
    private float switchTime;
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        index = 0;
        switchTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - switchTime > switchDelay)
        {
            switchTime = Time.time;
            for(int i = 0; i < mr.materials.Length; ++i)
            {
                mr.materials[i].color = colors[index];
            }
            ++index;
            index %= colors.Length;
        }
    }
}
