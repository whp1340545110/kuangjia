using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;

namespace PFramework
{
    namespace Timers
    {
        public class TimerManager : ManagerBase<TimerManager>
        {
            private TimerManager(){ }

            public const string DefaultTimerName = "default";

            private Dictionary<string, Timer> _timers = new Dictionary<string, Timer>();

            public GameObject gameObject { get; private set; }

            public override void Initialize()
            {
                gameObject = new GameObject("Peach.TimerManager");
                GameObject.DontDestroyOnLoad(gameObject);
            }

            private Timer GetOrCreateTimer(string timerName)
            {
                if (!_timers.TryGetValue(timerName, out Timer timer))
                {
                    timer = gameObject.AddComponent<Timer>();
                    timer.timerName = timerName;
                    _timers.Add(timerName, timer);
                }
                return timer;
            }

            public Timer GetTimer(string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName);
            }

            public TimerInfo Interval(float seconds, Action<int> onInterval, int count = -1, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).Interval(seconds, onInterval, count);
            }

            public TimerInfo Interval(TimeSpan interval, Action<int> onInterval, int count = -1, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).Interval(interval, onInterval, count);
            }

            public TimerInfo Interval(float seconds, float delay,Action<int> onInterval, int count = -1, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).Interval(seconds, delay, onInterval, count);
            }

            public TimerInfo Interval(TimeSpan interval, TimeSpan delay, Action<int> onInterval, int count = -1, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).Interval(interval, delay, onInterval, count);
            }

            public TimerInfo IntervalFrame(TimeSpan delay, Action<float> onInterval, int count = -1, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).IntervalFrame((float)delay.TotalSeconds, onInterval, count);
            }

            public TimerInfo IntervalFrame(float delay, Action<float> onInterval, int count = -1, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).IntervalFrame(delay, onInterval, count);
            }

            public TimerInfo Delay(float seconds, Action onDelay, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).Delay(seconds, onDelay);
            }

            public TimerInfo Delay(TimeSpan timeSpan, Action onDelay, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).Delay(timeSpan, onDelay);
            }

            public UniTask DelayTask(float seconds, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).DelayTask(seconds);
            }

            public UniTask DelayTask(TimeSpan timeSpan, string timerName = DefaultTimerName)
            {
                return GetOrCreateTimer(timerName).DelayTask(timeSpan);
            }

            public void RemoveTime(string name)
            {
                _timers.Remove(name);

            }
        }
    }
}

