
/***********************************************************************************************************
 * Produced by App Advisory	- http://app-advisory.com													   *
 * Facebook: https://facebook.com/appadvisory															   *
 * Contact us: https://appadvisory.zendesk.com/hc/en-us/requests/new									   *
 * App Advisory Unity Asset Store catalog: http://u3d.as/9cs											   *
 * Developed by Gilbert Anthony Barouch - https://www.linkedin.com/in/ganbarouch                           *
 ***********************************************************************************************************/


#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
#pragma warning disable 0618 // obslolete
#pragma warning disable 0108 
#pragma warning disable 0649 //never used


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if ENABLE_ADMOB

using GoogleMobileAds.Api;

namespace AppAdvisory.Ads
{
	public class AAAdmob : AdBase, IInterstitial, IBanner, IRewardedVideo
	{
		public string bannerId
		{
			get
			{
				return adIds.admobBannerID;
			}
		}

		public string interstitialID
		{
			get
			{
				return adIds.admobInterstitialID;
			}
		}

		public string rewardedVideoID
		{
			get
			{
				return adIds.admobRewardedVideoID;
			}
		}

		public string Name()
		{
			return "AAAdmob";
		}

		public void Init()
		{
			if(bannerId != null && string.IsNullOrEmpty(bannerId) == false)
				Debug.LogWarning("AAAdmob - Init bannerId = " + bannerId);

			if(interstitialID != null && string.IsNullOrEmpty(interstitialID) == false)
				Debug.LogWarning("AAAdmob - Init interstitialID = " + interstitialID);

			if(rewardedVideoID != null && string.IsNullOrEmpty(rewardedVideoID) == false)
				Debug.LogWarning("AAAdmob - Init rewardedVideoID = " + rewardedVideoID);

			RequestBanner();

			RequestInterstitial();
		}

		BannerView bannerView;
		InterstitialAd interstitial;

		private void RequestBanner()
		{
			if(!string.IsNullOrEmpty(bannerId))
			{
				Debug.LogWarning("AAAdmob - RequestBanner with bannerid = " + bannerId);	
				
				bannerView = new BannerView(bannerId, AdSize.SmartBanner, AdPosition.Bottom);
				bannerView.LoadAd(createAdRequest());
				bannerView.Hide();
			}
			else
			{
				Debug.LogWarning("AAAdmob - RequestBanner ERROR ID IS NULL!!");	
			}
		}
	
		Action<bool> onAdLoadSuccessInterstitial = null; 

		private void RequestInterstitial(Action<bool> onAdLoadSuccess)
		{
			this.onAdLoadSuccessInterstitial = onAdLoadSuccess;

			if(!string.IsNullOrEmpty(interstitialID))
			{
				Debug.LogWarning("AAAdmob - RequestInterstitial with interstitialID = " + interstitialID);

				interstitial = new InterstitialAd(interstitialID);

				interstitial.OnAdLoaded -= OnAdLoadedInterstitial;
				interstitial.OnAdFailedToLoad -= OnAdFailedToLoadInterstitial;

				interstitial.OnAdLoaded += OnAdLoadedInterstitial;
				interstitial.OnAdFailedToLoad += OnAdFailedToLoadInterstitial;

				interstitial.LoadAd(createAdRequest());
			}
			else
			{
				Debug.LogWarning("AAAdmob - RequestInterstitial ERROR ID IS NULL!!");	

			}
		}

		void OnAdLoadedInterstitial (object sender, EventArgs e)
		{
			print("OnAdLoadedInterstitial");

			if(onAdLoadSuccessInterstitial != null)
				onAdLoadSuccessInterstitial(true);

			this.onAdLoadSuccessInterstitial = null;
			interstitial.OnAdLoaded -= OnAdLoadedInterstitial;
			interstitial.OnAdFailedToLoad -= OnAdFailedToLoadInterstitial;
		}

		void OnAdFailedToLoadInterstitial (object sender, AdFailedToLoadEventArgs e)
		{
			print("OnAdFailedToLoadInterstitial");

			if(onAdLoadSuccessInterstitial != null)
				onAdLoadSuccessInterstitial(false);

			this.onAdLoadSuccessInterstitial = null;
			interstitial.OnAdLoaded -= OnAdLoadedInterstitial;
			interstitial.OnAdFailedToLoad -= OnAdFailedToLoadInterstitial;
		}

		private void RequestInterstitial()
		{
			RequestInterstitial(null);
		}

		private AdRequest createAdRequest()
		{

			return new AdRequest.Builder()
//				.AddTestDevice(AdRequest.TestDeviceSimulator)
//				.AddTestDevice("8fa42327347fc830609a54a833e611ed1cc716a7")
				.AddKeyword("game")
				.TagForChildDirectedTreatment(false)
				.Build();
		}

		public void ShowBanner()
		{
			if(bannerView == null)
			{
				Debug.LogWarning("AAAdmob - ShowBanner bannerView == null ===> requestBanner");
				RequestBanner();

				bannerView.OnAdLoaded -= OnAdLoadedBanner;
				bannerView.OnAdLoaded += OnAdLoadedBanner;
			}
			else
			{
				Debug.LogWarning("AAAdmob - ShowBanner bannerView != null ----> show");
				bannerView.Show();

				GC.Collect();
				Application.targetFrameRate = 60;
			}
		}

		void OnAdLoadedBanner(object sender, EventArgs e)
		{
			Debug.LogWarning("AAAdmob - ShowBanner bannerView == null ===> requestBanner delegate -----> banner is loaded ----> show");
			bannerView.Show();

			GC.Collect();
			Application.targetFrameRate = 60;
		}

		public void HideBanner()
		{
			Debug.LogWarning("AAAdmob - HideBanner");

			if(bannerView != null)
				bannerView.Hide();

			GC.Collect();
			Application.targetFrameRate = 60;
		}
		public void DestroyBanner()
		{
			Debug.LogWarning("AAAdmob - DestroyBanner");

			if(bannerView != null)
			{
				bannerView.Destroy();
			}

			GC.Collect();
			Application.targetFrameRate = 60;
		}
		public bool IsReadyInterstitial()
		{
			bool isOK = false;

			if(interstitial != null)
			{
				Debug.LogWarning("Interstitial == null");

				isOK = interstitial.IsLoaded();
			}

			Debug.LogWarning("AAAdmob - IsReadyInterstitial : " + isOK);


//			if(!isOK)
//			{
//				CacheInterstitial();
//			}

			return isOK;
		}

		public bool IsReadyInterstitialStartup()
		{
			return IsReadyInterstitial();
		}

		public void CacheInterstitial()
		{

			if(interstitial == null)
			{
				RequestInterstitial();
				return;
			}
			else
			{
				if(interstitial != null && !interstitial.IsLoaded())
				{
					Debug.LogWarning("AAAdmob - CacheInterstitial");

					interstitial.LoadAd(createAdRequest());
				}
				else
				{
					RequestInterstitial();
				}
			}
		}

		public void CacheInterstitialStartup()
		{
			CacheInterstitial();
		}

		Action<bool> successShowInterstitial = null;


		void UnsubcribeInterstitial()
		{
			successShowInterstitial = null;
			interstitial.OnAdFailedToLoad -= OnInterstitialFailedToLoad;
			interstitial.OnAdLoaded -= OnAdLoadedInterstitial;
			interstitial.OnAdOpening -= OnAdOpeningInterstitial;
		}

		void OnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs e)
		{
			if(successShowInterstitial != null)
				successShowInterstitial(false);

			UnsubcribeInterstitial();
		}



		void OnAdOpeningInterstitial (object sender, EventArgs e)
		{
			if(successShowInterstitial != null)
				successShowInterstitial(true);

			UnsubcribeInterstitial();
		}

		public void ShowInterstitial(Action<bool> success)
		{

			print("******** ShowInterstitial ADMOB ********");


			UnsubcribeInterstitial();

			if(interstitial != null && interstitial.IsLoaded())
			{
				interstitial.OnAdFailedToLoad -= OnInterstitialFailedToLoad;
				interstitial.OnAdOpening -= OnAdOpeningInterstitial;

				interstitial.OnAdFailedToLoad += OnInterstitialFailedToLoad;
				interstitial.OnAdOpening += OnAdOpeningInterstitial;

				interstitial.Show();

				if(success != null)
					success(true);
			}
			else
			{
				RequestInterstitial((bool isSuccess) => {
					if(isSuccess)
					{
						interstitial.Show();
						if(success != null)
							success(true);
					}
					else
					{
						if(success != null)
							success(false);
					}
				});
			}
		}

		public void ShowInterstitialStartup(Action<bool> success)
		{
			ShowInterstitial(success);
		}

		public void CacheRewardedVideo()
		{
			RewardBasedVideoAd.Instance.OnAdFailedToLoad -= OnRewardedVideoFailedToLoad;
			RewardBasedVideoAd.Instance.OnAdFailedToLoad += OnRewardedVideoFailedToLoad;

			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder().Build();
			RewardBasedVideoAd.Instance.LoadAd(request,rewardedVideoID);

		}

		void OnRewardedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs e)
		{
			print("ADMOB - OnRewardedVideoFailedToLoad - error = " + e.ToString());
			Invoke("CacheRewardedVideo",10);
		}

		public bool IsReadyRewardedVideo()
		{
			bool isOK = RewardBasedVideoAd.Instance.IsLoaded();

			if(!isOK)
			{
				CacheRewardedVideo();
			}

			return isOK;
		}

		Action<bool> successRewardedVideo = null;

		void OnAdFailToLoadRewarded(object sender, AdFailedToLoadEventArgs e) {

			var f = FindObjectsOfType<AudioSource>();

			var l = new List<AudioSource>();

			if(f != null)
			{
				foreach(var a in f)
				{
					if(a.isPlaying)
					{
						l.Add(a);
						a.mute = false;
					}
				}
			}

			if(successRewardedVideo != null)
				successRewardedVideo(false);

			UnsusbcribeRewarded();
		}

		void OnAdOpeningRewarded(object sender, EventArgs e) 
		{
			var f = FindObjectsOfType<AudioSource>();

			var l = new List<AudioSource>();

			if(f != null)
			{
				foreach(var a in f)
				{
					if(a.isPlaying)
					{
						l.Add(a);
						a.mute = true;
					}
				}
			}
		}

		void OnAdClosedRewarded(object sender, EventArgs e)
		{
			var f = FindObjectsOfType<AudioSource>();

			var l = new List<AudioSource>();
	
			if(f != null)
			{
				foreach(var a in f)
				{
					if(a.isPlaying)
					{
						l.Add(a);
						a.mute = false;
					}
				}
			}

			if(successRewardedVideo != null)
				successRewardedVideo(false);

			UnsusbcribeRewarded();
		}

		void OnAdStartedRewarded(object sender, EventArgs e)
		{
			var f = FindObjectsOfType<AudioSource>();

			var l = new List<AudioSource>();

			foreach(var a in f)
			{
				if(a.isPlaying)
				{
					l.Add(a);
					a.mute = true;
				}
			}
		}

		void OnAdRewarded(object sender, Reward e) 
		{
			var f = FindObjectsOfType<AudioSource>();

			var l = new List<AudioSource>();
				
			if(f != null)
			{
				foreach(var a in f)
				{
					if(a.isPlaying)
					{
						l.Add(a);
						a.mute = false;
					}
				}
			}

			if(successRewardedVideo != null)
				successRewardedVideo(true);

			UnsusbcribeRewarded();
		}

		public void ShowRewardedVideo(Action<bool> success)
		{
			UnsusbcribeRewarded();

			this.successRewardedVideo = success;

			if(!IsReadyRewardedVideo())
			{
				if(successRewardedVideo != null)
					successRewardedVideo(false);

				successRewardedVideo = null;

				return;
			}

			var f = FindObjectsOfType<AudioSource>();

			var l = new List<AudioSource>();


			RewardBasedVideoAd.Instance.OnAdFailedToLoad += OnAdFailToLoadRewarded;
			RewardBasedVideoAd.Instance.OnAdOpening += OnAdOpeningRewarded;
			RewardBasedVideoAd.Instance.OnAdClosed += OnAdClosedRewarded;
			RewardBasedVideoAd.Instance.OnAdStarted += OnAdStartedRewarded;
			RewardBasedVideoAd.Instance.OnAdRewarded += OnAdRewarded;

			RewardBasedVideoAd.Instance.Show();
		}

		void UnsusbcribeRewarded()
		{
			GC.Collect();
			Application.targetFrameRate = 60;

			successRewardedVideo = null;

			RewardBasedVideoAd.Instance.OnAdFailedToLoad -= OnAdFailToLoadRewarded;
			RewardBasedVideoAd.Instance.OnAdOpening -= OnAdOpeningRewarded;
			RewardBasedVideoAd.Instance.OnAdClosed -= OnAdClosedRewarded;
			RewardBasedVideoAd.Instance.OnAdStarted -= OnAdStartedRewarded;
			RewardBasedVideoAd.Instance.OnAdRewarded -= OnAdRewarded;
		}
	}
}

#endif