using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UniRx.Async;
using System;
using System.Text;
using System.Security.Cryptography;

namespace PFramework.Data
{
    [Serializable]
    public class DataBase
    {

        static DataSettings _settings;
        public static DataSettings Settings
        {
            get
            {
                if (!_settings)
                {
                    _settings = SettingsBase.GetSettings<DataSettings>();
                }
                return _settings;
            }
        }

        public virtual string DataPath => GetType().Name;

        public string FilePath => Path.Combine(Application.persistentDataPath, Settings.dataRootPath, DataPath);

        public string DirectoryPath => Path.Combine(Application.persistentDataPath, Settings.dataRootPath);

        public void LoadData()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            if (!File.Exists(FilePath))
            {
                var stream = File.CreateText(FilePath);
                stream.Close();
            }
            var path = Path.Combine(Application.persistentDataPath, FilePath);
            var text = File.ReadAllText(path);

#if !UNITY_EDITOR
            if (Settings.isAesCrypted)
            {
                try
                {
                    //这个地方要兼容之前没有加密后来又加密的存档
                    text = SecurityUtils.AesDecrypt(text, Settings.aesCryptKey);
                }
                catch(Exception e)
                {
                    Debug.Log($"解密失败，使用存档源文件:{e}");
                }
            }
#endif
            try
            {
                //这个地方这样写的目的
                //1、可以获取存档写在代码里的初始值
                //2、保证如果有属性或者实现类实现了ISerializationCallbackReceiver接口，可以调用到这个接口的实现方法
                if (string.IsNullOrEmpty(text))
                {
                    text = JsonUtility.ToJson(this);
                }
                JsonUtility.FromJsonOverwrite(text, this);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"直接解析失败，清档:{e}");
            }

            OnLoad();
        }

        public void Save()
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }
            var stream = File.CreateText(FilePath);
            stream.Write(CreateSaveString());
            stream.Close();
        }

        public async UniTask SaveAsync()
        {
            var stream = File.CreateText(FilePath);
            await stream.WriteAsync(CreateSaveString());
            stream.Close();
        }

        private string CreateSaveString()
        {
            var jsonText = JsonUtility.ToJson(this);
#if !UNITY_EDITOR
            if (Settings.isAesCrypted)
            {
                jsonText = SecurityUtils.AesEncrypt(jsonText, Settings.aesCryptKey);
            }
#endif
            return jsonText;
        }

        protected virtual void OnLoad() { }

    }
}
