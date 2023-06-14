using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleBehavior : MonoBehaviour
{
    [Tooltip("How long to wait before restarting the game")]
    public float waitTime = 2.0f;

    private void OnCollisionEnter(Collision other)
    {
        //First check if we collided with the player
        if (other.gameObject.GetComponent<PlayerBehavior>())
        {
            //Destroy the player
            Destroy(other.gameObject);

            //Call the function ResetGame after waitTime has passed
            Invoke("ResetGame", waitTime);
        }
    }

    /// <summary>
    /// Will restart the currently loaded level
    /// </summary>
    private void ResetGame()
    {
        //Restarts the current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
