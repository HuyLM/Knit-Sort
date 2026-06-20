using UnityEngine;

[System.Flags]
public enum AppodealAdUnit
{
    /// <summary>
    /// Init the ad units you’ve configured on the ironSource platform
    /// </summary>
    DEFAULT = 0,

    /// <summary>
    /// 
    /// </summary>
    REWARDED_VIDEO = 1,

    /// <summary>
    /// 
    /// </summary>
    INTERSTITIAL = 2,

    /// <summary>
    /// 
    /// </summary>
    BANNER = 8,
}
