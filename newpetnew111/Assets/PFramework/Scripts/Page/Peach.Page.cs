using PFramework.Page;
using UniRx.Async;
using UnityEngine;

namespace PFramework
{
    public static partial class Peach
    {
        public static PageManager PageMgr => PageManager.Instance;

        public static Camera UICamera => PageMgr.UICamera;

        /// <summary>
        /// 异步打开页面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath">aa路径</param>
        /// <param name="objects">参数</param>
        /// <returns></returns>
        public static UniTask<T> OpenPageAsync<T>(string pagePath, params object[] objects) where T : PageBase
        {
            return PageMgr.OpenPageAsync<T>(pagePath, objects);
        }

        /// <summary>
        /// 异步打开子页面（放在指定的Transform下）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath">aa路径</param>
        /// <param name="pageParent">父节点</param>
        /// <param name="objects">参数</param>
        /// <returns></returns>
        public static UniTask<T> OpenChildPageAsync<T>(string pagePath, Transform pageParent, params object[] objects) where T : PageBase
        {
            return PageMgr.OpenChildPageAsync<T>(pagePath, pageParent, objects);
        }

        /// <summary>
        /// 异步打开页面
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="objects"></param>
        public static UniTask OpenPageAsync(string pagePath, params object[] objects)
        {
            return PageMgr.OpenPageAsync(pagePath, objects);
        }

        /// <summary>
        /// 异步打开子页面（放在指定的Transform下）
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <param name="objects"></param>
        public static UniTask OpenChildPageAsync(string pagePath, Transform pageParent, params object[] objects)
        {
            return PageMgr.OpenChildPageAsync(pagePath, pageParent, objects);
        }

        /// <summary>
        /// 异步打开页面
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="objects"></param>
        public static void OpenPageAsyncForget(string pagePath, params object[] objects)
        {
            PageMgr.OpenPageAsyncForget(pagePath, objects);
        }

        /// <summary>
        /// 异步打开子页面（放在指定的Transform下）
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <param name="objects"></param>
        public static void OpenChildPageAsyncForget(string pagePath, Transform pageParent, params object[] objects)
        {
            PageMgr.OpeChildPageAsyncForget(pagePath, pageParent, objects);
        }

        /// <summary>
        /// 同步打开子页面（放在指定的Transform下）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <returns></returns>
        public static T OpenChildPageSync<T>(string pagePath, Transform pageParent, params object[] objects) where T : PageBase
        {
            return PageMgr.OpenChildPageSync<T>(pagePath, pageParent, objects);
        }

        /// <summary>
        /// 同步打开页面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static T OpenPageSync<T>(string pagePath, params object[] objects) where T : PageBase
        {
            return PageMgr.OpenPageSync<T>(pagePath, objects);
        }

        /// <summary>
        /// 同步打开子页面（放在指定的Transform下）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <returns></returns>
        public static T OpenChildPageSync<T>(GameObject prefab, Transform pageParent = null, params object[] objects) where T : PageBase
        {
            return PageMgr.OpenChildPageSync<T>(prefab, pageParent, objects);
        }

        /// <summary>
        /// 同步打开页面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static T OpenPageSync<T>(GameObject prefab, params object[] objects) where T : PageBase
        {
            return PageMgr.OpenPageSync<T>(prefab, objects);
        }

        /// <summary>
        /// 同步打开页面
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="pageParent"></param>
        /// <param name="objects"></param>
        public static void OpenChildPageSync(string pagePath, Transform pageParent = null, params object[] objects)
        {
            PageMgr.OpenChildPageSync(pagePath, pageParent, objects);
        }

        /// <summary>
        /// 同步打开页面
        /// </summary>
        /// <param name="pagePath"></param>
        /// <param name="objects"></param>
        public static void OpenPageSync(string pagePath, params object[] objects)
        {
            PageMgr.OpenPageSync(pagePath, objects);
        }

        /// <summary>
        /// 查找Page,无法查找子页面，子页面请使用GameObject.FindObjectOfType
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PageBase FindPage(string name)
        {
            return PageMgr.FindPage(name);
        }

        /// <summary>
        /// 获取对应窗口类型的画布
        /// </summary>
        /// <param name="uiLayer"></param>
        /// <returns></returns>
        public static Canvas GetCanvas(E_PageLayer uiLayer)
        {
            return PageMgr.GetCanvas(uiLayer);
        }
    }
}
