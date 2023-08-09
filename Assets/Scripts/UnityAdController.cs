using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdController
    : MonoBehaviour,
        IUnityAdsInitializationListener,
        IUnityAdsLoadListener,
        IUnityAdsShowListener
{
    /// <summary>
    /// If we should show ads or not
    /// </summary>
    public static bool showAds = true;

    //Nullable type
    public static DateTime? nextRewardTime = null;

    private string androidGameId = "5374187";
    private string iOSGameId = "5374186";
    private static string gameId;

    private string androidAdUnitId = "Interstitial_Android";
    private string iOSAdUnitId = "Interstitial_iOS";
    private static string adUnitId;

    /// <summary>
    /// If the game is in test mode or not
    /// </summary>
    private bool testMode = true;

    /// <summary>
    /// for holding the obstacle for continuing the game
    /// </summary>
    public static ObstacleBehavior obstacle;

    private void Awake()
    {
        InitializeAds();
    }

    /// <summary>
    /// Unity Ads must be initialized or else ads will not work properly
    /// </summary>
    public void InitializeAds()
    {
        #region Filter Platforms
#if UNITY_IOS
        gameId = iOSGameId;
        adUnitId = iOSAdUnitId;
#elif UNITY_ANDROID
        gameId = androidGameId;
        adUnitId = androidAdUnitId;
#elif UNITY_EDITOR
        //for testing in the editor
        gameId = androidGameId;
        adUnitId = androidAdUnitId;
#endif
        #endregion

        //No need to initialize if it is already done
        if (!Advertisement.isInitialized)
        {
            //Use the functions provided by this to allow custom behavior on the ads
            Advertisement.Initialize(gameId, testMode, this);
        }
    }

    /// <summary>
    /// Load content to the Ad Unit
    /// </summary>
    public void LoadAd()
    {
        //IMPORTANT! On ly load content AFTER initialization
        Debug.Log("Loading ad: " + adUnitId);
        Advertisement.Load(adUnitId, this);
    }

    /// <summary>
    /// Show the loaded content in the Ad Unit
    /// </summary>
    public void ShowAd()
    {
        //Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing ad: " + adUnitId);
        Advertisement.Show(adUnitId, this);
    }

    public void ShowRewardAd()
    {
        ShowAd();
    }

    #region IUnityAdsInitializationListener Methods
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity ads initialization failed: {error.ToString()} - {message}");
    }
    #endregion

    #region IUnityAdsLoadListener Methods
    public void OnUnityAdsAdLoaded(string placementId)
    {
        //Actions to take when an Ad is ready to display, such as enabling a rewards button
        Debug.Log("Ad loaded: " + placementId);
        ShowRewardAd();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading ad unit: {adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }
    #endregion

    #region IUnityAdsShowListener Methods
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing ad unit {adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //Pause the game while ad is shown
        PauseScreenBehavior.paused = true;
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(
        string placementId,
        UnityAdsShowCompletionState showCompletionState
    )
    {
        //If there is an obstacle, we can remove it to continue the game
        if (obstacle != null && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            obstacle.Continue();
        }
        //Unpause when ad is over
        PauseScreenBehavior.paused = false;
        Time.timeScale = 1f;

        //Set reward ad timer
        nextRewardTime = DateTime.Now.AddSeconds(15);
    }
    #endregion
}
