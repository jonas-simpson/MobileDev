using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible for moving the player automatically and receiving input
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class PlayerBehavior : MonoBehaviour
{
    /// <summary>
    /// A reference to the Rigidbody component
    /// </summary>
    private Rigidbody rb;

    [Tooltip("How fast the ball moves left/right")]
    public float dodgeSpeed = 5;

    [Tooltip("How fast the ball moves forward automatically")]
    [Range(0, 10)]
    public float rollSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        //Get access to our Rigidbody component
        rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// FixedUpdate is called at a fixed framerate and is a prime place to put anything based on time.
    /// </summary>

    void FixedUpdate()
    {
        //Check if we're moving to the side
        var horizontalSpeed = Input.GetAxis("Horizontal") * dodgeSpeed;

        //Check if we're running in the Unity editor or a standalone build
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        //If the mouse is held down (or the screen is tapped on Mobile)
        if (Input.GetMouseButton(0))
        {
            horizontalSpeed = CalculateMovement(Input.mousePosition);
        }
#endif

        //Check if we are running on a mobile device
#if UNITY_IOS || UNITY_ANDROID
        //Check if input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            //Store the first touch detected
            Touch touch = Input.touches[0];
            horizontalSpeed = CalculateMovement(touch.position);
        }
#endif

        rb.AddForce(horizontalSpeed, 0, rollSpeed);
    }

    /// <summary>
    /// Will figure out where to move the player horizontally
    /// </summary>
    /// <param name="pixelPos"> The position the player has touched/dlicked on</param>
    /// <returns>The direction to move in the x axis</returns>
    private float CalculateMovement(Vector3 pixelPos)
    {
        //Convert to a 0 to 1 scale
        Vector3 worldPos = Camera.main.ScreenToViewportPoint(pixelPos);

        float xMove = 0;

        //If we pressed the right side of the screen
        if (worldPos.x < 0.5f)
        {
            xMove = -1;
        }
        else
        {
            //Otherwise we're on the left
            xMove = 1;
        }

        return xMove * dodgeSpeed;
        ;
    }
}
