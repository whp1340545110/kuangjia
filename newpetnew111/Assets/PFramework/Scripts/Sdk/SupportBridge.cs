namespace PFramework.SdkBridge
{
    using UniRx.Async;
    public interface ISupportBridge
    {
        bool IsInitialized { get; }
        string GetCountryCode();
        string GetChannel();
        string GetSystemVersion();
        string GetAppVersionName();
        string GetDeviceId();
        string GetKeychainValueWithKey(string key);
        void Rate();
        void SetKeychainValue(string key, string value);
        UniTask<string> GetRemoteConfigAsync(string key);
    }
}
