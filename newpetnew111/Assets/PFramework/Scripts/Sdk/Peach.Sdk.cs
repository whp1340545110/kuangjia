namespace PFramework
{
    using System;
    using PFramework.SdkBridge;
    using UniRx.Async;
    using UnityEngine;
    public static partial class Peach
    {
        public static SdkBridgeManager SdkMgr => SdkBridgeManager.Instance;

        #region Ads
        public static void LoadAd(string placement, BridgeAdType adType, AdSettings settings = null)
        {
            SdkMgr.LoadAd(placement, adType, settings);
        }

        public static bool IsAdReady(string placement, BridgeAdType adType)
        {
            return SdkMgr.IsReady(placement, adType);
        }

        public static void ShowAd(string placement, BridgeAdType adType, AdSettings settings = null)
        {
            SdkMgr.ShowAd(placement, adType, settings);
        }

        public static void CloseAd(string placement, BridgeAdType adType = BridgeAdType.Banner)
        {
            SdkMgr.CloseAd(placement, adType);
        }

        public static AdListenerToken RegisterAdState(string placement, BridgeAdType adType, Action<AdsStateMessage> onAdMessage)
        {
            return SdkMgr.RegisterAdState(placement, adType, onAdMessage);
        }

        public static void UnRegisterAdState(AdListenerToken listenner)
        {
            SdkMgr.UnRegisterAdState(listenner);
        }

        public static AdListenerToken RegesterAdState(Action<AdsStateMessage> onAdMessage)
        {
            return SdkMgr.RegisterAdState(string.Empty, BridgeAdType.Unknown, onAdMessage);
        }
        #endregion

        #region event
        public static void LogEvent(string key, object obj = null, EventPlatform eventPlatform = EventPlatform.All)
        {
            SdkMgr.LogEvent(key, obj, eventPlatform);
        }
        #endregion

        #region Support
        public static string GetCountryCode()
        {
            return SdkMgr.GetCountryCode();
        }

        public static string GetChannel()
        {
            return SdkMgr.GetChannel();
        }

        public static string GetSystemVersion()
        {
            return SdkMgr.GetSystemVersion();
        }

        public static string GetAppVersionName()
        {
            return SdkMgr.GetAppVersionName();
        }

        public static string GetDeviceId()
        {
            return SdkMgr.GetDeviceId();
        }

        public static string GetKeychainValueWithKey(string key)
        {
            return SdkMgr.GetKeychainValueWithKey(key);
        }

        public static void SetKeychainValue(string key, string value)
        {
            SdkMgr.SetKeychainValue(key, value);
        }

        public static async UniTask<string> GetRemoteConfigAsync(string key)
        {
            return await SdkMgr.GetRemoteConfigAsync(key);
        }

        public static async UniTask<int> GetIntRemoteConfigAsync(string key, int defaultValue = 0)
        {
            int result;
            var remoteValueString = await SdkMgr.GetRemoteConfigAsync(key);
            if (!int.TryParse(remoteValueString, out result))
            {
                result = defaultValue;
            }
            Debug.LogFormat($"Get remote config key = {key}, value = {remoteValueString}");
            return result;
        }

        public static void Rate()
        {
            SdkMgr.Rate();
        }
        #endregion
    }
}