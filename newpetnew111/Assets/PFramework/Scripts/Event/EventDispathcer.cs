using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
using System;

namespace PFramework.Event
{
    public class EventDispathcer
    {
        private class EventInfo
        {
            internal int targetHash;
            internal Action<EventArguments> OnSyncEvent;
            internal Func<EventArguments, UniTask> OnAsyncEvent;
            internal string eventKey;
            internal EventInfo(string eventKey, Action<EventArguments> onSyncEvent, object target = null)
            {
                this.eventKey = eventKey;
                this.targetHash = target != null ? target.GetHashCode() : 0;
                this.OnSyncEvent = onSyncEvent;
            }
            internal EventInfo(string eventKey, Func<EventArguments, UniTask> onAsyncEvent, object target = null)
            {
                this.eventKey = eventKey;
                this.targetHash = target != null ? target.GetHashCode() : 0;
                this.OnAsyncEvent = onAsyncEvent;
            }
        }

        private class EventInfoList
        {
            public readonly List<EventInfo> list = new List<EventInfo>();

            public void SyncPublish(string dispatcherName,string eventKey, params object[] parameters)
            {
                List<EventInfo> temp = new List<EventInfo>(list);
                temp.ForIndex((index, info) =>
                {
                    info?.OnSyncEvent?.Invoke(new EventArguments(dispatcherName, eventKey, parameters));
                }, (index, e) =>
                {
                    Debug.LogError(e);
                    RemoveAt(index);
                });
            }

            public UniTask AsyncPublish(string dispatcherName, string eventKey, params object[] parameters)
            {
                List<UniTask> tasks = new List<UniTask>();
                List<EventInfo> temp = new List<EventInfo>(list);
                temp.ForIndex((index, info) =>
                {
                    if(info != null && info.OnAsyncEvent != null)
                    {
                        var task = info.OnAsyncEvent.Invoke(new EventArguments(dispatcherName, eventKey, parameters));
                        tasks.Add(task);
                    }
                }, (index, e) =>
                {
                    Debug.LogError(e);
                    RemoveAt(index);
                });
                return UniTask.WhenAll(tasks);
            }


            public void Add(EventInfo eventInfo)
            {
                list.Add(eventInfo);
            }

            public void RemoveTarget(object target)
            {
                if(target != null)
                {
                    list.ForIndex((index, info) =>
                    {
                        if (info.targetHash == target.GetHashCode())
                        {
                            //这个地方不能图方便调用RemoveAt，否则要引起多次重新排序
                            list[index] = null;
                        }
                    });
                    RemoveNull();
                }
            }

            public void RemoveAction(Action<EventArguments> action)
            {
                var actionIndex = list.FindIndex((info) => info.OnSyncEvent == action);
                if (actionIndex != -1)
                {
                    RemoveAt(actionIndex);
                }
            }

            public void RemoveAction(Func<EventArguments,UniTask> action)
            {
                var actionIndex = list.FindIndex((info) => info.OnAsyncEvent == action);
                if (actionIndex != -1)
                {
                    RemoveAt(actionIndex);
                }
            }

            private void RemoveAt(int index)
            {
                list.RemoveAt(index);
            }

            private void RemoveNull()
            {
                list.RemoveAll((info) => info == null);
            }
        }

        private Dictionary<string, EventInfoList> _eventDics = new Dictionary<string, EventInfoList>();
        private Dictionary<string, EventInfoList> _asyncEventDics = new Dictionary<string, EventInfoList>();

        public string Name { get; private set; }

        public EventDispathcer(string name)
        {
            Name = name;
        }

        public void RegisterEvent(string eventKey, Action<EventArguments> onRecieve, object target = null)
        {
            if (!_eventDics.ContainsKey(eventKey))
            {
                _eventDics.Add(eventKey, new EventInfoList());
            }
            _eventDics[eventKey].Add(new EventInfo(eventKey, onRecieve, target));
        }

        public void PublishEvent(string eventKey, params object[] parameters)
        {
            if (_eventDics.ContainsKey(eventKey))
            {
                _eventDics[eventKey].SyncPublish(Name,eventKey,parameters);
            }
        }

        public void UnregisterEvent(string eventKey, Action<EventArguments> onRecieve)
        {
            if (_eventDics.TryGetValue(eventKey, out EventInfoList infoList))
            {
                infoList.RemoveAction(onRecieve);
            }
        }

        public void UnregisterEvent(object target)
        {
            foreach (var infoList in _eventDics)
            {
                infoList.Value.RemoveTarget(target);
            }
        }

        public void RegisterAsyncEvent(string eventKey, Func<EventArguments, UniTask> onRecieve, object target = null)
        {
            if (!_asyncEventDics.ContainsKey(eventKey))
            {
                _asyncEventDics.Add(eventKey, new EventInfoList());
            }
            _asyncEventDics[eventKey].Add(new EventInfo(eventKey, onRecieve, target));
        }

        public UniTask PublishAsyncEvent(string eventKey, params object[] parameters)
        {
            UniTask task = default(UniTask);
            if (_asyncEventDics.ContainsKey(eventKey))
            {
                task = _asyncEventDics[eventKey].AsyncPublish(Name, eventKey, parameters);
            }
            return task;
        }

        public void UnregisterAsyncEvent(string eventKey, Func<EventArguments, UniTask> onRecieve)
        {
            if (_asyncEventDics.TryGetValue(eventKey, out EventInfoList infoList))
            {
                infoList.RemoveAction(onRecieve);
            }
        }

        public void UnregisterAsyncEvent(object target)
        {
            foreach (var infoList in _asyncEventDics)
            {
                infoList.Value.RemoveTarget(target);
            }
        }

        public void Destroy()
        {
            _eventDics = null;
            _asyncEventDics = null;
        }
    }
}
