namespace PFramework.SdkBridge
{
    public class DefaultAdsBridge : AdsBaseBridge
    {
        public override bool IsInitialized => true;

        protected override bool IsInterstitialReady(string placement)
        {
            return true;
        }

        protected override bool IsRewardedVideoReady(string placement)
        {
            return true;
        }

        protected override void LoadInterstitial(string placement, AdSettings settings = null)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Interstitial, BridgeAdState.Ready, placement));
        }

        protected override void LoadRewardedVideo(string placement, AdSettings settings = null)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.RewardedVideo, BridgeAdState.Ready, placement));
        }

        protected override void ShowBanner(string placement, AdSettings settings = null)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Banner, BridgeAdState.Opened, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Banner, BridgeAdState.Shown, placement));
        }

        protected override void RemoveBanner(string placement)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Banner, BridgeAdState.Closed, placement));
        }

        protected override void ShowInterstitial(string placement, AdSettings settings = null)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Interstitial, BridgeAdState.Opened, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Interstitial, BridgeAdState.Shown, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Interstitial, BridgeAdState.Playended, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Interstitial, BridgeAdState.Closed, placement));
        }

        protected override void ShowRewardedVideo(string placement, AdSettings settings = null)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.RewardedVideo, BridgeAdState.Opened, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.RewardedVideo, BridgeAdState.Shown, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.RewardedVideo, BridgeAdState.Playended, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.RewardedVideo, BridgeAdState.Rewarded, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.RewardedVideo, BridgeAdState.Closed, placement));
        }

        protected override void LoadNative(string placement, AdSettings settings = null)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Native, BridgeAdState.Ready, placement));
        }

        protected override bool IsNativeReady(string placement)
        {
            return false;
        }

        protected override void ShowNative(string placement, AdSettings settings = null)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Native, BridgeAdState.Opened, placement));
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Native, BridgeAdState.Shown, placement));
        }

        protected override void RemoveNative(string placement)
        {
            EnqueueAdState(new AdsStateMessage(BridgeAdType.Native, BridgeAdState.Closed, placement));
        }
    }
}
