/********************************************************************
    created:	2020-04-28 				
    author:		jordenwu						
    purpose:	管理游戏配置的 导入 导出								
*********************************************************************/
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEditor.Callbacks;
using PFramework.Cfg;
using PFramework;

namespace PeachEditor.Cfg
{
    public static class CfgEditor
    {

        //策划配置目录
        public static string CfgExcelDir => Application.dataPath + $"/{Peach.PeachAssetsDir}/Cfgs/Excels";
        //c#代码定义生成目录
        public static string CfgCSharpDir => Application.dataPath + $"/{Peach.PeachAssetsDir}/Cfgs/Scripts";
        //c# 模型inspect代码目录
        public static string CfgCSharpInspectorDir => Application.dataPath + $"/{Peach.PeachAssetsDir}/Cfgs/Editor";
        //excel 转换成scriptobj资源目录
        public static string CfgsAssetDir => Application.dataPath + $"/{Peach.PeachAssetsDir}/{Peach.GetSettings<CfgSettings>().cfgAssetDir}";
        public static string CfgsAssetLocalDir => $"Assets/{Peach.PeachAssetsDir}/{Peach.GetSettings<CfgSettings>().cfgAssetDir}/";

        public static void ImportExcel()
        {
            //创建目录
            if (!Directory.Exists(CfgCSharpDir))
            {
                PFramework.FileUtil.CreateDirectory(CfgCSharpDir);
            }
            if (!Directory.Exists(CfgCSharpInspectorDir))
            {
                PFramework.FileUtil.CreateDirectory(CfgCSharpInspectorDir);
            }
            //
            var excelPath = CfgExcelDir;
            if (string.IsNullOrEmpty(excelPath))
                return;
            //生成
            Debug.Log("excelPath==" + excelPath);
            EEConverter.GenerateCSharpFiles(excelPath, CfgCSharpDir, CfgCSharpInspectorDir, CfgsAssetDir);

        }

        public static void RefreshExcelAsset()
        {
            //刷新数据
            EEConverter.GenerateScriptableObjects(CfgExcelDir, CfgsAssetDir);
        }


        public static void Clean()
        {
            EditorPrefs.SetBool(EEConverter.csChangedKey, false);
            DeleteGenedCfgCSFile();
            DeleteCfgScriptableObjectFolder();
            AssetDatabase.Refresh();
        }

        //脚本编译完成回调
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (!EditorPrefs.GetBool(EEConverter.csChangedKey, false)) return;
            EditorPrefs.SetBool(EEConverter.csChangedKey, false);
            PDebug.Log("Scripts are reloaded, start generating assets...");
            EEConverter.GenerateScriptableObjects(CfgExcelDir, CfgsAssetDir);
        }

        //清理配置定义Cs 文件夹
        private static void DeleteGenedCfgCSFile()
        {
            //cs 
            if (Directory.Exists(CfgCSharpDir))
            {
                PFramework.FileUtil.ClearDirectory(CfgCSharpDir);
            }
            //inspector
            if (Directory.Exists(CfgCSharpInspectorDir))
            {
                PFramework.FileUtil.ClearDirectory(CfgCSharpInspectorDir);
            }
        }

        //清理配置资源文件夹
        private static void DeleteCfgScriptableObjectFolder()
        {
            if (Directory.Exists(CfgsAssetDir))
            {
                PFramework.FileUtil.ClearDirectory(CfgsAssetDir);
            }
        }


        /// <summary>
        /// 创建配置资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            string name = typeof(T).Name;
            string path = CfgsAssetLocalDir + name + ".asset";
            string fullPath = CfgsAssetDir + "/" + name + ".asset";
            if (System.IO.File.Exists(fullPath))
            {
                bool ret = EditorUtility.DisplayDialog("警告", "已经存在该配置确认覆盖！", "重置", "取消");
                if (ret)
                {
                    T asset = ScriptableObject.CreateInstance<T>();
                    AssetDatabase.CreateAsset(asset, path);
                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = asset;
                }
            }
            else
            {
                T asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }

        }

        [OnSettingsInit(prioty = 100)]
        public static void CreateResourcesDir()
        {
            if (!Directory.Exists(CfgCSharpDir))
            {
                PFramework.FileUtil.CreateDirectory(CfgCSharpDir);
            }

            if (!Directory.Exists(CfgCSharpInspectorDir))
            {
                PFramework.FileUtil.CreateDirectory(CfgCSharpInspectorDir);
            }

            if (!Directory.Exists(CfgExcelDir))
            {
                PFramework.FileUtil.CreateDirectory(CfgExcelDir);
            }

            if (!AssetDatabase.IsValidFolder($"Assets/{Peach.PeachAssetsDir}/Resources/Peach/Cfgs"))
            {
                AssetDatabase.CreateFolder($"Assets/{Peach.PeachAssetsDir}/Resources/Peach", "Cfgs");
            }

            if (!AssetDatabase.IsValidFolder($"Assets/{Peach.PeachAssetsDir}/Resources/Peach/Fonts"))
            {
                AssetDatabase.CreateFolder($"Assets/{Peach.PeachAssetsDir}/Resources/Peach", "Fonts");
            }
            AssetDatabase.Refresh();
        }
    }
}
