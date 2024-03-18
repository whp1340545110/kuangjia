using System;
using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;

namespace PFramework.Timers
{
    public class TimerInfo : IDisposable
    {
        private float _deltaTime;
        private int _count;
        private Action<int> _callback;

        private Action<float> _frameCallback;

        private float _interval;
        public float Interval => _interval;

        private float _delay;
        public float Delay => _delay;

        private bool _isDisposed;
        public bool IsDisposed => _isDisposed;

        private TimerInfo() { }

        internal TimerInfo(float interval, Action<int> callback, int count = -1)
        {
            this._interval = interval;
            this._count = count;
            this._callback = callback;
            this._isDisposed = count == 0;
            this._delay = -1;
        }

        internal TimerInfo(float interval, float delay, Action<int> callback, int count = -1)
        {
            this._interval = interval;
            this._count = count;
            this._callback = callback;
            this._isDisposed = count == 0;
            this._delay = delay;
        }

        internal TimerInfo(float delay, Action<float> frameCallback, int count = -1)
        {
            this._interval = -1;
            this._count = count;
            this._frameCallback = frameCallback;
            this._isDisposed = count == 0;
            this._delay = delay;
        }

        public void Invoke(float deltaTime)
        {
            if (!_isDisposed)
            {
                _deltaTime += deltaTime;
                if (_delay >= 0)
                {
                    if (_deltaTime > _delay)
                    {
                        _delay = -1;
                        _count -= 1;
                        _isDisposed = _count == 0;
                        _callback?.Invoke(0);
                    }
                }
                else if (_deltaTime > _interval)
                {
                    if (_interval < 0)
                    {
                        _frameCallback.Invoke(_deltaTime);
                        _deltaTime = 0;
                        _count -= 1;
                        _isDisposed = _count == 0;
                    }
                    else
                    {
                        int invokeCount = Mathf.RoundToInt(_deltaTime / _interval);
                        //限制了剩余次数的就只能调用剩余次数
                        if (_count > 0)
                        {
                            invokeCount = invokeCount > _count ? _count : invokeCount;

                            _count -= invokeCount;
                            //次数用完了
                            _isDisposed = _count == 0;
                        }

                        _deltaTime -= invokeCount * _interval;

                        _callback?.Invoke(invokeCount);
                    }
                }
            }
        }

        public void DisposeInvoke()
        {
            _callback?.Invoke(_count);
            Dispose();
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }

    public class Timer : MonoBehaviour
    {
        private readonly List<TimerInfo> _timerInfos = new List<TimerInfo>();

        private int _infoCount = 0;

        private bool _isNeedRemoved = false;

        private Coroutine _timingCoroutine;

        public int timeScale = 1;

        private float _lastRealtime = 0;

        public string timerName;

        [SerializeField]
        private bool _isActive = true;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        StartTiming();
                    }
                    else
                    {
                        StopTiming();
                    }
                }
            }
        }

        public TimerInfo Interval(float interval, Action<int> onInterval, int count = -1)
        {
            var info = new TimerInfo(interval, 0, onInterval, count);
            AddTimerInfo(info);
            TryStartTiming();
            return info;
        }

        public TimerInfo Interval(TimeSpan interval, Action<int> onInterval, int count = -1)
        {
            return Interval((float)interval.TotalSeconds, onInterval, count);
        }

        public TimerInfo Interval(float interval, float delay, Action<int> onInterval, int count = -1)
        {
            var info = new TimerInfo(interval, delay, onInterval, count);
            AddTimerInfo(info);
            TryStartTiming();
            return info;
        }

        public TimerInfo Interval(TimeSpan interval, TimeSpan delay, Action<int> onInterval, int count = -1)
        {
            return Interval((float)interval.TotalSeconds, (float)delay.TotalSeconds, onInterval, count);
        }

        public TimerInfo IntervalFrame(TimeSpan delay, Action<float> onInterval, int count = -1)
        {
            return IntervalFrame((float)delay.TotalSeconds, onInterval, count);
        }

        public TimerInfo IntervalFrame(float delay, Action<float> onInterval, int count = -1)
        {
            var info = new TimerInfo(delay, onInterval, count);
            AddTimerInfo(info);
            TryStartTiming();
            return info;
        }

        public TimerInfo Delay(float delay, Action onDelay)
        {
            var info = new TimerInfo(0, delay, (count) => { onDelay(); }, 1);
            AddTimerInfo(info);
            TryStartTiming();
            return info;
        }

        public TimerInfo Delay(TimeSpan delay, Action onDelay)
        {
            return Delay((float)delay.TotalSeconds, onDelay);
        }

        public UniTask DelayTask(float delay)
        {
            var info = new TimerInfo(0, delay, null, 1);
            AddTimerInfo(info);
            TryStartTiming();
            return UniTask.WaitUntil(() => info.IsDisposed);
        }

        public UniTask DelayTask(TimeSpan delay)
        {
            return DelayTask((float)delay.TotalSeconds);
        }

        private void TryStartTiming()
        {
            if (IsActive)
            {
                StartTiming();
            }
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        private void StartTiming()
        {
            if (_timingCoroutine == null)
            {
                _timingCoroutine = StartCoroutine(TimingEnumrator());
            }
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        private void StopTiming()
        {
            if (_timingCoroutine != null)
            {
                StopCoroutine(_timingCoroutine);
                _timingCoroutine = null;
            }
        }

        /// <summary>
        /// 计时协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator TimingEnumrator()
        {
            while (_infoCount > 0)
            {
                _lastRealtime = Time.realtimeSinceStartup;
                yield return null;
                float deltaTime = (Time.realtimeSinceStartup - _lastRealtime) * timeScale;
                _timerInfos.ForIndex((index, info) => CheckInfoInvokeAndDisposed(info, deltaTime));
                RemoveFinishedInfos();
            }
            _timingCoroutine = null;
        }

        /// <summary>
        /// 检测info的方法是否需要唤醒和是否需要移除
        /// </summary>
        /// <param name="index"></param>
        /// <param name="info"></param>
        private void CheckInfoInvokeAndDisposed(TimerInfo info, float deltaTime)
        {
            info.Invoke(deltaTime);
            _isNeedRemoved |= info.IsDisposed;
        }

        private void AddTimerInfo(TimerInfo timerInfo)
        {
            _timerInfos.Add(timerInfo);
            _infoCount++;
        }

        private void RemoveFinishedInfos()
        {
            if (_isNeedRemoved)
            {
                int count = _timerInfos.RemoveAll((info) => info.IsDisposed);
                _isNeedRemoved = false;
                _infoCount -= count;
            }
        }
    }
}
