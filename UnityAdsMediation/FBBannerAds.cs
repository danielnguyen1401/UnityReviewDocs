using AudienceNetwork;
using Game;
using UnityEngine;

namespace Ads
{
    public class FBBannerAds : MonoBehaviour
    {
        private bool _bannerLoaded;
        private AdView _adView;
        public bool Testing;
        public static FBBannerAds Instance;

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
                HideAds();
            }
            else
                LoadBanner();
        }

        private void LoadBanner()
        {
#if UNITY_ANDROID
            string adUnitId = GameConfig.Instance.FbAds.androidBanner.Trim();
#elif UNITY_IPHONE
        string adUnitID = GameConfig.Instance.FbAds.iOSBanner.Trim();
#endif
            if (_adView)
                _adView.Dispose();

            _adView = Testing
                ? new AdView("IMG_16_9_APP_INSTALL#457854978072123_469445200246434", AdSize.BANNER_HEIGHT_50)
                : new AdView(adUnitId, AdSize.BANNER_HEIGHT_50);

            _adView.Register(gameObject);

            _adView.AdViewDidFailWithError = (delegate(string error)
            {
                _bannerLoaded = false;
//            Debug.Log("Banner failed to load with error: " + error);
            });
            _adView.AdViewWillLogImpression = (delegate() { });
            _adView.AdViewDidClick = (delegate() { });

            // Initiate a request to load an ad.
            _adView.LoadAd();
            // Set delegates to get notified on changes or when the user interacts with the ad.
            _adView.AdViewDidLoad = (delegate() // Banner loaded.
            {
                _bannerLoaded = true;
                _adView.Show(AdPosition.BOTTOM);
                AppsFlyerManager.Instance.TrackBannerFacebookView();
            });
            _adView.AdViewWillLogImpression = (delegate() // Banner logged impression
            {
            });
            _adView.AdViewDidClick = (delegate() // Banner clicked
            {
            });
        }

        public bool BannerLoaded()
        {
            return _bannerLoaded;
        }

        
        private void HideAds()
        {
            gameObject.SetActive(false);
        }

    }
}