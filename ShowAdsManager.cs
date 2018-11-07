using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using System;
//using UnityEngine.Advertisements;
public class ShowAdsManager : MonoBehaviour
{
#if UNITY_IPHONE
	 string idAdMobInterstitial = "";
    string idAdMobRewardBasedVideoAd = "";
	 string idAdMobBanner = "";
	 string testDeviceId = "";
#endif
#if UNITY_ANDROID
    public string idAdMobInterstitial = "";
    public string idAdMobRewardBasedVideoAd = "";
    public string idAdMobBanner = "";
    //public string testDeviceId = "";
#endif
    public static ShowAdsManager instance = null;
    InterstitialAd interstitial;
    BannerView bannerView;
    public RewardBasedVideoAd rewardBasedVideo;
    
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;

        }
        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

        this.RequestRewardBasedVideo();
    }

    public void LoadFullAdmob() // RequestInterstial
    { 
        // Create an empty ad request.
        interstitial = new InterstitialAd("/93656639/16423759");
        AdRequest request = new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
            //.AddTestDevice(testDeviceId)  // My test device.
            .Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    public void ShowFullAdmod()
    {
        interstitial.Show();
        LoadFullAdmob();
    }

    public void LoadBannerAdmod()
    {
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView("/93656639/76931428", AdSize.Banner, AdPosition.Top);
        // Create an empty ad request.
        AdRequest requestBanner = new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
            //.AddTestDevice(testDeviceId)  // My test device.
            .Build();
        // Load the banner with the request.
        bannerView.LoadAd(requestBanner);
    }


    private void RequestRewardBasedVideo()
    {
        //#if UNITY_ANDROID
        //   string adUnitId = "ca-app-pub-4670407930848534/8893284217";
        //#elif UNITY_IPHONE
        //    string adUnitId = "ca-app-pub-3940256099942544/1712485313";
        //#else
        //    string adUnitId = "unexpected_platform";
        //#endif
        
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request, "/93656639/93784512");
    }

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print(
            "HandleRewardBasedVideoFailedToLoad event received with message: "
                             + args.Message);
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        this.RequestRewardBasedVideo();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        MonoBehaviour.print(
            "HandleRewardBasedVideoRewarded event received for "
                        + amount.ToString() + " " + type);
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    }
    public void ShowRewardBasedVideo()
    {
        if (this.rewardBasedVideo.IsLoaded())
        {
            this.rewardBasedVideo.Show();       
        }
    }
    public void ShowBanner()
    {
        bannerView.Show();
    }

    public void HideBanner()
    {
        bannerView.Hide();
    }


}
