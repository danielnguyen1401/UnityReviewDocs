using Ads;
using Game;
using UnityEngine;
using UnityEngine.Monetization;

public class RewardAdsUnity : MonoBehaviour
{
    public static RewardAdsUnity Instance;

    private string placementId = "rewardedVideo";
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
        Monetization.Initialize(gameID, testMode);
    }

    public bool ShowVideoAds()
    {
        if (!UnityAdsReady())
            return false;

        ShowAdPlacementContent ad = null;
        ad = Monetization.GetPlacementContent(placementId) as ShowAdPlacementContent;

        if (ad != null)
        {
            AppsFlyerManager.Instance.TrackVideoRewardUnityViewed();
            GameManager.Instance.soundManager.PauseMusic();
            ad.Show(AdFinished);
            print("VIDEO ADS VIEWED");
        }
        return true;
    }

    public bool UnityAdsReady()
    {
        return Monetization.IsReady(placementId);
    }

    void AdFinished(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            print("UNITY ADS FINISHED");
            AppsFlyerManager.Instance.TrackVideoRewardUnityFinished();
            GameManager.Instance.soundManager.PlayMusic();
            GamePlay.Instance.AfterVideoAds();
        }
    }
}