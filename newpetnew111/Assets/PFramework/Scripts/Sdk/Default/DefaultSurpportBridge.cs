namespace PFramework.SdkBridge
{
    using UniRx.Async;
    using UnityEngine;
#pragma warning disable 1998
    public class DefaultSurpportBridge : ISupportBridge
    {
        public bool IsInitialized => true;

        public string GetAppVersionName()
        {
            return Application.version;
        }

        public string GetChannel()
        {
            return "organic";
        }

        public string GetCountryCode()
        {
            return "US";
        }

        public string GetDeviceId()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public string GetSystemVersion()
        {
            return Application.unityVersion;
        }

        public string GetKeychainValueWithKey(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public void SetKeychainValue(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public async UniTask<string> GetRemoteConfigAsync(string key)
        {
            return "";
        }

        public void Rate()
        {
            
        }
    }
#pragma warning restore 1998
}
