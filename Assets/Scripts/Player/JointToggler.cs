using UnityEngine;

//I did not make this
//created by bjennings76 on  https://forum.unity.com/threads/enable-disable-a-joint.24525/ 

public class JointToggler : MonoBehaviour
{
    [SerializeField] private Joint joint;
    private Rigidbody connectedBody;

    private void Awake()
    {
        joint = joint ? joint : GetComponent<Joint>();
        if (joint) connectedBody = joint.connectedBody;
        else Debug.LogError("No joint found.", this);
    }

    private void OnEnable() { joint.connectedBody = connectedBody; }

    private void OnDisable()
    {
        joint.connectedBody = null;
        connectedBody.WakeUp();
    }


}