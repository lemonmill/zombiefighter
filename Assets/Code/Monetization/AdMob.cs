using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMob : MonoBehaviour
{
    public static AdMob Instance;

    private InterstitialAd interstitial;
    private RewardedAd rewarded;

    private const string interstitialID = "ca-app-pub-4754964166688469/4534670447";
    private const string rewardedID = "ca-app-pub-4754964166688469/2838445397";

    private int _rewardAmount = 100;

    #region Unity Methods

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        AdInit();

        InterstitialRequest();
        RewardedRequest();
    }

    private void OnDestroy()
    {
        InterstitialDestroy();
        RewardedDestroy();
    }

    #endregion

    #region Private Methods
    private void AdInit()
    {
        System.Action<InitializationStatus> initCompleteAction = initStatus => { };
        MobileAds.Initialize(initCompleteAction);
    }

    //Interstatial
    private void InterstitialRequest()
    {
        interstitial = new InterstitialAd(interstitialID);

        AdRequest adRequest = new AdRequest.Builder().Build();
        interstitial.LoadAd(adRequest);

        interstitial.OnAdLoaded += HandleOnInterstitialLoaded;
        interstitial.OnAdFailedToLoad += HandleOnInterstitialFailedToLoad;
        interstitial.OnAdOpening += HandleOnInterstitialOpened;
        interstitial.OnAdClosed += HandleOnInterstitialClosed;
    }
    private void InterstitialDestroy()
    {
        interstitial.OnAdLoaded -= HandleOnInterstitialLoaded;
        interstitial.OnAdFailedToLoad -= HandleOnInterstitialFailedToLoad;
        interstitial.OnAdOpening -= HandleOnInterstitialOpened;
        interstitial.OnAdClosed -= HandleOnInterstitialClosed;
        interstitial.Destroy();
    }


    //Rewarded
    private void RewardedRequest()
    {
        rewarded = new RewardedAd(rewardedID);

        AdRequest adRequest = new AdRequest.Builder().Build();
        rewarded.LoadAd(adRequest);

        this.rewarded.OnAdLoaded += HandleRewardedAdLoaded;
        this.rewarded.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        this.rewarded.OnAdOpening += HandleRewardedAdOpening;
        this.rewarded.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        this.rewarded.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewarded.OnAdClosed += HandleRewardedAdClosed;
    }

    private void RewardedDestroy()
    {
        rewarded.OnAdLoaded -= HandleOnInterstitialLoaded;
        rewarded.OnAdFailedToLoad -= HandleOnInterstitialFailedToLoad;
        rewarded.OnAdOpening -= HandleOnInterstitialOpened;
        rewarded.OnAdClosed -= HandleOnInterstitialClosed;
        rewarded.Destroy();
    }
    #endregion

    #region Public Methods
    public static void ShowInterstitial()
    {
        if (Instance.interstitial.IsLoaded())
        {
            Instance.interstitial.Show();
        }
    }
    public static void ShowRewarded(int rewardAmount = 100)
    {
        if (Instance.rewarded.IsLoaded())
        {
            Instance._rewardAmount = rewardAmount;
            Instance.rewarded.Show();
        }
    }
    #endregion

    #region Events
    //Interstatial
    private void HandleOnInterstitialClosed(object sender, EventArgs e)
    {
        InterstitialDestroy();
        InterstitialRequest();
    }

    private void HandleOnInterstitialOpened(object sender, EventArgs e) { }

    private void HandleOnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        InterstitialDestroy();
        InterstitialRequest();
    }

    private void HandleOnInterstitialLoaded(object sender, EventArgs e) { }

    //Rewarded
    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        RewardedDestroy();
        RewardedRequest();
    }

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        SavableSettings.instance.AddMoney(_rewardAmount);
    }

    private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e)
    {
        RewardedDestroy();
        RewardedRequest();
    }

    private void HandleRewardedAdOpening(object sender, EventArgs e) { }

    private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RewardedDestroy();
        RewardedRequest();
    }

    private void HandleRewardedAdLoaded(object sender, EventArgs e) { }
    #endregion
}
