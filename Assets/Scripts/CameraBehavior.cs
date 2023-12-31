using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Will adjust the camera to follow and face a target
/// </summary>

public class CameraBehavior : MonoBehaviour
{
    [Tooltip("What object should the camera be looking at")]
    public Transform target;

    [Tooltip("How offset will the camera be to the target?")]
    public Vector3 offset = new Vector3(0, 3, -6);

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Check if target is a valid object
        if (target)
        {
            //Set our position to an offset of our target
            transform.position = target.position + offset;

            //Chenge the rotation to face target
            transform.LookAt(target);
        }
    }
}
