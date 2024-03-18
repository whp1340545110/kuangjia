namespace PFramework
{
    using System;
    using UniRx.Async;
    using UnityEngine;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using UnityEngine.U2D;

    public abstract class ResManagerBase : ManagerBase<ResManagerBase>
    {
        #region Sync
        public abstract UnityEngine.Object LoadSync(string path);

        public abstract T LoadSync<T>(string path) where T : UnityEngine.Object;

        public abstract UnityEngine.Object[] LoadAllSync(string paths);

        public abstract T[] LoadAllSync<T>(string path) where T : UnityEngine.Object;
        #endregion Sync

        #region Async
        public abstract UniTask<UnityEngine.Object[]> LoadAllAsync(string[] paths, Action<UnityEngine.Object[]> callback = null);

        public abstract UniTask<T[]> LoadAllAsync<T>(string[] paths, Action<T[]> callback = null) where T : UnityEngine.Object;

        public abstract UniTask<UnityEngine.Object> LoadAsync(string path, Action<UnityEngine.Object> callback = null);

        public abstract UniTask<T> LoadAsync<T>(string path, Action<T> callback = null) where T : UnityEngine.Object;
        #endregion

        #region Scene
        public abstract UniTask<SceneInstance> LoadSceneAsync(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100);
        #endregion

        #region Release
        public abstract void Release(object obj);
        #endregion
    }


    public static partial class Peach
    {
        public static ResManagerBase ResMgr => ResManagerBase.Instance;
        #region Sync
        public static UnityEngine.Object LoadSync(string path)
        {
            return ResMgr.LoadSync(path);
        }

        public static T LoadSync<T>(string path) where T : UnityEngine.Object
        {
            return ResMgr.LoadSync<T>(path);
        }

        public static UnityEngine.Object[] LoadAllSync(string paths)
        {
            return ResMgr.LoadAllSync(paths);
        }

        public static T[] LoadAllSync<T>(string path) where T : UnityEngine.Object
        {
            return ResMgr.LoadAllSync<T>(path);
        }
        #endregion Sync

        #region Async
        public static UniTask<UnityEngine.Object[]> LoadAllAsync(string[] paths, Action<UnityEngine.Object[]> callback)
        {
            return ResMgr.LoadAllAsync(paths, callback);
        }

        public static UniTask<T[]> LoadAllAsync<T>(string[] paths, Action<T[]> callback) where T : UnityEngine.Object
        {
            return ResMgr.LoadAllAsync<T>(paths, callback);
        }

        public static UniTask<UnityEngine.Object> LoadAsync(string path, Action<UnityEngine.Object> callback = null)
        {
            return ResMgr.LoadAsync(path, callback);
        }

        public static UniTask<T> LoadAsync<T>(string path, Action<T> callback = null) where T : UnityEngine.Object
        {
            return ResMgr.LoadAsync<T>(path, callback);
        }
        #endregion

        #region Scene
        public static UniTask<SceneInstance> LoadSceneAsync(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            return ResMgr.LoadSceneAsync(path, loadSceneMode, activateOnLoad, priority);
        }
        #endregion

        #region Release
        public static void Release(object obj)
        {
            ResMgr.Release(obj);
        }
        #endregion
    }
}
