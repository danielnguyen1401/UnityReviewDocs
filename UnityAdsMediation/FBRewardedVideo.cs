using AudienceNetwork;
using UnityEngine;

namespace Ads
{
    public class FBRewardedVideo : MonoBehaviour
    {
        private RewardedVideoAd rewardedVideoAd;
        private bool isLoaded;
        private bool didClose;
        public bool Testing;
        public static FBRewardedVideo Instance;

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
            LoadRewardedVideo();
        }

        private void LoadRewardedVideo()
        {
#if UNITY_ANDROID
            var videoId = GameConfig.Instance.FbAds.androidRewardedVideo.Trim();
#elif UNITY_IPHONE
            var videoId = GameConfig.Instance.FbAds.iOSRewardedVideo.Trim();
#endif
            RewardData rewardData = new RewardData();
            rewardData.UserId = "USER_ID";
            rewardData.Currency = "REWARD_ID";

            rewardedVideoAd =
                Testing
                    ? new RewardedVideoAd("VID_HD_16_9_15S_APP_INSTALL#2025198837536206_2115449855177770", rewardData)
                    : new RewardedVideoAd(videoId, rewardData);
            rewardedVideoAd.Register(gameObject);

            // Set delegates to get notified on changes or when the user interacts with the ad.
            rewardedVideoAd.RewardedVideoAdDidLoad = (delegate()
            {
//                Debug.Log("RewardedVideo ad loaded.");
                isLoaded = true;
            });
            rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate(string error)
            {
//                Debug.Log("RewardedVideo ad failed to load with error: " + error);
            });
            rewardedVideoAd.RewardedVideoAdWillLogImpression = (delegate()
            {
//                Debug.Log("RewardedVideo ad logged impression.");
            });
            rewardedVideoAd.RewardedVideoAdDidClick = (delegate()
            {
//                Debug.Log("RewardedVideo ad clicked.");
            });

            rewardedVideoAd.RewardedVideoAdDidClose = (delegate()
            {
                didClose = true;
                if (rewardedVideoAd != null)
                {
                    rewardedVideoAd.Dispose();
                }
            });
            rewardedVideoAd.RewardedVideoAdDidSucceed = (delegate() // for finished video
            {
                GamePlay.Instance.AfterVideoAds();
            });
            rewardedVideoAd.RewardedVideoAdDidFail = (delegate() // for fail finished
            {
                GameOver.Instance.OnCancel();
            });

            rewardedVideoAd.rewardedVideoAdActivityDestroyed = (delegate()
            {
                if (!didClose)
                {
                    GameOver.Instance.OnCancel();
                }
            });
            rewardedVideoAd.LoadAd();
        }

        public bool VideoIsReady()
        {
            return isLoaded;
        }

        public bool ShowRewardedVideo()
        {
            if (!isLoaded)
            {
                LoadRewardedVideo();
            }
            else
            {
                rewardedVideoAd.Show();
                AppsFlyerManager.Instance.TrackRewardVideoFacebookView();
                isLoaded = false;
                return true;
            }

            return false;
        }
    }
}