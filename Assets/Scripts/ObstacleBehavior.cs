using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ObstacleBehavior : MonoBehaviour
{
    [Tooltip("How long to wait before restarting the game")]
    public float waitTime = 2.0f;

    private GameObject player;
    UnityAdController adController;

    private void Start()
    {
        adController = FindObjectOfType<UnityAdController>();
    }

    private void OnCollisionEnter(Collision other)
    {
        //First check if we collided with the player
        if (other.gameObject.GetComponent<PlayerBehavior>())
        {
            //Destroy (hide) the player
            other.gameObject.SetActive(false);
            player = other.gameObject;

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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //Bring up restart menu
        var go = GetGameOverMenu();
        go.SetActive(true);

        //Get our continue button
        var buttons = go.transform.GetComponentsInChildren<Button>();
        Button continueButton = null;

        foreach (var button in buttons)
        {
            if (button.gameObject.name == "Continue Button")
            {
                continueButton = button;
                break;
            }
        }

        //If we found the button we can use it
        if (continueButton)
        {
            // if (UnityAdController.showAds)
            // {
            //     //If player clicks on this button we want to show an ad and then continue
            //     // var adController = GameObject.FindObjectOfType<UnityAdController>();
            //     // continueButton.onClick.AddListener(adController.ShowAd);
            //     // UnityAdController.obstacle = this;
            StartCoroutine(ShowContinue(continueButton));
            // }
            // else
            // {
            //     //If we can't play ad, no need for continue button
            //     continueButton.gameObject.SetActive(false);
            // }
        }
    }

    public GameObject explosion;

    /// <summary>
    /// If the object is tapped, we spawn an explosion and destroy this object
    /// </summary>
    private void PlayerTouch()
    {
        Debug.Log("Message received");
        if (explosion != null)
        {
            var particles = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(particles, 1.0f);
        }

        Destroy(this.gameObject);
    }

    /// <summary>
    /// Retrieves the Game Over menu game object
    /// </summary>
    /// <returns>The Game Over menu</returns>
    GameObject GetGameOverMenu()
    {
        var canvas = GameObject.Find("Canvas").transform;
        return canvas.Find("Game Over").gameObject;
    }

    /// <summary>
    /// Handles continuing the game if needed
    /// </summary>
    public void Continue()
    {
        var go = GetGameOverMenu();
        go.SetActive(false);
        player.SetActive(true);

        //Explode this as well (so if we respawn player can continue)
        PlayerTouch();
    }

    public IEnumerator ShowContinue(Button contButton)
    {
        while (true)
        {
            var btnText = contButton.GetComponentInChildren<TextMeshProUGUI>();

            //Check if we haven't reached the next reward time yet, if one exists
            var rewardTime = UnityAdController.nextRewardTime;

            bool validTime = rewardTime.HasValue;
            bool timePassed = true;

            if (validTime)
            {
                timePassed = DateTime.Now > rewardTime.Value;
            }

            if (!timePassed)
            {
                //Unable to click the button
                contButton.interactable = false;

                //Get the time remaining until we get to the next reward time
                TimeSpan remaining = rewardTime.Value - DateTime.Now;

                //Get the time left in the following format: 99:99
                var countdownText = string.Format(
                    "{0:D2}:{1:D2}",
                    remaining.Minutes,
                    remaining.Seconds
                );

                //Set our button's text to reflect the new time
                btnText.text = countdownText;

                //Come back after 1 second and check again
                //yield return new WaitForSecondsRealtime(1f);
                yield return new WaitForSeconds(1f);
            }
            else if (!UnityAdController.showAds)
            {
                //It's valid to click the button now
                contButton.interactable = true;

                //If player clicks on the button we want to just continue
                contButton.onClick.AddListener(Continue);

                UnityAdController.obstacle = this;

                Debug.Log("Button should say 'Free Continue'");
                //Change text to allow continue
                btnText.text = "Free Continue";

                //We can now leave the coroutine
                break;
            }
            else
            {
                //It's valid to click the button now
                contButton.interactable = true;

                //if the player clicks on button we want to play ad and then continue

                contButton.onClick.AddListener(adController.LoadAd);
                UnityAdController.obstacle = this;

                //Change text to its original version
                btnText.text = "Continue (Play Ad)";

                //We can now leave the coroutine
                yield break;
            }
        }
    }
}
