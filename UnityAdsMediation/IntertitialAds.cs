using Ads;
using UnityEngine;
using UnityEngine.Monetization;

public class IntertitialAds : MonoBehaviour
{
    public static IntertitialAds Instance;

    string placementId = "interstitial";
    public bool testMode = false;

#if UNITY_IOS
        public const string gameID = "2979944";
#elif UNITY_ANDROID
    public const string gameID = "2979945";
#elif UNITY_EDITOR
        public const string gameID = "1111111";
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        if (GameSettings.settings.boughtRemoveAds)
        {
        }
        else
            Monetization.Initialize(gameID, testMode);
    }

//    public void ShowAd()
//    {
//        ShowAdWhenReady();
//    }

    public bool ShowAdWhenReady()
    {
        if (!Monetization.IsReady(placementId))
            return false;
        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;
        if (ad != null) //ShowIntertitial_UnityAds_OK
        {
            ad.Show();
            ad.Show(AdFinished);
            AppsFlyerManager.Instance.TrackInterstitialUnityView();
        }
        return true;
    }

    void AdFinished(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
//            GamePlay.Instance.OverType = GamePlay.GameOverType.None;
        }
    }
}