namespace PFramework
{
    using System.Collections.Generic;
    using PFramework.Event;
    using UniRx.Async;
    public static partial class Peach
    {
        public static EventManager EventMgr => EventManager.Instance;

        /// <summary>
        /// 同步事件监听
        /// </summary>
        /// <param name="key"></param>
        public static void RegisterEvent(string key, System.Action<EventArguments> callback, object target = null, string dispatcherName = "")
        {
            EventMgr.RegisterEvent(key, callback, target, dispatcherName);
        }

        /// <summary>
        /// 同步事件监听
        /// </summary>
        /// <param name="keys"></param>
        public static void RegisterEvent(string[] keys, System.Action<EventArguments> callback, object target = null, string dispatcherName = "")
        {
            EventMgr.RegisterEvent(keys, callback, target, dispatcherName);
        }

        /// <summary>
        /// 移除同步事件监听
        /// </summary>
        /// <param name="key"></param>
        public static void UnregisterEvent(string key, System.Action<EventArguments> callback, string dispatcherName = "")
        {
            EventMgr.UnregisterEvent(key, callback, dispatcherName);
        }

        /// <summary>
        /// 移除所有同步事件监听
        /// </summary>
        public static void UnregisterAllEvents(object target)
        {
            EventMgr.UnregisterAllEvents(target);
        }

        /// 异步事件监听
        /// </summary>
        /// <param name="key"></param>
        public static void RegisterAsyncEvent(string key, System.Func<EventArguments, UniTask> callback, object target = null, string dispatcherName = "")
        {
            EventMgr.RegisterAsyncEvent(key, callback, target, dispatcherName);
        }

        /// <summary>
        /// 异步事件监听
        /// </summary>
        /// <param name="keys"></param>
        public static void RegisterAsyncEvent(string[] keys, System.Func<EventArguments, UniTask> callback, object target = null, string dispatcherName = "")
        {
            EventMgr.RegisterAsyncEvent(keys, callback, target, dispatcherName);
        }

        /// <summary>
        /// 移除异步事件监听
        /// </summary>
        /// <param name="key"></param>
        public static void UnregisterAsyncEvent(string key, System.Func<EventArguments, UniTask> callback, string dispatcherName = "")
        {
            EventMgr.UnregisterAsyncEvent(key, callback, dispatcherName);
        }

        /// <summary>
        /// 移除所有异步事件监听
        /// </summary>
        public static void UnregisterAllAsyncEvents(object target)
        {
            EventMgr.UnregisterAllAsyncEvents(target);
        }

        /// <summary>
        /// 移除所有事件监听
        /// </summary>
        public static void UnregisterAll(object target)
        {
            EventMgr.UnregisterAll(target);
        }

        /// 抛出同步事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objects"></param>
        public static void PublishEvent(string key, params object[] objects)
        {
            EventMgr.PublishEvent(key, objects);
        }

        /// <summary>
        /// 抛出异步事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static UniTask PublishAsyncEvent(string key, params object[] objects)
        {
            return EventMgr.PublishAsyncEvent(key, objects);
        }

        /// <summary>
        /// 抛出同步事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objects"></param>
        public static void PublishEventOn(string dispatcherName, string key, params object[] objects)
        {
            EventMgr.PublishEventOn(dispatcherName ,key, objects);
        }

        /// <summary>
        /// 抛出异步事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static UniTask PublishAsyncEventOn(string dispatcherName, string key, params object[] objects)
        {
            return EventMgr.PublishAsyncEventOn(dispatcherName, key, objects);
        }


        private class SingleEventDelegater
        {
            private bool _isPublished = false;
            private EventArguments _eventArgs;
            private string _eventKey;

            private Queue<SingleEventDelegater> _poolQuene = new Queue<SingleEventDelegater>();
            public SingleEventDelegater Create(string eventKey, bool isAsync, string dispatcherName)
            {
                var obj = _poolQuene.Dequeue();
                if(obj == null)
                {
                    obj = new SingleEventDelegater(eventKey, isAsync, dispatcherName);
                }
                return obj;
            }

            public SingleEventDelegater(string eventKey,bool isAsync, string dispatcherName)
            {
                _eventKey = eventKey;
                if (isAsync)
                {
                    RegisterAsyncEvent(eventKey, OnAsyncEvent, this, dispatcherName);
                }
                else
                {
                    RegisterEvent(eventKey, OnEvent, this, dispatcherName);
                }
            }

            public async UniTask<EventArguments> WaitForPublish(PlayerLoopTiming playerLoopTiming)
            {
                await UniTask.WaitUntil(() => _isPublished, playerLoopTiming);
                var args = _eventArgs;
                _eventArgs = null;
                _isPublished = false;
                _poolQuene.Enqueue(this);
                return args;
            }

            private void OnEvent(EventArguments arguments)
            {
                _isPublished = true;
                _eventArgs = arguments;
                UnregisterEvent(_eventKey, OnEvent);
            }

            private UniTask OnAsyncEvent(EventArguments arguments)
            {
                _isPublished = true;
                _eventArgs = arguments;
                UnregisterAsyncEvent(_eventKey, OnAsyncEvent);
                return default(UniTask);
            }
        }

        /// <summary>
        /// 等待一次同步事件抛出
        /// </summary>
        /// <param name="eventKey"></param>
        /// <param name="playerLoopTiming"></param>
        /// <returns></returns>
        public static async UniTask<EventArguments> WaitForEventPublish(string eventKey, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update, string dispatcherName = "")
        {
            var delegater = new SingleEventDelegater(eventKey, false, dispatcherName);
            return await delegater.WaitForPublish(playerLoopTiming);
        }

        /// <summary>
        /// 等待一次异步事件抛出
        /// </summary>
        /// <param name="eventKey"></param>
        /// <param name="playerLoopTiming"></param>
        /// <returns></returns>
        public static async UniTask<EventArguments> WaitForAsyncEventPublish(string eventKey, PlayerLoopTiming playerLoopTiming = PlayerLoopTiming.Update, string dispatcherName = "")
        {
            var delegater = new SingleEventDelegater(eventKey, true, dispatcherName);
            return await delegater.WaitForPublish(playerLoopTiming);
        }
    }
}
