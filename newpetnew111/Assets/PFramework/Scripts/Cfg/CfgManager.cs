
using UnityEngine;
using PFramework;
using System.Collections.Generic;
using UniRx;
using System;
using TMPro;

namespace PFramework.Cfg
{
    public class CfgManager : ManagerBase<CfgManager>
    {
        //缓存的游戏配置列表
        private Dictionary<string, CfgBase> _cachedCfgs = new Dictionary<string, CfgBase>();

        public ReactiveProperty<SystemLanguage> Language { get; private set; } = new ReactiveProperty<SystemLanguage>(SystemLanguage.English);

        private CfgSettings _settings;
        public CfgSettings Settings => _settings;

        private CfgManager() { }

        public override void Initialize()
        {
            //初始化的时候缓存是空的所以不会调用SetLanguage
            Language.Subscribe(OnLanguageChanged);
            _settings = CfgSettings.GetSettings<CfgSettings>();
        }

        private void OnLanguageChanged(SystemLanguage language)
        {
            foreach(var cfg in _cachedCfgs.Values)
            {
                cfg.SetLanguage(language);
            }
        }

        /// <summary>
        /// 获取配置 对外
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        public T GetCfg<T>() where T : CfgBase
        {
            string nameKey = typeof(T).Name;
            CfgBase so;
            if (_cachedCfgs.TryGetValue(nameKey, out so))
            {
                return so as T;
            }
            else
            {
                return LoadOneCfg<T>();
            }
        }


        //加载一个配置
        private T LoadOneCfg<T>() where T : CfgBase
        {
            string cfgName = typeof(T).Name;
            if (_cachedCfgs.ContainsKey(cfgName))
            {
                return _cachedCfgs[cfgName] as T;
            }
            //构建一个
            T so = null;
            ////先判断外部是否存在
            var cfgObject = Peach.LoadSync<ScriptableObject>(_settings.cfgAssetsLoadPath + "/" + cfgName);
            //Main.ResMgr.
            if (cfgObject != null)
            {
                so = cfgObject as T;
                //map
                so.OnCfgLoad();
                _cachedCfgs.Add(cfgName, so);
                so.SetLanguage(Language.Value);
            }
            else
            {
                Debug.LogError($"No Language Cfg Asset of {typeof(T)}");
            }
            return so;
        }

        public void ClearAll()
        {
            if (_cachedCfgs != null)
            {
                _cachedCfgs.Clear();
            }
        }
    }
}
