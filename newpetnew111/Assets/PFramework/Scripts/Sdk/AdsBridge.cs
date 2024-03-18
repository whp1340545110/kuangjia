using System;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;

namespace PFramework.SdkBridge
{
    /// <summary>
    /// 广告类型
    /// </summary>
    public enum BridgeAdType
    {
        Banner,
        Interstitial,
        RewardedVideo,
        Native,
        Offerwall,
        Unknown,
    }

    /// <summary>
    /// 广告回调的状态
    /// </summary>
    public enum BridgeAdState
    {
        Ready,
        WillShow,
        Shown,
        Opened,
        Clicked,
        Closed,
        Error,
        Playended,
        Rewarded,
        Unknown
    }

    public partial class AdSettings
    {
        public RectOffset nativeRect;
        public int bannerHeight;
        public AdSettings(int bannerheight)
        {
            this.bannerHeight = bannerheight;
        }
        public AdSettings(int top,int buttom,int left,int right)
        {
            nativeRect = new RectOffset(left, right, top, buttom);
        }
    }

    /// <summary>
    /// 广告回调的参数
    /// </summary>
    public class AdsStateMessage
    {
        public BridgeAdType adType;
        public BridgeAdState adState;
        public string placement;
        public int errorCode;
        public string errorMessage;
        public AdsStateMessage(BridgeAdType adType,BridgeAdState adState,string placement = "",int errorCode = 0,string errorMessage = "")
        {
            this.adType = adType;
            this.adState = adState;
            this.placement = placement;
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
        }
    }

    /// <summary>
    /// 取消毁掉监听的token
    /// </summary>
    public class AdListenerToken
    {
        public BridgeAdType adType;
        public string placement;
        public System.Action<AdsStateMessage> onAdMessage;
    }

    public interface IAdsBridge
    {
        bool IsInitialized { get; }
        bool IsReady(string placement,BridgeAdType adType);
        void LoadAd(string placement, BridgeAdType adType, AdSettings settings = null);
        void ShowAd(string placement, BridgeAdType adType, AdSettings settings = null);
        void CloseAd(string placement, BridgeAdType adType = BridgeAdType.Banner);

        /// <summary>
        /// 注册广告回调,placement填空，adType填Unknown为监听所有广告
        /// </summary>
        /// <returns></returns>
        AdListenerToken RegisterAdState(string placement,BridgeAdType adType, System.Action<AdsStateMessage> onAdMessage);
        /// <summary>
        /// 取消广告注册
        /// </summary>
        /// <param name="component"></param>
        void UnRegisterAdState(AdListenerToken token);
        void Update();
    }

    public abstract class AdsBaseBridge : IAdsBridge
    {
        #region Interface
        public abstract bool IsInitialized { get; }

        public bool IsReady(string placement, BridgeAdType adType)
        {
            if (IsInitialized)
            {
                switch (adType)
                {
                    case BridgeAdType.Banner:
                        return IsInitialized;
                    case BridgeAdType.RewardedVideo:
                        return IsRewardedVideoReady(placement);
                    case BridgeAdType.Interstitial:
                        return IsInterstitialReady(placement);
                    case BridgeAdType.Native:
                        return IsNativeReady(placement);
                    default:
                        return false;
                }
            }
            return false;
        }


        public void LoadAd(string placement, BridgeAdType adType, AdSettings settings = null)
        {
            if (!IsReady(placement, adType))
            {
                switch (adType)
                {
                    case BridgeAdType.RewardedVideo:
                        LoadRewardedVideo(placement, settings);
                        break;
                    case BridgeAdType.Interstitial:
                        LoadInterstitial(placement, settings);
                        break;
                    case BridgeAdType.Native:
                        LoadNative(placement, settings);
                        break;
                }
            }
        }

        public async UniTask LoadAd(string placement, BridgeAdType adType, float delaySeconds)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds));
            LoadAd(placement, adType);
        }


        public void ShowAd(string placement, BridgeAdType adType, AdSettings settings = null)
        {
            Debug.Log($"placement ready = {IsReady(placement, adType)}");
            if (IsReady(placement, adType))
            {
                EnqueueAdState(new AdsStateMessage(adType, BridgeAdState.WillShow, placement));
                switch (adType)
                {
                    case BridgeAdType.Banner:
                        ShowBanner(placement, settings);
                        break;
                    case BridgeAdType.RewardedVideo:
                        ShowRewardedVideo(placement, settings);
                        break;
                    case BridgeAdType.Interstitial:
                        ShowInterstitial(placement, settings);
                        break;
                    case BridgeAdType.Native:
                        ShowNative(placement, settings);
                        break;
                    default:
                        break;
                }
            }
        }

        public void CloseAd(string placement, BridgeAdType adType = BridgeAdType.Banner)
        {
            if (IsInitialized)
            {
                switch (adType)
                {
                    case BridgeAdType.Native:
                        RemoveNative(placement);
                        break;
                    case BridgeAdType.Banner:
                        RemoveBanner(placement);
                        break;
                    default:
                        break;
                }
            }
        }

        public AdListenerToken RegisterAdState(string placement, BridgeAdType adType, System.Action<AdsStateMessage> onAdMessage)
        {
            var token = new AdListenerToken()
            {
                placement = placement,
                adType = adType,
                onAdMessage = onAdMessage
            };


            if (IsReady(placement, adType))
            {
                onAdMessage?.Invoke(new AdsStateMessage(adType, BridgeAdState.Ready));
            }

            _additionTokens.Add(token);
            return token;
        }

        public void UnRegisterAdState(AdListenerToken token)
        {
            _removalTokens.Add(token);
        }


        public void Update()
        {
            InvokeTokenListeners();
        }

        #endregion
        private Queue<AdsStateMessage> _adMessageQueue = new Queue<AdsStateMessage>();
        private readonly List<AdListenerToken> _adListenerTokens = new List<AdListenerToken>();
        private readonly List<AdListenerToken> _additionTokens = new List<AdListenerToken>();
        private readonly List<AdListenerToken> _removalTokens = new List<AdListenerToken>();

        private AdsStateMessage _closeState;
        private bool _isRewarded = false;

        protected void EnqueueAdState(AdsStateMessage adsStateMessage)
        {
            //因为rv的Reward和close事件前后顺序不固定，所以这里要缓存信息，保证Reward在Close之前
            if (adsStateMessage.adType == BridgeAdType.RewardedVideo)
            {
                if (adsStateMessage.adState == BridgeAdState.Rewarded)
                {
                    if (_closeState != null)
                    {
                        _adMessageQueue.Enqueue(adsStateMessage);
                        _adMessageQueue.Enqueue(_closeState);
                        _closeState = null;
                    }
                    else
                    {
                        _isRewarded = true;
                        _adMessageQueue.Enqueue(adsStateMessage);
                    }
                }
                else if (adsStateMessage.adState == BridgeAdState.Closed)
                {
                    if (_isRewarded)
                    {
                        _adMessageQueue.Enqueue(adsStateMessage);
                        _isRewarded = false;

                    }
                    else
                    {
                        //有上一个残留的，就先触发，记录该次close
                        if (_closeState != null)
                        {
                            _adMessageQueue.Enqueue(_closeState);
                        }
                        _closeState = adsStateMessage;
                        DelayForCloseState(_closeState).Forget();
                    }
                }
                else
                {
                    _adMessageQueue.Enqueue(adsStateMessage);
                }
            }
            else
            {
                _adMessageQueue.Enqueue(adsStateMessage);
            }
        }


        private async UniTask DelayForCloseState(AdsStateMessage adsStateMessage)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3));
            //3秒后还是没得Reward且CloseState依然是当前这个，就触发关闭
            if (_closeState == adsStateMessage)
            {
                _adMessageQueue.Enqueue(_closeState);
                _closeState = null;
            }
        }

        private void AddTokensInMainLoop()
        {
            foreach (var listenner in _additionTokens)
            {
                _adListenerTokens.Add(listenner);
            }
            _additionTokens.Clear();
        }

        private void RemoveTokensInMainLoop()
        {
            for (int i = _removalTokens.Count - 1; i >= 0; i--)
            {
                var listenner = _removalTokens[i];
                _adListenerTokens.Remove(listenner);
                _removalTokens.RemoveAt(i);
            }
        }

        private void InvokeTokenListeners()
        {
            lock (_adMessageQueue)
            {
                //把增添回调放到循环遍历的外部，放置在遍历的回调中又对回调进行增添操作导致List迭代器报错
                AddTokensInMainLoop();
                while (_adMessageQueue.Count > 0)
                {
                    var adMessage = _adMessageQueue.Dequeue();

                    if (adMessage.adState == BridgeAdState.Closed)
                    {
                        LoadAd(adMessage.placement, adMessage.adType);
                    }
                    else if (adMessage.adState == BridgeAdState.Error)
                    {
                        LoadAd(adMessage.placement, adMessage.adType, 10).Forget();
                    }

                    Debug.LogFormat("placement {0},state {1}", adMessage.placement, adMessage.adState);
                    foreach (var listenner in _adListenerTokens)
                    {
                        if (listenner.placement.ToString().Equals(adMessage.placement) && listenner.adType == adMessage.adType)
                        {
                            try
                            {
                                listenner.onAdMessage?.Invoke(adMessage);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                                //UnRegisterAdState(component);
                            }
                        }
                        //这个为监听所有广告回调的类型
                        else if (listenner.placement == string.Empty && listenner.adType == BridgeAdType.Unknown)
                        {
                            try
                            {
                                listenner.onAdMessage?.Invoke(adMessage);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                                //UnRegisterAdState(component);
                            }
                        }
                    }
                }
                RemoveTokensInMainLoop();
            }
        }

        protected abstract void LoadRewardedVideo(string placement, AdSettings settings = null);

        protected abstract bool IsRewardedVideoReady(string placement);

        protected abstract void ShowRewardedVideo(string placement, AdSettings settings = null);


        protected abstract void LoadInterstitial(string placement, AdSettings settings = null);

        protected abstract bool IsInterstitialReady(string placement);

        protected abstract void ShowInterstitial(string placement, AdSettings settings = null);


        protected abstract void ShowBanner(string placement, AdSettings settings = null);

        protected abstract void RemoveBanner(string placement);


        protected abstract void LoadNative(string placement, AdSettings settings = null);

        protected abstract bool IsNativeReady(string placement);

        protected abstract void ShowNative(string placement, AdSettings settings = null);

        protected abstract void RemoveNative(string placement);
    }
}
