namespace PFramework.Res
{
    using System.Collections.Generic;
    using UnityEngine;
    using UniRx.Async;
    using UnityEngine.AddressableAssets;
    using System;
    using UnityEngine.ResourceManagement.ResourceLocations;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    using UnityEngine.ResourceManagement.AsyncOperations;
    using System.Text;

    public class AAResManager : ResManagerBase
    {
        private class AAResHandler
        {
            //加载指定资源的句柄，用于释放指定资源，采用不区分的引用计数
            private Queue<AsyncOperationHandle> _handles = new Queue<AsyncOperationHandle>();
            private Dictionary<string, AAResHandler> _children = new Dictionary<string, AAResHandler>();

            public string key;

            public bool IsActive => _children.Count > 0 || _handles.Count > 0;
           
            public AAResHandler(string key)
            {
                this.key = key;
            }

            public void AddHandle(string path, AsyncOperationHandle handle)
            {
                if (!path.IsNullOrEmpty())
                {
                    var splits = path.Split('/');
                    var rootKey = splits[0];
                    if (rootKey.Equals(key))
                    {
                        if (splits.Length > 1)
                        {
                            var childKey = splits[1];
                            var child = _children.GetOrCreate(childKey, () => new AAResHandler(childKey));
                            child.AddHandle(BuildChildPath(splits), handle);
                        }
                        else
                        {
                            _handles.Enqueue(handle);
                        }
                    }
                }
            }

            public void Release(string path)
            {
                if (!path.IsNullOrEmpty())
                {
                    var splits = path.Split('/');
                    var rootKey = splits[0];
                    if (rootKey.Equals(key))
                    {
                        if(splits.Length > 1)
                        {
                            var childKey = splits[1];
                            if (_children.ContainsKey(childKey))
                            {
                                _children[childKey].Release(BuildChildPath(splits));
                                if (!_children[childKey].IsActive)
                                {
                                    _children.Remove(childKey);
                                }
                            }
                        }
                        else
                        {
                            if(_handles.Count > 0)
                            {
                                var handle = _handles.Dequeue();
                                Addressables.Release(handle);
                            }
                            else
                            {
                                _children.Values.ForEach((child) => child.ReleaseAllHandles(), null);
                                _children.Clear();
                            }
                        }
                    }
                }
            }

            private void ReleaseAllHandles()
            {
                foreach(var handle in _handles)
                {
                    Addressables.Release(handle);
                }
                foreach(var child in _children.Values)
                {
                    child.ReleaseAllHandles();
                }

                _children.Clear();
                _handles.Clear();
            }

            private string BuildChildPath(string[] splits)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < splits.Length; i++)
                {
                    sb.Append(splits[i]);
                    if (i != splits.Length - 1)
                    {
                        sb.Append("/");
                    }
                }
                return sb.ToString();
            }
        }

        private Dictionary<string, AAResHandler> _rootHandlers = new Dictionary<string, AAResHandler>();

        private AAResManager() { }

        public override void Initialize() { }

        public override UnityEngine.Object LoadSync(string path)
        {
            return Resources.Load(path);
        }

        public override T LoadSync<T>(string path)
        {
            return Resources.Load<T>(path);
        }

        public override UnityEngine.Object[] LoadAllSync(string path)
        {
            return Resources.LoadAll(path);
        }

        public override T[] LoadAllSync<T>(string path)
        {
            return Resources.LoadAll<T>(path);
        }

        public override UniTask<UnityEngine.Object[]> LoadAllAsync(string[] paths, Action<UnityEngine.Object[]> callback = null)
        {
            return AALoadAllAsync(paths, callback);
        }

        public override UniTask<T[]> LoadAllAsync<T>(string[] paths, Action<T[]> callback = null)
        {
            return AALoadAllAsync(paths, callback);
        }

        public override UniTask<UnityEngine.Object> LoadAsync(string path, Action<UnityEngine.Object> callback = null)
        {
            return AALoadAsync(path, callback);
        }

        public override UniTask<T> LoadAsync<T>(string path, Action<T> callback = null)
        {
            return AALoadAsync(path, callback);
        }

        public override UniTask<SceneInstance> LoadSceneAsync(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            return AALoadSceneAsync(path, loadSceneMode, activateOnLoad, priority);
        }

        public override void Release(object obj)
        {
            if(obj is string @key)
            {
                var handler = GetRootHandler(key);
                handler.Release(key);
            }
            else
            {
                Debug.LogError("Error param is not string! 此模块只支持根据路径卸载资源");
            }
        }

        //----------------------Addressable2UniTask-----------------------------
        private AAResHandler GetRootHandler(string path)
        {
            AAResHandler handler = null;
            if (!path.IsNullOrEmpty())
            {
                var rootKey = string.Empty;
                var splits = path.Split('/');
                rootKey = splits[0];
                if (!rootKey.IsNullOrEmpty())
                {
                    handler = _rootHandlers.GetOrCreate(rootKey,()=> new AAResHandler(rootKey));
                }
            }
            return handler;
        }

        private UniTask<T[]> AALoadAllAsync<T>(string[] paths, Action<T[]> callback) where T : UnityEngine.Object
        {
            int pathsLength = paths.Length;
            T[] results = new T[pathsLength];
            UniTask<T>[] tasks = new UniTask<T>[pathsLength];
            for (int i = 0; i < pathsLength; i++)
            {
                int index = i;
                tasks[i] = AALoadAsync<T>(paths[index], (result) => { results[index] = result; });
            }
            var task = UniTask.WhenAll(tasks);
            if(callback != null)
            {
                task.ContinueWith(callback).Forget();
            }
            return task;
        }

        private UniTask<T> AALoadAsync<T>(string path, Action<T> callback = null) where T : UnityEngine.Object
        {
            var utcs = new UniTaskCompletionSource<T>();
            var handle = Addressables.LoadAssetAsync<T>(path);
            handle.Completed += x => {
                    utcs.TrySetResult(x.Result);
                    callback?.Invoke(x.Result);
                };
            var handler = GetRootHandler(path);
            if(handler != null)
            {
                handler.AddHandle(path, handle);
            }
            return utcs.Task;
        }

        private UniTask<SceneInstance> AALoadSceneAsync(string path, LoadSceneMode loadSceneMode = LoadSceneMode.Single,bool activateOnLoad = true, int priority = 100)
        {
            var utcs = new UniTaskCompletionSource<SceneInstance>();
            var handle = Addressables.LoadSceneAsync(path, loadSceneMode, activateOnLoad, priority);
            handle.Completed += x => utcs.TrySetResult(x.Result);
            var handler = GetRootHandler(path);
            if (handler != null)
            {
                handler.AddHandle(path, handle);
            }
            return utcs.Task;
        }

        #region 暂时用不上
        private UniTask<IList<IResourceLocation>> AALoadLocationsAsync(string label)
        {
            var utcs = new UniTaskCompletionSource<IList<IResourceLocation>>();
            AssetLabelReference labelReference = new AssetLabelReference
            {
                labelString = label
            };
            Addressables.LoadResourceLocationsAsync(labelReference).Completed += (x) => { utcs.TrySetResult(x.Result); };
            return utcs.Task;
        }

        private UniTask<GameObject> AAInstantiateAsync(string path, Transform parent = null)
        {
            var utcs = new UniTaskCompletionSource<GameObject>();
            Addressables.InstantiateAsync(path, parent).Completed += x => utcs.TrySetResult(x.Result);
            return utcs.Task;
        }
        #endregion
    }
}
