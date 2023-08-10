using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBehavior : MonoBehaviour
{
    public UnityAdController adController;

    /// <summary>
    /// Will load a new scene upon being called
    /// </summary>
    /// <param name="levelName">The name of the scene to load</param>
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);

        if (UnityAdController.showAds)
        {
            //Show an ad
            if (adController)
                adController.LoadAd();
        }
    }

    public void DisableAds()
    {
        UnityAdController.showAds = false;

        //Used to store that we shouldn't show ads
        PlayerPrefs.SetInt("Show Ads", 0);
    }

    protected virtual void Start()
    {
        //Initialize the ShowAds variable
        UnityAdController.showAds = (PlayerPrefs.GetInt("Show Ads", 1) == 1);
    }
}
