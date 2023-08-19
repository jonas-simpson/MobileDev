using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class PauseScreenBehavior : MainMenuBehavior
{
    public static bool paused;

    [Tooltip("Reference to the Pause Menu object to turn on / off")]
    public GameObject pauseMenu;

    protected override void Start()
    {
        //Initialize ads if needed
        base.Start();

        if (!UnityAdController.showAds)
        {
            //If not showing ads, just start the game
            SetPauseMenu(false);
        }
    }

    /// <summary>
    /// Reloads our current level, effectively "restarting" the game
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Will turn our pause menu on or off
    /// </summary>
    /// <param name="isPaused"></param>
    public void SetPauseMenu(bool isPaused)
    {
        paused = isPaused;

        //If the game is paused, timescale is 0, otherwise 1
        Time.timeScale = (paused) ? 0 : 1;
        pauseMenu.SetActive(paused);

        if (paused)
        {
            var result = Analytics.CustomEvent("Paused");
            if (result == AnalyticsResult.Ok)
            {
                Debug.Log("Event sent: Paused");
            }
        }
    }

    #region Share Score via Twitter
    /// <summary>
    /// Web address to create a tweet
    /// </summary>
    private const string tweetTextAddress = "http://twitter.com/intent/tweet?text=";

    /// <summary>
    /// Where we want players to visit
    /// </summary>
    private string appStoreLink = "http://google.com";

    [Tooltip("Reference to the player, for score")]
    public PlayerBehavior player;

    /// <summary>
    /// Will open Twitter with a prebuilt tweet. When called on iOS or Android,
    /// will open Twitter app if installed.
    /// </summary>
    public void TweetScore()
    {
        //Create contents of the tweet
        string tweet =
            "I got "
            + string.Format("{0:0}", player.Score)
            + " points in Endless Roller! Can you do better?";

        //Create the entire message
        string message = tweet + '\n' + appStoreLink;

        //Ensure string is URL friendly
        string url = UnityEngine.Networking.UnityWebRequest.EscapeURL(message);

        //Open the URL to create the tweet
        Application.OpenURL(tweetTextAddress + url);
    }

    #endregion
}
