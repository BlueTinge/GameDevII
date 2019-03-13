using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLevel : MonoBehaviour
{
    Cartesian CS;
    public WallObject wo;

    // Start is called before the first frame update
    void Start()
    {
        int len = 10;
        int width = 10;
        CS = new Cartesian(this.transform,len,1,width,1f,1f,1f);

        for(int x = 0; x < len; x++)
        {
            for(int z = 0; z < width; z++)
            {
                CS.GetEdge(new Vector3(x, 0, z), CS.GetForwardDirection(Orientation.SOUTH_TO_NORTH)).AddComponent(new Wall(wo, .1f));
                CS.GetEdge(new Vector3(x, 0, z), CS.GetForwardDirection(Orientation.WEST_TO_EAST)).AddComponent(new Wall(wo, .1f));
            }
        }
        CS.GetEdge(new Vector3(0, 0, 0), CS.GetBackwardDirection(Orientation.SOUTH_TO_NORTH)).AddComponent(new Wall(wo, .1f));
        CS.GetEdge(new Vector3(0, 0, 0), CS.GetBackwardDirection(Orientation.WEST_TO_EAST)).AddComponent(new Wall(wo, .1f));

        for (int x = 0; x < len; x++)
        {
            for (int z = 0; z < width; z++)
            {
                CS.GetEdge(new Vector3(x, 0, z), CS.GetForwardDirection(Orientation.SOUTH_TO_NORTH)).Translate();
                CS.GetEdge(new Vector3(x, 0, z), CS.GetForwardDirection(Orientation.WEST_TO_EAST)).Translate();
            }
        }
        CS.GetEdge(new Vector3(0, 0, 0), CS.GetBackwardDirection(Orientation.SOUTH_TO_NORTH)).Translate();
        CS.GetEdge(new Vector3(0, 0, 0), CS.GetBackwardDirection(Orientation.WEST_TO_EAST)).Translate();

        wo.UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
