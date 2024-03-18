using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UniRx.Async;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace PFramework.Page
{
    public class PageManager : ManagerBase<PageManager>
    {
        private PageManager() { }

        private readonly List<PageBase> _popupStack = new List<PageBase>();

        /// <summary>
        /// Canvas
        /// </summary>
        private readonly Dictionary<E_PageLayer, Canvas> _canvas = new Dictionary<E_PageLayer, Canvas>();

        /// <summary>
        /// 已经关闭的UI的缓存池
        /// </summary>
        private readonly Dictionary<string, List<PageBase>> _cache = new Dictionary<string, List<PageBase>>();

        /// <summary>
        /// 已经打开的UI的缓存
        /// </summary>
        private readonly Dictionary<string, List<PageBase>> _pages = new Dictionary<string, List<PageBase>>();

        private Transform _uiRoot;

        private PageSettings _settings;

        private EventSystem _uiEventSystem;

        private Camera _uiCamera;
        public Camera UICamera => _uiCamera;

        public Action<PageBase> onPageCreate;
        public Action<PageBase> onPageClose;

        #region 初始化和重置

        public override void Initialize()
        {
            _settings = PageSettings.GetSettings<PageSettings>();
            CreateCanvases();
            CreateCamera();
            SceneManager.sceneLoaded += OnSceneLoad;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void CreateCanvases()
        {
            var canvasRoot = new GameObject("UIRoot");
            GameObject.DontDestroyOnLoad(canvasRoot);
            _uiRoot = canvasRoot.transform;
            foreach (int intValue in Enum.GetValues(typeof(E_PageLayer)))
            {
                E_PageLayer type = (E_PageLayer)intValue;
                var canvas = GetCanvas(type);
                canvas.transform.SetParent(canvasRoot.transform);
                if (type == E_PageLayer.Hidden)
                {
                    canvas.gameObject.SetActive(false);
                }
                if (_settings.isUICameraTargetLayerMask)
                {
                    canvas.gameObject.layer = _settings.uiLayer;
                }
            }
        }

        private void CreateCamera()
        {
            var cameraObject = new GameObject("UICamera");
            cameraObject.transform.SetParent(_uiRoot, false);
            _uiCamera = cameraObject.AddComponent<Camera>();
            _uiCamera.clearFlags = _settings.cameraClearFlags;
            _uiCamera.depth = _settings.cameraDepth;
            _uiCamera.orthographic = _settings.orthographic;
            if (_settings.isUICameraTargetLayerMask)
            {
                _uiCamera.cullingMask = _settings.cameraCullingMask;
            }
            foreach (var canvas in _canvas.Values)
            {
                canvas.worldCamera = _uiCamera;
            }
        }

        private void CreateEventSystem()
        {
            if (!EventSystem.current)
            {
                var eventSystemObject = new GameObject("EventSystem");
                _uiEventSystem = eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
                eventSystemObject.AddComponent<BaseInput>();
                eventSystemObject.transform.SetParent(_uiRoot);
            }
            else if(!_uiEventSystem)
            {
                EventSystem.current.transform.SetParent(_uiRoot);
                _uiEventSystem = EventSystem.current;
            }
        }

        /// <summary>
        /// 切场景的时候
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneUnloaded(Scene scene)
        {
            if (_settings.isClearUIOnSceneUnload)
            {
                CloseAllPagesOnSceneUnload();
            }
        }

        /// <summary>
        /// 切场景的时候
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            CreateEventSystem();
            DelayFrameOnSceneLoad().Forget();
        }

        private async UniTask DelayFrameOnSceneLoad()
        {
            await UniTask.DelayFrame(0, PlayerLoopTiming.PreUpdate);
            JoinSceneUIToCache();
        }

        //这个是为了把场景UI加入UI管理
        private void JoinSceneUIToCache()
        {
            var uiArray = GameObject.FindObjectsOfType<PageBase>();
            foreach (var page in uiArray)
            {
                var canvas = page.transform.parent.GetComponent<Canvas>();
                if (canvas && !canvas.transform.parent)
                {
                    page.transform.SetParent(_canvas[page.UILayer].transform, false);
                    AddToPages(page);
                    page.OnCreate();
                    onPageCreate?.Invoke(page);
                    var childrenPage = page.transform.GetComponentsInChildren<PageBase>();
                    foreach (var child in childrenPage)
                    {
                        if(child != page)
                        {
                            child.OnCreate();
                            onPageCreate?.Invoke(child);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 创建一个Canvas
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        private Canvas CreateCanvas(E_PageLayer layer)
        {
            Canvas canvas = null;
            var canvasName = PageUtility.GetCanvasName(layer);
            var go = GameObject.Find(canvasName);
            if (go)
            {
                canvas = go.GetComponent<Canvas>();
            }
            if (!canvas)
            {
                canvas = PageUtility.CreateCanvas(layer);
            }
            return canvas;
        }
        #endregion


        //根据预制体实例化页面
        private T CreatePageByPrefab<T>(string uiName, GameObject pagePrefab, Transform pageParent = null) where T : PageBase
        {
            if (!pageParent)
            {
                var prefabPage = pagePrefab.GetComponent<PageBase>();
                pageParent = GetCanvas(prefabPage.UILayer).transform;
            }

            var go = GameObject.Instantiate(pagePrefab, pageParent);

            if (_settings.isUICameraTargetLayerMask)
            {
                go.layer = _settings.uiLayer;
                var children = go.transform.GetComponentsInChildren<Transform>(true);
                foreach (var child in children)
                {
                    child.gameObject.layer = _settings.uiLayer;
                }
            }
            go.name = uiName;
            var page = go.GetComponent<T>();
            return page;

        }

        //TODO 上一层的弹窗隐藏
        private void ResortStackPopups()
        {
            if (_popupStack.Count >= 2)
            {
                PageBase pop = _popupStack[_popupStack.Count - 2];
                pop.OnPause();
            }
        }

        #region 缓存
        private void CloseAllPagesOnSceneUnload()
        {
            foreach (var key in _pages.Keys)
            {
                ClosePages(key,true);
            }
        }

        private void ClosePages(string uiName, bool onSceneUnload = false)
        {
            if (_pages.ContainsKey(uiName))
            {
                var list = _pages[uiName];
                //倒序关闭，不然这里会引发报错
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (!onSceneUnload || list[i].isCloseOnSceneUnload)
                    {
                        ClosePage(list[i]);
                    }
                }
            }
        }


        private void AddToPages(PageBase page)
        {
            string key = page.gameObject.name;
            if (!_pages.ContainsKey(key))
            {
                _pages.Add(key, new List<PageBase>());
            }
            if (!_pages[key].Contains(page))
            {
                _pages[key].Add(page);
            }
            if (page.isPopup && page.isStackPopup)
            {
                _popupStack.Add(page);
            }
        }

        private PageBase GetCachePage(string name)
        {
            PageBase page = null;
            if (_cache.ContainsKey(name))
            {
                var list = _cache[name];
                foreach (var ui in list)
                {
                    page = ui;
                    break;
                }
                if (page)
                {
                    list.Remove(page);
                }
            }
            return page;
        }

        /// <summary>
        /// 将该界面暂存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        private void AddToCache(PageBase page)
        {
            string key = page.gameObject.name;
            if (!_cache.ContainsKey(key))
            {
                _cache.Add(key, new List<PageBase>());
            }
            if (!_cache[key].Contains(page))
            {
                _cache[key].Add(page);
            }
        }
        #endregion

        #region 加载
        private async Task<GameObject> LoadGameObjectAsync(string pagePath)
        {
            GameObject prefab = await Peach.LoadAsync<GameObject>(pagePath);
            return prefab;
        }

        private GameObject LoadGameObjectSync(string pagePath)
        {
            GameObject prefab = Peach.LoadSync<GameObject>(pagePath);
            return prefab;
        }
        #endregion

        #region 对外接口
        #region Aysnc FromAA
        /// <summary>
        /// 异步打开AAUI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath">aa路径</param>
        /// <param name="pageParent">父节点</param>
        /// <param name="objects">参数</param>
        /// <returns></returns>
        public async UniTask<T> OpenChildPageAsync<T>(string pagePath, Transform pageParent, params object[] objects) where T : PageBase
        {
            var uiName = GetPageName(pagePath);
            T page = GetCachePage(uiName) as T;
            if (page)
            {
                CreateFromCache(pageParent, uiName, page);
            }
            else
            {
                GameObject prefab = await LoadGameObjectAsync(pagePath);
                if (prefab)
                {
                    var prefabPage = prefab.GetComponent<PageBase>();
                    if (!prefabPage)
                    {
                        return default(T);
                    }
                    CheckIsSinglePage(pageParent, uiName, prefabPage);
                    page = CreatePageByPrefab<T>(uiName, prefabPage.gameObject, pageParent);
                }
            }
            OnPageCreate(page, pageParent, objects);
            return page;
        }

        /// <summary>
        /// 异步打开AAUI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath">aa路径</param>
        /// <param name="objects">参数</param>
        /// <returns></returns>
        public UniTask<T> OpenPageAsync<T>(string pagePath, params object[] objects) where T : PageBase
        {
            return OpenChildPageAsync<T>(pagePath,null,objects);
        }

        /// <summary>
        /// 异步打开AAUI
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="objects"></param>
        public UniTask OpenPageAsync(string pagePath, params object[] objects)
        {
            return OpenChildPageAsync<PageBase>(pagePath, null, objects);
        }

        /// <summary>
        /// 异步打开AAUI
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <param name="objects"></param>
        public UniTask OpenChildPageAsync(string pagePath, Transform pageParent, params object[] objects)
        {
            return OpenChildPageAsync<PageBase>(pagePath, pageParent, objects);
        }

        /// <summary>
        /// 异步打开AAUI
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="objects"></param>
        public void OpenPageAsyncForget(string pagePath, params object[] objects)
        {
            OpenPageAsync<PageBase>(pagePath, objects).Forget();
        }

        /// <summary>
        /// 异步打开AAUI
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <param name="objects"></param>
        public void OpeChildPageAsyncForget(string pagePath, Transform pageParent, params object[] objects)
        {
            OpenChildPageAsync<PageBase>(pagePath, pageParent, objects).Forget();
        }
        #endregion

        #region Sync

        /// <summary>
        /// 同步打开页面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <returns></returns>
        public T OpenChildPageSync<T>(string pagePath, Transform pageParent, params object[] objects) where T : PageBase
        {
            var uiName = GetPageName(pagePath);
            T page = null;
            page = GetCachePage(uiName) as T;
            if (page)
            {
                CreateFromCache(pageParent, uiName, page);
            }
            else
            {
                GameObject prefab = LoadGameObjectSync(pagePath);
                if (prefab)
                {
                    var prefabPage = prefab.GetComponent<PageBase>();
                    if (!prefabPage)
                    {
                        return default(T);
                    }
                    CheckIsSinglePage(pageParent, uiName, prefabPage);
                    page = CreatePageByPrefab<T>(uiName, prefabPage.gameObject, pageParent);
                }
            }
            OnPageCreate(page, pageParent, objects);
            return page;
        }

        public T OpenPageSync<T>(string pagePath, params object[] objects) where T : PageBase
        {
            return OpenChildPageSync<T>(pagePath, null, objects);
        }

        /// <summary>
        /// 同步打开页面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <returns></returns>
        public T OpenChildPageSync<T>(GameObject prefab, Transform pageParent = null, params object[] objects) where T : PageBase
        {
            var uiName = prefab.name;
            T page = null;
            page = GetCachePage(uiName) as T;
            if (page)
            {
                CreateFromCache(pageParent, uiName, page);
            }
            else
            {
                var prefabPage = prefab.GetComponent<PageBase>();
                if (!prefabPage)
                {
                    return default(T);
                }
                CheckIsSinglePage(pageParent, uiName, prefabPage);
                page = CreatePageByPrefab<T>(uiName, prefabPage.gameObject, pageParent);
            }
            OnPageCreate(page, pageParent, objects);
            return page;
        }

        public T OpenPageSync<T>(GameObject prefab, params object[] objects) where T : PageBase
        {
            return OpenChildPageSync<T>(prefab, null, objects);
        }

        /// <summary>
        /// 同步打开页面
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <param name="objects"></param>
        public void OpenChildPageSync(string pagePath, Transform pageParent = null, params object[] objects)
        {
            OpenPageSync<PageBase>(pagePath, pageParent, objects);
        }

        public void OpenPageSync(string pagePath, params object[] objects)
        {
            OpenPageSync<PageBase>(pagePath, null, objects);
        }

        #endregion


        private void CreateFromCache<T>(Transform pageParent, string uiName, T page) where T : PageBase
        {
            CheckIsSinglePage(pageParent, uiName, page);
            page.transform.SetParent(pageParent ? pageParent : _canvas[page.UILayer].transform);
        }

        private void CheckIsSinglePage(Transform pageParent, string uiName, PageBase page)
        {
            if (!pageParent)
            {
                if (page.isSingleton)
                {
                    ClosePages(uiName);
                }
            }
        }

        private T OnPageCreate<T>(T page, Transform pageParent, object[] objects) where T : PageBase
        {
            if (!pageParent)
            {
                AddToPages(page);
            }
            if (page.isPopup && page.isStackPopup)
            {
                ResortStackPopups();
            }
            //page生命周期函数
            page.OnCreate(objects);
            onPageCreate?.Invoke(page);
            return page;
        }

        private string GetPageName(string pagePath)
        {
            //var index = pagePath.LastIndexOf("/");
            //if (index == -1)
            //{
            //    return pagePath;
            //}
            //return pagePath.Substring(index + 1);
            return pagePath;
        }

        public async UniTask PreloadUI(List<string> pagePaths)
        {
            foreach (var path in pagePaths)
            {
                await LoadGameObjectAsync(path);
            }
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="page"></param>
        internal void ClosePage(PageBase page)
        {
            if (_pages.ContainsKey(page.gameObject.name))
            {
                _pages[page.gameObject.name].Remove(page);
            }
            if (page.isPopup && page.isStackPopup && _popupStack.Contains(page))
            {
                bool isTopPopup = _popupStack.IndexOf(page) == _popupStack.Count - 1;
                _popupStack.Remove(page);
                if (isTopPopup && _popupStack.Count > 0)
                {
                    _popupStack.Last().OnResume();
                }
            }
            var childrenPage = page.transform.GetComponentsInChildren<PageBase>();
            foreach (var child in childrenPage)
            {
                child.OnClose();
                onPageClose?.Invoke(child);
            }
            if (page.isCached)
            {
                AddToCache(page);
                page.transform.SetParent(_canvas[E_PageLayer.Hidden].transform);
                page.gameObject.SetActive(true);
            }
            else
            {
                if (page.IsAutoRelease)
                {
                    Peach.Release(page.name);
                }
                GameObject.Destroy(page.gameObject);
            }
        }

        public PageBase FindPage(string pagePath)
        {
            if (_pages.ContainsKey(pagePath))
            {
                var list = _pages[pagePath];
                if (list.Count > 0)
                {
                    return list[0];
                }
            }
            return null;
        }

        /// <summary>
        /// 获取对应窗口类型的画布
        /// </summary>
        /// <param name="uiLayer"></param>
        /// <returns></returns>
        public Canvas GetCanvas(E_PageLayer uiLayer)
        {
            //GameObject canvasObj;
            Canvas canvas;

            bool hasCanvas = _canvas.TryGetValue(uiLayer, out canvas);
            if (!hasCanvas)
            {
                canvas = CreateCanvas(uiLayer);
                _canvas.Add(uiLayer, canvas);
            }
            return canvas;
        }
        #endregion
    }

    public enum E_PageLayer
    {
        Default,//最下层，主界面，加载界面等
        Window,//子菜单
        Popup,//弹窗
        Top,//这个目前应该只有loading菊花
        Hidden,//隐藏层，缓存界面放这一层
    }
}