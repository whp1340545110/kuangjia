using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace PFramework.Page
{
#pragma warning disable 0649
    public class PageBase : MonoBehaviour
    {
        #region 弹窗的属性
        [SerializeField]
        private bool _isPopup;
        /// <summary>
        /// 是否是弹窗
        /// </summary>
        public bool isPopup => _isPopup;

        [SerializeField]
        private bool _isStackPopup = true;
        /// <summary>
        /// 弹窗堆栈,新的弹窗隐藏旧的还未关闭的弹窗
        /// </summary>
        public bool isStackPopup => _isStackPopup;
        #endregion

        [SerializeField]
        private bool _isSingleton = true;
        /// <summary>
        /// 是否是单例
        /// </summary>
        internal bool isSingleton => _isSingleton;

        [SerializeField]
        private bool _isCloseOnSceneUnload = true;
        /// <summary>
        /// 是否是切换场景时销毁
        /// </summary>
        internal bool isCloseOnSceneUnload => _isCloseOnSceneUnload;

        [SerializeField]
        private bool _isCached = false;
        /// <summary>
        /// 是否需要放入缓冲池
        /// </summary>
        internal bool isCached => _isCached;

        [SerializeField]
        private bool _isAutoRelease = false;
        /// <summary>
        /// 关闭时是否自动释放资源
        /// </summary>
        public bool IsAutoRelease => _isAutoRelease;

        [SerializeField]
        private E_PageLayer _uiLayer = E_PageLayer.Default;
        /// <summary>
        /// 打开的层级
        /// </summary>
        public virtual E_PageLayer UILayer
        {
            get
            {
                if (_isPopup)
                {
                    return E_PageLayer.Popup;
                }
                return _uiLayer;
            }
        }

        private Animation _pageAnimation;
        public Animation PageAnimation
        {
            get
            {
                if (_useAnimation && !_pageAnimation)
                {
                    _pageAnimation = GetComponent<Animation>();
                    if (!_pageAnimation)
                    {
                        _pageAnimation = gameObject.AddComponent<Animation>();
                    }
                }
                return _pageAnimation;
            }
        }

        [SerializeField]
        private bool _useAnimation;

        [SerializeField]
        private AnimationClip _createAni;
        public AnimationClip CreateAni => _createAni;

        [SerializeField]
        private AnimationClip _closeAni;
        public AnimationClip CloseAni => _closeAni;

        #region 状态
        /// <summary>
        /// 正在关闭
        /// </summary>
        private bool _isClosing = false;

        /// <summary>
        /// 是否已经关闭
        /// </summary>
        public bool IsClosed { get; private set; }
        #endregion

        #region PageManager接口
        internal void OnCreate(params object[] objects)
        {
            IsClosed = false;
            _isClosing = false;
            PlayPageAnimationClip(_createAni).Forget();
            OnPageCreate(new Arguments() { args = objects });
        }

        internal void OnClose()
        {
            IsClosed = true;
            OnPageClose();
        }
        internal void OnResume()
        {
            gameObject.SetActive(true);
            OnPopupResume();
        }

        internal void OnPause()
        {
            //如果这个弹窗正在关闭就不能SetActive false
            if (!IsClosed && !_isClosing)
            {
                gameObject.SetActive(false);
                OnPopupPause();
            }
        }
        #endregion

        #region 关闭接口
        public void Close()
        {
            if (!IsClosed)
            {
                PageManager.Instance.ClosePage(this);
            }
        }

        /// <summary>
        /// 异步的关闭，播放动画
        /// </summary>
        /// <returns></returns>
        public async UniTask CloseAsync()
        {
            if (!IsClosed && !_isClosing)
            {
                _isClosing = true;
                await PlayPageAnimationClip(_closeAni);
                if (!IsClosed)
                {
                    PageManager.Instance.ClosePage(this);
                }
                _isClosing = false;
            }
        }


        public void CloseAsyncForget()
        {
            CloseAsync().Forget();
        }

        protected async UniTask PlayPageAnimationClip(AnimationClip clip)
        {
            if (PageAnimation)
            {
                if (clip)
                {
                    try
                    {
                        PageAnimation.AddClip(clip, clip.name);
                        PageAnimation.clip = clip;
                        PageAnimation.Play(clip.name);
                        await UniTask.WaitUntil(() => IsClosed || !PageAnimation.IsPlaying(clip.name));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(string.Format("Page {0} Create Animation Play Failed, Exception = {1}", GetType().Name, e));
                    }
                }
            }
        }
        #endregion

        #region 其他
        public async UniTask WaitForClose()
        {
            await UniTask.WaitUntil(() => IsClosed);
        }

        protected async UniTask Delay(float seconds)
        {
            System.Threading.CancellationTokenSource cancellationSource = new System.Threading.CancellationTokenSource();
            var timeSpan = TimeSpan.FromSeconds(seconds);
            var delayTask = UniTask.Delay(timeSpan, false, PlayerLoopTiming.Update, cancellationSource.Token);
            var checkCloseTask = UniTask.WaitUntil(() => IsClosed, PlayerLoopTiming.Update, cancellationSource.Token);
            await UniTask.WhenAny(delayTask, checkCloseTask);
            if (IsClosed)
            {
                throw new Exception("Page Close Exception");
            }
            cancellationSource.Cancel();
        }

        protected void StopCoroutineAndReset(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
        #endregion

        #region Mono生命周期
        private void Awake()
        {
            OnPageAwake();
        }

        private void Start()
        {
            OnPageStart();
        }

        private void OnDestroy()
        {
            OnPageDestroy();
        }
        #endregion

        #region  子类生命周期
        protected virtual void OnPageAwake() { }

        protected virtual void OnPageStart() { }

        protected virtual void OnPageDestroy() { }

        protected virtual void OnPageCreate(Arguments args) { }

        protected virtual void OnPageClose() { }

        protected virtual void OnPopupResume() { }

        protected virtual void OnPopupPause() { }
        #endregion
    }
#pragma warning restore 0649
}
