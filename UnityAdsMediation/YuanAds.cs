using System;
using System.Collections;
using Game;
using GoogleMobileAds.Api;
//using GoogleMobileAds.Api.Mediation.AppLovin;
//using GoogleMobileAds.Api.Mediation.Vungle;
//using GoogleMobileAds.Api.Mediation.UnityAds;
using UnityEngine;

namespace Ads
{
    public class YuanAds : MonoBehaviour
    {
        private BannerView _bannerView;
        private InterstitialAd _interstitial;
        private RewardBasedVideoAd _rewardBasedVideo;
        public static YuanAds Instance;
        private bool _watched;
        private bool _runAtStart = false;
        private bool _isAdClosed;
        private bool _isCoroutineExecuting;
#if UNITY_ANDROID
        private const string AppId = "ca-app-pub-2507380763226650~2817838483";
#elif UNITY_IPHONE
        private string appId = "";
#endif

        private void Awake()
        {
            MobileAds.Initialize(AppId);
//            AppLovin.Initialize();
//            GoogleMobileAds.Api.Mediation.UnityAds.UnityAds.SetGDPRConsentMetaData(true);
//            Vungle.UpdateConsentStatus(VungleConsent.ACCEPTED, "1.0.0");
//            Vungle.GetCurrentConsentMessageVersion();
            _rewardBasedVideo = RewardBasedVideoAd.Instance;
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
                Destroy(gameObject);
        }

        public void Start()
        {
            _rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            _rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
            _rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            _rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;

            if (GameSettings.settings.boughtRemoveAds)
            {
            }
            else
                StartCoroutine(ExecuteAfterTime(12f, RequestBannerNInters));

            StartCoroutine(ExecuteAfterTime(12f, RequestRewardBasedVideo));
        }

        private void RequestBannerNInters()
        {
            RequestBanner();
            RequestInterstitial();
        }

        void Update()
        {
            if (_isAdClosed)
            {
                if (_watched) // do all the actions, reward the player
                {
                    GameManager.Instance.soundManager.PlayMusic();
                    AppsFlyerManager.Instance.TrackVideoRewardAdmobFinished();
                    GamePlay.Instance.AfterVideoAds();
                    RequestRewardBasedVideo();
                    _watched = false;
                }
                else // Ad closed but user skipped ads, so no reward
                    GameOver.Instance.OnCancel();

                GameManager.Instance.soundManager.PlayMusic();
                _isAdClosed = false; // to make sure this action will happen only once.
            }
        }

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
//                .SetBirthday(new DateTime(1985, 1, 1))
                .TagForChildDirectedTreatment(false)
                .Build();
        }

        private void RequestBanner()
        {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-2507380763226650/8505279439"; // banner
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-2507380763226650/7635050896";
#endif
            if (_bannerView != null)
                _bannerView.Destroy();

            _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
//            bannerView.OnAdLoaded += HandleAdLoaded;
//            bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;
//            bannerView.OnAdOpening += HandleAdOpened;
//            bannerView.OnAdClosed += HandleAdClosed;
//            bannerView.OnAdLeavingApplication += HandleAdLeftApplication;
            _bannerView.LoadAd(CreateAdRequest());
            HideBanner();
//            if (GameSettings.settings.boughtRemoveAds)
//            else
//                Invoke("ShowBanner", 1f);
        }

        public void HideBanner()
        {
            if (_bannerView != null)
            {
                _bannerView.Hide();
            }
        }

        public void ShowBanner()
        {
            if (GameSettings.settings.boughtRemoveAds)
                return;
            AppsFlyerManager.Instance.TrackBannerAdmobView();
            _bannerView.Show();
        }

        private void RequestInterstitial()
        {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-2507380763226650/2191152525"; // inter
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-2507380763226650/2599234686";
#endif
            if (_interstitial != null)
                _interstitial.Destroy();

            _interstitial = new InterstitialAd(adUnitId);
//            interstitial.OnAdLoaded += HandleInterstitialLoaded;
//            interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
            _interstitial.OnAdOpening += HandleInterstitialOpened;
            _interstitial.OnAdClosed += HandleInterstitialClosed;
//            interstitial.OnAdLeavingApplication += HandleInterstitialLeftApplication;
            _interstitial.LoadAd(CreateAdRequest());
        }

        private void RequestRewardBasedVideo()
        {
            _watched = false;
#if UNITY_ANDROID
            var adUnitId = "ca-app-pub-2507380763226650/8668930960"; // video
#elif UNITY_IPHONE
        var adUnitId = "ca-app-pub-2507380763226650/1449804549";
#endif
            var request = new AdRequest.Builder().Build();
            _rewardBasedVideo.LoadAd(request, adUnitId);
        }

        public bool ShowInterstitial()
        {
            if (!_interstitial.IsLoaded())
            {
            }
            else
            {
                AppsFlyerManager.Instance.TrackInterstitialAdmobView();
                _interstitial.Show();
                return true;
            }

            return false;
        }

        public bool InterstitialIsReady()
        {
            var isOk = _interstitial.IsLoaded();
            if (!isOk)
                RequestInterstitial();
            return isOk;
        }

        public bool VideoIsReady()
        {
            var isOk = _rewardBasedVideo.IsLoaded();
            if (!isOk)
                RequestRewardBasedVideo();
            return isOk;
        }

        public void ShowRewardBasedVideo()
        {
            if (!_rewardBasedVideo.IsLoaded())
                RequestRewardBasedVideo();
            else
            {
                AppsFlyerManager.Instance.TrackVideoRewardAdmobViewed();
                GameManager.Instance.soundManager.PauseMusic();
                _rewardBasedVideo.Show();
            }
        }

        #region Banner callback handlers

        public void HandleAdLoaded(object sender, EventArgs args)
        {
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
        }

        public void HandleAdOpened(object sender, EventArgs args)
        {
        }

        public void HandleAdClosed(object sender, EventArgs args)
        {
        }

        public void HandleAdLeftApplication(object sender, EventArgs args)
        {
        }

        #endregion

        #region Interstitial callback handlers

        public void HandleInterstitialLoaded(object sender, EventArgs args)
        {
        }

        public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
        }

        private void HandleInterstitialOpened(object sender, EventArgs args)
        {
            GameManager.Instance.soundManager.PauseMusic();
        }

        private void HandleInterstitialClosed(object sender, EventArgs args)
        {
            RequestInterstitial();
//            GamePlay.Instance.OverType = GamePlay.GameOverType.None;
            GameManager.Instance.soundManager.PlayMusic();
        }

        public void HandleInterstitialLeftApplication(object sender, EventArgs args)
        {
        }

        #endregion

        #region RewardBasedVideo callback handlers

        public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
        {
        }

        public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Invoke("RequestRewardBasedVideo", 5);
        }

        public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
        {
        }

        private void HandleRewardBasedVideoStarted(object sender, EventArgs args)
        {
            GameManager.Instance.soundManager.PauseMusic();
        }

        private void HandleRewardBasedVideoClosed(object sender, EventArgs args)
        {
            _isAdClosed = true;
        }

        private void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            _watched = true;
        }

        public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
            print("HandleRewardBasedVideoLeftApplication event received");
        }

        #endregion

        private void OnDestroy()
        {
            if (_interstitial != null)
            {
                _interstitial.OnAdOpening -= HandleInterstitialOpened;
                _interstitial.OnAdClosed -= HandleInterstitialClosed;
            }

            if (_rewardBasedVideo != null)
            {
                _rewardBasedVideo.OnAdFailedToLoad -= HandleRewardBasedVideoFailedToLoad;
                _rewardBasedVideo.OnAdStarted -= HandleRewardBasedVideoStarted;
                _rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
                _rewardBasedVideo.OnAdClosed -= HandleRewardBasedVideoClosed;
            }
        }

        IEnumerator ExecuteAfterTime(float time, Action task)
        {
            if (_isCoroutineExecuting)
                yield break;
            _isCoroutineExecuting = true;
            yield return new WaitForSeconds(time);
            task();
            _isCoroutineExecuting = false;
        }
    }
}