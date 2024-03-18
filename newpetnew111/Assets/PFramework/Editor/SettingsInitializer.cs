using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PeachEditor
{
    public static class SettingsInitializer
    {
        public static void InitSettings()
        {
            InvokeCallbacks();
            AssetDatabase.Refresh();
        }

        private static void InvokeCallbacks()
        {
            var invokeMethods = new List<MethodInfo>();
            var asmbs = PEditorUtility.GetEditorAssemblies();
            foreach(var asmb in asmbs)
            {
                var types = asmb.GetTypes();
                foreach (var aType in types)
                {
                    var methodInfos = aType.GetMethods();
                    foreach (var methodInfo in methodInfos)
                    {
                        var att = methodInfo.GetCustomAttribute<OnSettingsInitAttribute>();
                        if (att != null && methodInfo.IsStatic)
                        {
                            invokeMethods.Add(methodInfo);
                        }
                    }
                }
            }

            invokeMethods.Sort((x, y) =>
            {
                var attX = x.GetCustomAttribute<OnSettingsInitAttribute>();
                var attY = y.GetCustomAttribute<OnSettingsInitAttribute>();
                return attX.prioty - attY.prioty;
            });
            invokeMethods.ForEach((method) => method.Invoke(null, new object[0]));
        }
    }
}
