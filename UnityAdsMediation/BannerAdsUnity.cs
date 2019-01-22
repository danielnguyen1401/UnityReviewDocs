using UnityEngine;
using UnityEngine.Advertisements;

namespace Ads
{
    public class BannerAdsUnity : MonoBehaviour
    {
        string bannerPlacement = "bannerBottom";
        public static BannerAdsUnity Instance;
        public bool TestMode;
#if UNITY_IOS
    public const string gameID = "2979944";
#elif UNITY_ANDROID
        public const string gameID = "2979945";
#elif UNITY_EDITOR
    public const string gameID = "1111111";
#endif

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (GameSettings.settings.boughtRemoveAds)
            {
            }
            else
                Advertisement.Initialize(gameID, TestMode);
        }

        public bool ShowBannerWhenReady()
        {
            if (!Advertisement.IsReady(bannerPlacement))
            {
            }
            else
            {
                Advertisement.Banner.Show(bannerPlacement);
                AppsFlyerManager.Instance.TrackBannerUnityView();
                return true;
            }

            return false;
        }
    }
}