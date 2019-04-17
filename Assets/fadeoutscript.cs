using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadeoutscript : MonoBehaviour
{
    Image img;
    public float rateofdecrease = 0.005f;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        Color c = img.color;
        c.a = 0;
        img.color = c;
        img.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator fadescreenout()
    {
        print("GOTTEM");
        img.enabled = true;
        while (img.color.a < 1)
        {
            print("GOING");
            Color c = img.color;
            c.a += rateofdecrease;
            img.color = c;
            yield return new WaitForEndOfFrame();
        }
    }
}
