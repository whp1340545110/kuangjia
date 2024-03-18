
namespace PFramework.SdkBridge
{
    using UnityEngine;
    using UniRx.Async;
    using System;
    using UniRx;
    public partial class SdkBridgeManager : ManagerBase<SdkBridgeManager>
    {
        private IAdsBridge _adsBridge;

        private IEventBridge _eventBridge;

        private ISupportBridge _supportBridge;

        public bool IsAdsInitialized => _adsBridge != null && _adsBridge.IsInitialized;

        public bool IsEventInitialized => _eventBridge != null && _eventBridge.IsInitialized;

        public bool IsSupportInitialized => _supportBridge != null && _supportBridge.IsInitialized;

        private SdkBridgeManager(){ }

        public override void Initialize()
        {
            CreateDefaultBridge();

            Observable.EveryUpdate().Subscribe(Update);
        }

        private void CreateDefaultBridge()
        {
            if (_adsBridge == null)
            {
                _adsBridge = new DefaultAdsBridge();
            }

            if (_supportBridge == null)
            {
                _supportBridge = new DefaultSurpportBridge();
            }

            if (_eventBridge == null)
            {
                _eventBridge = new DefaultEventBridge();
            }
        }

        public void SetEventBridge(IEventBridge eventBridge)
        {
            _eventBridge = eventBridge;
        }

        public void SetAdsBridge(IAdsBridge adsBridge)
        {
            _adsBridge = adsBridge;
        }

        public void SetSupportBridge(ISupportBridge supportBridge)
        {
            _supportBridge = supportBridge;
        }

#region Ads
        public void LoadAd(string placement, BridgeAdType adType,AdSettings settings = null)
        {
            _adsBridge.LoadAd(placement, adType, settings);
        }

        public bool IsReady(string placement, BridgeAdType adType)
        {
            return _adsBridge.IsReady(placement, adType);
        }

        public void ShowAd(string placement, BridgeAdType adType, AdSettings settings = null)
        {
            _adsBridge.ShowAd(placement, adType, settings);
        }

        public void CloseAd(string placement, BridgeAdType adType = BridgeAdType.Banner)
        {
            _adsBridge.CloseAd(placement, adType);
        }

        public AdListenerToken RegisterAdState(string placement, BridgeAdType adType, Action<AdsStateMessage> onAdMessage)
        {
            return _adsBridge.RegisterAdState(placement, adType, onAdMessage);
        }

        public void UnRegisterAdState(AdListenerToken listenner)
        {
            _adsBridge.UnRegisterAdState(listenner);
        }
#endregion

#region event
        public void LogEvent(string key, object obj = null,EventPlatform eventPlatform = EventPlatform.All)
        {
            _eventBridge.LogEvent(key, obj, eventPlatform);
        }
#endregion

#region Support
        public string GetCountryCode()
        {
            return _supportBridge.GetCountryCode();
        }

        public string GetChannel()
        {
            return _supportBridge.GetChannel();
        }

        public string GetSystemVersion()
        {
            return _supportBridge.GetSystemVersion();
        }

        public string GetAppVersionName()
        {
            return _supportBridge.GetAppVersionName();
        }

        public string GetDeviceId()
        {
            return _supportBridge.GetDeviceId();
        }

        public string GetKeychainValueWithKey(string key)
        {
            return _supportBridge.GetKeychainValueWithKey(key);
        }

        public void SetKeychainValue(string key, string value)
        {
            _supportBridge.SetKeychainValue(key, value);
        }

        public async UniTask<string> GetRemoteConfigAsync(string key)
        {
            return await _supportBridge.GetRemoteConfigAsync(key);
        }

        public async UniTask<int> GetIntRemoteConfigAsync(string key, int defaultValue = 0)
        {
            int result;
            var remoteValueString = await _supportBridge.GetRemoteConfigAsync(key);
            if (!int.TryParse(remoteValueString, out result))
            {
                result = defaultValue;
            }
            Debug.LogFormat($"Get remote config key = {key}, value = {remoteValueString}");
            return result;
        }

        public void Rate()
        {
            _supportBridge.Rate();
        }
#endregion

        private void Update(long frame)
        {
            _adsBridge.Update();
        }
    }
}
