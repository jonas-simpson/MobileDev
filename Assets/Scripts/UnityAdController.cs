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
    /// Unity Ads must be initialized or else ads will not work properly
    /// </summary>
    void Start()
    {
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

        //No need to initialize if it is already done
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode);
        }
    }

    /// <summary>
    /// Load content to the Ad Unit
    /// </summary>
    public static void LoadAd()
    {
        //IMPORTANT! On ly load content AFTER initialization
        Debug.Log("Loading ad: " + adUnitId);
        Advertisement.Load(adUnitId);
    }

    /// <summary>
    /// Show the loaded content in the Ad Unit
    /// </summary>
    public static void ShowAd()
    {
        //Note that if the ad content wasn't previously loaded, this method will fail
        Debug.Log("Showing ad: " + adUnitId);
        Advertisement.Show(adUnitId);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity ads initialization failed: {error.ToString()} - {message}");
    }

    public void OnUnityAdsAdLoaded(string placementId) { }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading ad unit: {adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing ad unit {adUnitId}: {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
    }

    public void OnUnityAdsShowStart(string placementId) { }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(
        string placementId,
        UnityAdsShowCompletionState showCompletionState
    ) { }
}
