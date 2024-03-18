using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

namespace PFramework.Res
{
    public class NetResManager : ManagerBase<NetResManager>
    {
        [Serializable]
        private class CacheFile
        {
            public List<string> files;
        }

        public string CacheRootPath {
            get {
                return ResUtility.GetNetCacheRootPath();
            }
        }

        public string CacheTextureDirectoryPath {
            get {
                return CacheRootPath + "/Textures";
            }
        }

        public string CatlogPath {
            get {
                return CacheRootPath + "/catlog.json";
            }
        }

        public string CacheBundleDirectoryPath {
            get {
                return CacheRootPath + "/AssetBundle";
            }
        }

        private CacheFile _cacheFile;

        private NetResManager(){ }

        public override void Initialize() { }

        #region 下载图片
        public async UniTask<Texture2D> LoadTexture(string url, bool cache = false)
        {
            try
            {
                if (cache && CheckHasCache(url))
                {
                    return await LoadTextureFromCache(url);
                }
                else
                {
                    var texture2D = await LoadTexutureFromPath(url);
                    if (cache && texture2D != null)
                    {
                        SaveTextureToCache(url, texture2D);
                    }
                    return texture2D;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        private async UniTask<Texture2D> LoadTextureFromCache(string url)
        {
            var filePath = GetCacheTexurePath(url);
            return await LoadTexutureFromPath("file://" + filePath);
        }

        private async UniTask<Texture2D> LoadTexutureFromPath(string path)
        {
            var req = UnityWebRequestTexture.GetTexture(path);
            await req.SendWebRequest().ConfigureAwait(null, PlayerLoopTiming.Update);
            if (req.isNetworkError || req.isHttpError)
            {
                string errmsg = string.Format("Get HttpError {0} url = {1}", req.error, path);
                Debug.LogError(errmsg);
                return null;
            }
            else
            {
                return DownloadHandlerTexture.GetContent(req);
            }
        }

        private void SaveTextureToCache(string url, Texture2D texture2D)
        {
            try
            {
                var md5FileName = GetMd5HashFileName(url);
                var bytes = texture2D.EncodeToJPG();
                if (!Directory.Exists(CacheTextureDirectoryPath))
                {
                    Directory.CreateDirectory(CacheTextureDirectoryPath);
                }
                File.WriteAllBytes(GetCacheTexurePath(url), bytes);
                _cacheFile.files.Add(md5FileName);
                var json = JsonUtility.ToJson(_cacheFile);
                File.WriteAllText(CatlogPath, json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        #endregion

        #region 下载AssetBundle
        /// <summary>
        /// 同步加载已缓存的资源包
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string url)
        {
            try
            {
                if (CheckHasCache(url))
                {

                    return AssetBundle.LoadFromFile(GetCacheAssetBundlePath(url));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        /// <summary>
        /// 异步加载资源包，可能从本地和云端下载
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async UniTask<AssetBundle> LoadAssetBundleAsync(string url)
        {
            if (CheckHasCache(url))
            {
                try
                {
                    var req = AssetBundle.LoadFromFileAsync(GetCacheAssetBundlePath(url));
                    await UniTask.WaitUntil(() => req.isDone);
                    return req.assetBundle;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    return null;
                }
            }
            else
            {
                var assetBundle = await DownloadAssetBundle(url);
                return assetBundle;
            }
        }

        //暂不考虑依赖关系的下载
        public async UniTask<AssetBundle> DownloadAssetBundle(string url, System.Action<float> onProgress = null, bool cache = true)
        {
            var req = UnityWebRequest.Get(url);
            var operation = req.SendWebRequest();
            while (!operation.isDone)
            {
                onProgress?.Invoke(operation.progress);
                await UniTask.DelayFrame(1);
            }
            onProgress?.Invoke(operation.progress);
            if (req.isNetworkError || req.isHttpError)
            {
                string errmsg = string.Format("Get HttpError {0} url = {1}", req.error, url);
                Debug.LogError(errmsg);
                return null;
            }
            else
            {
                var data = req.downloadHandler.data;
                var bundleReq = AssetBundle.LoadFromMemoryAsync(data);
                await bundleReq.ConfigureAwait(null, PlayerLoopTiming.Update);
                var assetBundle = bundleReq.assetBundle;
                if (assetBundle != null && cache)
                {
                    SaveAssetBundleToCache(url, data);
                }
                return assetBundle;
            }
        }

        private void SaveAssetBundleToCache(string url, byte[] data)
        {
            try
            {
                var md5FileName = GetMd5HashFileName(url);
                if (!Directory.Exists(CacheTextureDirectoryPath))
                {
                    Directory.CreateDirectory(CacheTextureDirectoryPath);
                }
                File.WriteAllBytes(GetCacheAssetBundlePath(url), data);
                _cacheFile.files.Add(md5FileName);
                var json = JsonUtility.ToJson(_cacheFile);
                File.WriteAllText(CatlogPath, json);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                //一般空间不足会有这个异常
            }
        }
        #endregion

        public bool CheckHasCache(string url)
        {
            if (_cacheFile == null)
            {
                _cacheFile = new CacheFile();
                if (!Directory.Exists(CacheRootPath))
                {
                    Directory.CreateDirectory(CacheRootPath);
                }
                if (File.Exists(CatlogPath))
                {
                    var json = File.ReadAllText(CatlogPath);
                    JsonUtility.FromJsonOverwrite(json, _cacheFile);
                }
                else
                {
                    _cacheFile.files = new List<string>();
                }
            }
            var md5HashName = GetMd5HashFileName(url);
            if (_cacheFile.files.Contains(md5HashName))
            {
                return true;
            }
            return false;
        }

        private string GetCacheAssetBundlePath(string url)
        {
            var fileName = GetMd5HashFileName(url);
            return CacheBundleDirectoryPath + "/" + fileName;
        }

        private string GetCacheTexurePath(string url)
        {
            var fileName = GetMd5HashFileName(url);
            return CacheTextureDirectoryPath + "/" + fileName;
        }

        private string GetMd5HashFileName(string url)
        {
            var splits = url.Split('.');
            var extName = string.Empty;
            if (splits.Length >= 2)
            {
                extName = "." + splits[splits.Length - 1];
            }
            var md5HashName = SecurityUtils.GetMd5Hash(url);
            return md5HashName + extName;
        }
    }
}
