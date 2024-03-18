namespace PFramework.SdkBridge
{
    public enum EventPlatform
    {
        All,
        /// <summary>
        /// Appflyer
        /// </summary>
        AF,
        /// <summary>
        /// GameAnalysis
        /// </summary>
        GA,
        /// <summary>
        /// FireBase
        /// </summary>
        FB,
        /// <summary>
        /// Umeng
        /// </summary>
        UM
    }
    public interface IEventBridge
    {
        bool IsInitialized { get; }
        /// <summary>
        /// 这里obj是一个可序列化对象，或者string(um)或者数字(ga)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="eventPlatform"></param>
        void LogEvent(string key,object obj,EventPlatform eventPlatform = EventPlatform.All);
    }

    public abstract class EventBaseBridge : IEventBridge
    {
        public void LogEvent(string key, object obj, EventPlatform eventPlatform = EventPlatform.All)
        {
            switch (eventPlatform)
            {
                case EventPlatform.All:
                    LogAllPlatformEvent(key, obj);
                    break;
                case EventPlatform.AF:
                    LogAFEvent(key, obj);
                    break;
                case EventPlatform.GA:
                    LogGAEvent(key, obj);
                    break;
                case EventPlatform.FB:
                    LogFBEvent(key, obj);
                    break;
                case EventPlatform.UM:
                    LogUMEvent(key, obj);
                    break;
            }
        }

        public abstract bool IsInitialized { get; }

        protected abstract void LogAllPlatformEvent(string key, object obj);

        protected abstract void LogAFEvent(string key, object obj);

        protected abstract void LogGAEvent(string key, object obj);

        protected abstract void LogFBEvent(string key, object obj);

        protected abstract void LogUMEvent(string key, object obj);

    }
}
