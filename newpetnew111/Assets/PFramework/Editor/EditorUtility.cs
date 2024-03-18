namespace PeachEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using PFramework;
    using UnityEditor;
    using UnityEngine;

    public static class PEditorUtility
    {
        public static Assembly[] GetEditorAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            var asmbPath = Environment.CurrentDirectory + "/Library/ScriptAssemblies/Assembly-CSharp-Editor-firstpass.dll";
            if (File.Exists(asmbPath))
            {
                assemblies.Add(Assembly.LoadFrom(asmbPath));
            }
            asmbPath = Environment.CurrentDirectory + "/Library/ScriptAssemblies/Assembly-CSharp-Editor.dll";
            if (File.Exists(asmbPath))
            {
                assemblies.Add(Assembly.LoadFrom(asmbPath));
            }
            return assemblies.ToArray();
        }

        public static Assembly[] GetRunTimeAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            var asmbPath = Environment.CurrentDirectory + "/Library/ScriptAssemblies/Assembly-CSharp-firstpass.dll";
            if (File.Exists(asmbPath))
            {
                assemblies.Add(Assembly.LoadFrom(asmbPath));
            }
            asmbPath = Environment.CurrentDirectory + "/Library/ScriptAssemblies/Assembly-CSharp.dll";
            if (File.Exists(asmbPath))
            {
                assemblies.Add(Assembly.LoadFrom(asmbPath));
            }
            return assemblies.ToArray();
        }

        public static List<string> GetSubClassName(Type superClassType)
        {
            List<string> list = new List<string>();

            var asmbs = GetRunTimeAssemblies();

            foreach(var a in asmbs)
            {
                foreach (Type t in a.GetTypes())
                {
                    //是否是類
                    if (t.IsClass)
                    {
                        //是否是當前類的派生類
                        if (t.IsSubclassOf(superClassType))
                        {
                            list.Add(t.Name);
                        }
                    }
                }
            }
            return list;
        }
    }

    public static class SettingsEditorUtility
    {
        [OnSettingsInit(prioty = 0)]
        public static void CreateAllSettingTypes()
        {
            var typeList = FindAllSettingTypes();
            foreach (var cType in typeList)
            {
                CreateSettings(cType);
            }
            AssetDatabase.Refresh();
        }

        public static List<Type> FindAllSettingTypes()
        {
            var aType = typeof(SettingsBase);
            var typeList = new List<Type>();
            var asmbs = PEditorUtility.GetRunTimeAssemblies();
            foreach(var asmb in asmbs)
            {
                var types = asmb.GetTypes();
                foreach (var type in types)
                {
                    var baseType = type.BaseType;  //获取基类
                    while (baseType != null)  //获取所有基类
                    {
                        if (baseType.FullName == aType.FullName)
                        {
                            typeList.Add(type);
                            break;
                        }
                        else
                        {
                            baseType = baseType.BaseType;
                        }
                    }
                }
            }
            return typeList;
        }

        public static List<SettingsBase> GetSettingsList(List<Type> settingTypes)
        {
            List<SettingsBase> settingsList = new List<SettingsBase>();
            foreach (var aType in settingTypes)
            {
                if (aType.IsSubclassOf(typeof(SettingsBase)))
                {
                    var settings = AssetDatabase.LoadAssetAtPath(string.Format($"Assets/{Peach.PeachAssetsDir}/Resources/Peach/Settings/{aType.Name}.asset"), aType);
                    if (settings)
                    {
                        settingsList.Add(settings as SettingsBase);
                    }
                }
            }
            return settingsList;
        }

        public static void CreateSettings(Type settingsType)
        {
            if (!settingsType.IsSubclassOf(typeof(SettingsBase)))
            {
                return;
            }
            if (!AssetDatabase.IsValidFolder($"Assets/{Peach.PeachAssetsDir}"))
            {
                AssetDatabase.CreateFolder("Assets", Peach.PeachAssetsDir);
            }

            if (!AssetDatabase.IsValidFolder($"Assets/{Peach.PeachAssetsDir}/Resources"))
            {
                AssetDatabase.CreateFolder($"Assets/{Peach.PeachAssetsDir}", "Resources");
            }

            if (!AssetDatabase.IsValidFolder($"Assets/{Peach.PeachAssetsDir}/Resources/Peach"))
            {
                AssetDatabase.CreateFolder($"Assets/{Peach.PeachAssetsDir}/Resources", "Peach");
            }

            if (!AssetDatabase.IsValidFolder($"Assets/{Peach.PeachAssetsDir}/Resources/Peach/Settings"))
            {
                AssetDatabase.CreateFolder($"Assets/{Peach.PeachAssetsDir}/Resources/Peach", "Settings");
            }

            var settings = AssetDatabase.LoadAssetAtPath(string.Format($"Assets/{Peach.PeachAssetsDir}/Resources/Peach/Settings/{settingsType.Name}.asset"), settingsType);
            if (!settings)
            {
                settings = ScriptableObject.CreateInstance(settingsType);
                AssetDatabase.CreateAsset(settings, string.Format($"Assets/{Peach.PeachAssetsDir}/Resources/Peach/Settings/{settingsType.Name}.asset"));
            }
        }
    }
}
