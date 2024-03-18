using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;

namespace PFramework.Event
{
    public class EventArguments : Arguments
    {
        public string dispatcherName;
        public string eventKey;
        public EventArguments(string dispatcherName, string eventKey, object[] objects) : base(objects)
        {
            this.dispatcherName = dispatcherName;
            this.eventKey = eventKey;
        }
    }

    public class EventManager : ManagerBase<EventManager>
    {
        private EventManager() { }

        private Dictionary<string, EventDispathcer> _dispatchers = new Dictionary<string, EventDispathcer>();

        readonly string DefaultDispatcherName = "Default";

        public override void Initialize() { }

        public EventDispathcer Default
        {
            get
            {
                if (!_dispatchers.ContainsKey(DefaultDispatcherName))
                {
                    _dispatchers.Add(DefaultDispatcherName, new EventDispathcer(DefaultDispatcherName));
                }
                return _dispatchers[DefaultDispatcherName];
            }
        }

        public EventDispathcer GetDispatcher(string dispatcherName)
        {
            if (dispatcherName.IsNullOrEmpty())
            {
                dispatcherName = DefaultDispatcherName;
            }
            EventDispathcer dispatcher;
            if (!_dispatchers.TryGetValue(dispatcherName, out dispatcher))
            {
                dispatcher = new EventDispathcer(dispatcherName);
                _dispatchers.Add(dispatcherName, dispatcher);
            }
            return dispatcher;
        }


        public ICollection<EventDispathcer> GetDispatchers()
        {
            return _dispatchers.Values;
        }

        public void DestroyDispacther(string dispatcherName)
        {
            if (!_dispatchers.ContainsKey(dispatcherName))
            {
                var dispatcher = _dispatchers[dispatcherName];
                dispatcher.Destroy();
                _dispatchers.Remove(dispatcherName);
            }
        }

        /// <summary>
        /// 同步事件监听
        /// </summary>
        /// <param name="key"></param>
        public void RegisterEvent(string key, System.Action<EventArguments> callback, object target = null, string dispatcherName = "")
        {
            GetDispatcher(dispatcherName).RegisterEvent(key, callback, target);
        }

        /// <summary>
        /// 同步事件监听
        /// </summary>
        /// <param name="keys"></param>
        public void RegisterEvent(string[] keys, System.Action<EventArguments> callback, object target = null, string dispatcherName = "")
        {
            EventDispathcer dispathcer = GetDispatcher(dispatcherName);
            foreach (var key in keys)
            {
                dispathcer.RegisterEvent(key, callback, target);
            }
        }

        /// <summary>
        /// 移除同步事件监听
        /// </summary>
        /// <param name="key"></param>
        public void UnregisterEvent(string key, System.Action<EventArguments> callback, string dispatcherName = "")
        {
            GetDispatcher(dispatcherName).UnregisterEvent(key,callback);
        }

        /// <summary>
        /// 移除所有同步事件监听
        /// </summary>
        public void UnregisterAllEvents(object target)
        {
            GetDispatchers().ForEach((dispatcher) => { dispatcher.UnregisterEvent(target); }, (dispatcher, e) => { Debug.LogError($"{dispatcher.Name}:{e}"); });
        }

        /// 异步事件监听
        /// </summary>
        /// <param name="key"></param>
        public void RegisterAsyncEvent(string key, System.Func<EventArguments, UniTask> callback, object target = null, string dispatcherName = "")
        {
            GetDispatcher(dispatcherName).RegisterAsyncEvent(key, callback, target);
        }

        /// <summary>
        /// 异步事件监听
        /// </summary>
        /// <param name="keys"></param>
        public void RegisterAsyncEvent(string[] keys, System.Func<EventArguments, UniTask> callback, object target = null, string dispatcherName = "")
        {
            EventDispathcer dispathcer = GetDispatcher(dispatcherName);
            foreach (var key in keys)
            {
                dispathcer.RegisterAsyncEvent(key, callback, target);
            }
        }

        /// <summary>
        /// 移除异步事件监听
        /// </summary>
        /// <param name="key"></param>
        public void UnregisterAsyncEvent(string key, System.Func<EventArguments, UniTask> callback, string dispatcherName = "")
        {
            GetDispatcher(dispatcherName).UnregisterAsyncEvent(key, callback);
        }

        /// <summary>
        /// 移除所有异步事件监听
        /// </summary>
        public void UnregisterAllAsyncEvents(object target)
        {
            GetDispatchers().ForEach((dispatcher) => { dispatcher.UnregisterAsyncEvent(target); }, (dispatcher, e) => { Debug.LogError($"{dispatcher.Name}:{e}"); });
        }

        /// <summary>
        /// 移除所有事件监听
        /// </summary>
        public void UnregisterAll(object target)
        {
            UnregisterAllEvents(target);
            UnregisterAllAsyncEvents(target);
        }

        /// 抛出同步事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objects"></param>
        public void PublishEvent(string key, params object[] objects)
        {
            Default.PublishEvent(key, objects);
        }

        /// <summary>
        /// 抛出异步事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public UniTask PublishAsyncEvent(string key, params object[] objects)
        {
            return Default.PublishAsyncEvent(key, objects);
        }

        /// <summary>
        /// 抛出同步事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objects"></param>
        public  void PublishEventOn(string dispatcherName, string key, params object[] objects)
        {
            GetDispatcher(dispatcherName).PublishEvent(key, objects);
        }

        /// <summary>
        /// 抛出异步事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public UniTask PublishAsyncEventOn(string dispatcherName, string key, params object[] objects)
        {
            return GetDispatcher(dispatcherName).PublishAsyncEvent(key, objects);
        }
    }
}
