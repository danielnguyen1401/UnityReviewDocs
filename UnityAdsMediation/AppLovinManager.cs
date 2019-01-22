using Ads;
using UnityEngine;

public class AppLovinManager : MonoBehaviour
{
    private const string SDK_KEY =
        "AFnm0QSLh8Xyt7yzuyD1hoB8L0dFN2OrEBTT81F9hi8-J7JiaF_f67k8vhsJo_aekXTz0GqIeOqxXCWK8s7Tlb";

    private bool _isPreloadingRewardedVideo = false;
    public static AppLovinManager Instance;

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

    void Start()
    {
        AppLovin.SetSdkKey(SDK_KEY);
        AppLovin.InitializeSdk();
//        AppLovin.SetTestAdsEnabled("true");
        AppLovin.SetUnityAdListener(gameObject.name); // gameobject with name ApplovinListener

        Invoke("LoadVideoDelay", 5f);
    }

    private void LoadVideoDelay()
    {
        AppLovin.LoadRewardedInterstitial();
        if (GameSettings.settings.boughtRemoveAds)
            AppLovin.HideAd();
    }

    public bool ShowInterstitial()
    {
        if (!AppLovin.HasPreloadedInterstitial())
            AppLovin.PreloadInterstitial();
        else
        {
            AppLovin.ShowInterstitial();
            return true;
        }

        return false;
    }

    public bool InterstitialIsReady() // if readey inter
    {
        var result = AppLovin.HasPreloadedInterstitial();
        if (result)
        {
        }
        else
            AppLovin.PreloadInterstitial();

        return result;
    }

    public bool ShowRewardedInterstitial() // show video
    {
        if (!AppLovin.IsIncentInterstitialReady())
        {
            _isPreloadingRewardedVideo = true;
            AppLovin.LoadRewardedInterstitial();
        }
        else
        {
            _isPreloadingRewardedVideo = false;
            AppLovin.ShowRewardedInterstitial();
            AppsFlyerManager.Instance.TrackVideoAppLovinView();
            return true;
        }

        return false;
    }

    public void ShowBanner()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        AppLovin.ShowAd(AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_BOTTOM);
        AppsFlyerManager.Instance.TrackBannerAppLovinView();
#endif
    }

    void onAppLovinEventReceived(string ev)
    {
//        Debug.Log("EVENT : " + ev);
        // Log AppLovin event
        // Special Handling for Rewarded events
        if (ev.Equals("LOADBANNERFAILED"))
            YuanAds.Instance.ShowBanner();

        if (ev.Equals("REWARDAPPROVED"))
        {
            // Process an event like REWARDAPPROVEDINFO:100:Credits
//                char[] delimiter = {'|'};
//                string[] split = ev.Split(delimiter);
            // Pull out the amount of virtual currency.
//                double amount = double.Parse(split[1]);
            // Pull out the name of the virtual currency
//                string currencyName = split[2];
            // Do something with this info - for example, grant coins to the user
            // myFunctionToUpdateBalance(currencyName, amount);
//                Log("Rewarded " + amount + " " + currencyName);
            AppsFlyerManager.Instance.TrackVideoAppLovinFinished();
            GamePlay.Instance.AfterVideoAds();
        }
//        }
        // Check if this is a Rewarded Video preloading event
        else if (_isPreloadingRewardedVideo && (ev.Equals("LOADED") || ev.Equals("LOADFAILED")))
        {
            _isPreloadingRewardedVideo = false;

            if (ev.Equals("LOADED"))
            {
//                RewardedVideoButtonTitle.text = REWARDED_VIDEO_BUTTON_TITLE_SHOW;
            }
            else
            {
//                RewardedVideoButtonTitle.text = REWARDED_VIDEO_BUTTON_TITLE_PRELOAD;
            }
        }
    }

//    private void Log(string message)
//    {
//        StatusText.text = message;
//        Debug.Log(message);
//    }
}