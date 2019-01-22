using AudienceNetwork;
using UnityEngine;

namespace Ads
{
    public class FBInterstitial : MonoBehaviour
    {
        private InterstitialAd interstitialAd;
        private bool isLoaded;
        private bool didClose;
        public bool Testing;
        public static FBInterstitial Instance;

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
                HideAds();
            else
                LoadInterstitial();
        }

        private void HideAds()
        {
            gameObject.SetActive(false);
        }

        public void LoadInterstitial()
        {
#if UNITY_ANDROID
            string adUnitId = GameConfig.Instance.FbAds.androidInterstitial.Trim();
#elif UNITY_IPHONE
        string adUnitID = GameConfig.Instance.FbAds.iOSInterstitial.Trim();
#endif
            interstitialAd = Testing
                ? new InterstitialAd("IMG_16_9_APP_INSTALL#457854978072123_469446143579673")
                : new InterstitialAd(adUnitId);
            this.interstitialAd.Register(this.gameObject);

            // Set delegates to get notified on changes or when the user interacts with the ad.
            this.interstitialAd.InterstitialAdDidLoad = (delegate()
            {
//                Debug.Log("Interstitial ad loaded.");
                this.isLoaded = true;
            });
            interstitialAd.InterstitialAdDidFailWithError = (delegate(string error)
            {
//                Debug.Log("Interstitial ad failed to load with error: " + error);
            });
            interstitialAd.InterstitialAdWillLogImpression = (delegate()
            {
//                Debug.Log("Interstitial ad logged impression.");
            });
            interstitialAd.InterstitialAdDidClick = (delegate()
            {
//                Debug.Log("Interstitial ad clicked.");
            });

            this.interstitialAd.interstitialAdDidClose = (delegate()
            {
//                Debug.Log("Interstitial ad did close.");
                didClose = true;
                if (this.interstitialAd != null)
                {
                    this.interstitialAd.Dispose();
                    LoadInterstitial();
                }
            });
#if UNITY_ANDROID
            this.interstitialAd.interstitialAdActivityDestroyed = (delegate()
            {
                if (!didClose)
                {
                }
                else
                {
                }
            });
#endif

            this.interstitialAd.LoadAd();
        }

        public bool ShowInterstitial()
        {
            if (!this.isLoaded)
            {
                LoadInterstitial();
            }
            else
            {
//                Debug.Log("Interstitial Ad not loaded!");
                this.interstitialAd.Show();
                AppsFlyerManager.Instance.TrackInterstitialFacebookView();
                this.isLoaded = false;
                return true;
            }

            return false;
        }
    }
}