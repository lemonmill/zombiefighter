using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestAdButtonAdapter : MonoBehaviour
{
    public enum AdType
    {
        Interstatial,
        Rewarded
    }
    public AdType adType;
    public int rewardAmount;

    public void ShowAd()
    {
        switch (adType)
        {
            case AdType.Interstatial:
                AdMob.ShowInterstitial();
                break;
            case AdType.Rewarded:
                AdMob.ShowRewarded(rewardAmount);
                break;
        }
    }
}
