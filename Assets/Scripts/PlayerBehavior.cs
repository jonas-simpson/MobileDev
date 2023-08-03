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

    [Header("Swipe properties")]
    [Tooltip("How far will the player move upon swiping")]
    public float swipeMove = 2f;

    [Tooltip("How far must the player swipe before we will execute the action (in inches)")]
    public float minSwipeDistance = 0.25f;

    /// <summary>
    /// Used to hold the value that converts minSwipeDistance to pixels
    /// </summary>
    private float minSwipeDistancePixels;

    /// <summary>
    /// Stores the starting position of mobile touch events
    /// </summary>
    private Vector2 touchStart;

    [Header("Scaling properties")]
    [Tooltip("The minimum size (in Unity units) that the player should be")]
    public float minScale = 0.5f;

    [Tooltip("The maximum size (in Unity units) that the player should be")]
    public float maxScale = 3.0f;

    /// <summary>
    /// The current scale of the player
    /// </summary>
    private float currentScale = 1;

    // Start is called before the first frame update
    void Start()
    {
        //Get access to our Rigidbody component
        rb = gameObject.GetComponent<Rigidbody>();

        minSwipeDistancePixels = minSwipeDistance * Screen.dpi;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        //Check if we are running on a mobile device
#if UNITY_IOS || UNITY_ANDROID
        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            //Store the first touch detected
            Touch touch = Input.touches[0];

            SwipeTeleport(touch);

            ScalePlayer();
        }
#endif
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

    /// <summary>
    /// Will teleport the player if swiped to the left or right
    /// </summary>
    /// <param name="touch">Current touch event</param>
    private void SwipeTeleport(Touch touch)
    {
        //Check if touch just started
        if (touch.phase == TouchPhase.Began)
        {
            //if so, set touchStart
            touchStart = touch.position;
        }
        //if the touch has ended
        else if (touch.phase == TouchPhase.Ended)
        {
            //Get the position the touch ended at
            Vector2 touchEnd = touch.position;

            //Calculate the difference between the beginning and end of the touch on the x axis
            float x = touchEnd.x - touchStart.x;

            //If we are not moving far enough, don't do the teleport
            if (Mathf.Abs(x) < minSwipeDistancePixels)
            {
                return;
            }

            Vector3 moveDirection;
            //if moved negatively in the x axis, move left
            if (x < 0)
            {
                moveDirection = Vector3.left;
            }
            else
            {
                //Otherwise we're on the right
                moveDirection = Vector3.right;
            }

            RaycastHit hit;
            //Only move if we wouldn't hit something
            if (!rb.SweepTest(moveDirection, out hit, swipeMove))
            {
                //Move the player
                rb.MovePosition(rb.position + (moveDirection * swipeMove));
            }
        }
    }

    /// <summary>
    /// Will change the player's scale via pinching and stretching two touch events
    /// </summary>
    private void ScalePlayer()
    {
        //We must have two touches to check if we are scaling the object
        if (Input.touchCount != 2)
        {
            return;
        }
        else
        {
            //Store the touches detected
            Touch touch0 = Input.touches[0];
            Touch touch1 = Input.touches[1];

            //Find the position in the previous frame of each touch
            Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
            Vector2 touch1Prev = touch1.position - touch1.deltaPosition;

            //Find the distance (or magnitude) between the touches in each frame
            float prevTouchDeltaMag = (touch0Prev - touch1Prev).magnitude;
            float touchDeltaMag = (touch0Prev - touch1Prev).magnitude;

            //Find the difference in distances between each frame
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            //keep the change consistent no matter the framerate
            float newScale = currentScale - (deltaMagnitudeDiff * Time.deltaTime);

            //Ensure that it is valid
            newScale = Mathf.Clamp(newScale, minScale, maxScale);

            //Update the player's scale
            transform.localScale = Vector3.one * newScale;

            //Set our current scale for the next frame
            currentScale = newScale;
        }
    }
}
