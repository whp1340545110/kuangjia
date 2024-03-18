namespace PFramework
{
    using System;
    using PFramework.Timers;
    using UniRx.Async;
    public static partial class Peach
    {
        public static TimerManager TimerMgr => TimerManager.Instance;

        public static Timer GetTimer(string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.GetTimer(timerName);
        }

        public static TimerInfo Interval(float seconds, float delay, Action<int> onInterval, int count = -1, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.Interval(seconds, delay, onInterval, count, timerName);
        }

        public static TimerInfo Interval(float seconds, Action<int> onInterval, int count = -1, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.Interval(seconds, onInterval, count, timerName);
        }

        public static TimerInfo Interval(TimeSpan interval, Action<int> onInterval, int count = -1, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.Interval(interval, onInterval, count, timerName);
        }

        public static TimerInfo Interval(TimeSpan interval, TimeSpan delay, Action<int> onInterval, int count = -1, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.Interval(interval, delay, onInterval, count, timerName);
        }

        public static TimerInfo IntervalFrame(TimeSpan delay, Action<float> onInterval, int count = -1, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.IntervalFrame((float)delay.TotalSeconds, onInterval, count, timerName);
        }

        public static TimerInfo IntervalFrame(float delay, Action<float> onInterval, int count = -1, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.IntervalFrame(delay, onInterval, count, timerName);
        }

        public static TimerInfo Delay(float seconds, Action onDelay, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.Delay(seconds, onDelay, timerName);
        }

        public static TimerInfo Delay(TimeSpan delay, Action onDelay, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.Delay(delay, onDelay, timerName);
        }

        public static UniTask DelayTask(float seconds, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.DelayTask(seconds, timerName);
        }

        public static UniTask DelayTask(TimeSpan delay, string timerName = TimerManager.DefaultTimerName)
        {
            return TimerMgr.DelayTask(delay, timerName);
        }
    }
}
