using System;
using System.Collections;
using UnityEngine;

namespace Ads
{
    public class YuanMediationAds : MonoBehaviour
    {
        private static YuanMediationAds _instance;
        private bool _isExecuting;

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (GameSettings.settings.boughtRemoveAds)
            {
            }
            else
            {
                StartCoroutine(ExecuteAfterTime(4f, BannerMediation));
            }
        }

        void BannerMediation()
        {
            if (!FBBannerAds.Instance.BannerLoaded())
            {
                // show applovin
                AppLovinManager.Instance.ShowBanner();
                // show vungle
                // show admob
            }
        }

        public void InterstitialMediation() // call when game Over
        {
            if (!FBInterstitial.Instance.ShowInterstitial())
            {
                if (!IntertitialAds.Instance.ShowAdWhenReady())
                {
                    if (!AppLovinManager.Instance.ShowInterstitial())
                    {
                        if (!YuanAds.Instance.ShowInterstitial())
                        {
                        }
                    }
                }
            }
        }

        public void VideoMediation() // for continue
        {
            if (!FBRewardedVideo.Instance.ShowRewardedVideo())
            {
                if (!RewardAdsUnity.Instance.ShowVideoAds())
                {
                    if (!AppLovinManager.Instance.ShowRewardedInterstitial())
                    {
//                        // show Vungle
//                        // show Admob
                        YuanAds.Instance.ShowRewardBasedVideo();
                    }
                }
            }
        }

        void Update()
        {
        }

        public static YuanMediationAds ShareInstance()
        {
            return _instance;
        }

        IEnumerator ExecuteAfterTime(float time, Action task)
        {
            if (_isExecuting)
                yield break;
            _isExecuting = true;
            yield return new WaitForSeconds(time);
            task();
            _isExecuting = false;
        }
    }
}