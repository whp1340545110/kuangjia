using System;
using System.Collections.Generic;
using System.Reflection;
using Singleton;
using UnityEngine;

namespace PFramework
{
    public static partial class Peach
    {
        private static PeachSettings _settings;
        public static PeachSettings Settings
        {
            get
            {
                if (!_settings)
                {
                    _settings = GetSettings<PeachSettings>();
                }
                return _settings;
            }
        }

        public static bool IsInitilized { get; private set; } = false;

        public const string PeachAssetsDir = "PeachAssets";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (!IsInitilized)
            {
                InitializeManagers();
                IsInitilized = true;
            }
        }

        private static void InitializeManagers()
        {
            var managerFullNames = Settings.otherManagers;

            List<IManager> managers = new List<IManager>();
            float t = Time.realtimeSinceStartup;
            managerFullNames.ForEach((managerName) => { managers.Add(CreateManager(managerName)); });
            managers.Add(CreateManager(Settings.resManager));
            managers.ForEach((manager) => { manager?.Initialize(); },
                    (manager,e) => { PDebug.LogError($"{manager} Initialize error : {e}"); });
        }

        private static IManager CreateManager(string typeName)
        {
            IManager manager = null;
            try
            {
                if (typeName.IsNullOrEmpty())
                {
                    throw new Exception("Empty typeName exception");
                }

                Type aType = null;
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach(var assemb in assemblies)
                {
                    aType = assemb.GetType(typeName);
                    if(aType != null)
                    {
                        break;
                    }
                }

                if(aType == null)
                {
                    throw new Exception("Manager type is null exception");
                }

                bool isIManager = aType.GetInterface(typeof(IManager).ToString()) != null;
                if (isIManager)
                {
                    // 获取私有构造函数
                    var ctors = aType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

                    // 获取无参构造函数
                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);

                    if (ctor == null)
                    {
                        throw new Exception("Non-Public Constructor() not found! in " + aType);
                    }

                    manager = ctor.Invoke(null) as IManager;
                }
            }
            catch(Exception e)
            {
                var error = string.Format("Add Manager {0} Failed! Detaile : {1}", typeName , e.ToString());
                PDebug.LogError(error);
            }
            return manager;
        }


        public static T GetSettings<T>() where T : SettingsBase
        {
            return SettingsBase.GetSettings<T>();
        }

    }
}
