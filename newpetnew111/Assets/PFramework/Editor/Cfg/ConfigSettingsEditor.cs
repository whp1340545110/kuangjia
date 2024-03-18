using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PFramework.Cfg;
using System.IO;
using System;
using UnityEditor.AnimatedValues;
using PFramework;

namespace PeachEditor.Cfg
{
    [CustomEditor(typeof(CfgSettings))]
    public class ConfigSettingsEditor : Editor
    {
        private bool _isShowExtraSettings;
        private CfgSettings _cfgSettings;
        private void OnEnable()
        {
            _cfgSettings = (CfgSettings)target;
        }

        public override void OnInspectorGUI()
        {
            _isShowExtraSettings = EditorGUILayout.Foldout(_isShowExtraSettings, "Extra Settings");
            if (_isShowExtraSettings)
            {
                ShowExtraSettings();
            }
            ShowToolsButton();

        }

        private void ShowExtraSettings()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            ShowConfigScriptablePath();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            ShowCfgNameSpace();
            ShowCfgLoadPath();
            ShowLocalFontsPath();

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void ShowLocalFontsPath()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent("字体资源加载路径"));
            var prop = serializedObject.FindProperty("localFontsLoadPath");
            prop.stringValue = EditorGUILayout.DelayedTextField(prop.stringValue);
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        private void ShowCfgLoadPath()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent("配置资源加载路径"));
            var prop = serializedObject.FindProperty("cfgAssetsLoadPath");
            prop.stringValue = EditorGUILayout.DelayedTextField(prop.stringValue);
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        private void ShowToolsButton()
        {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("打开Excel目录", GUILayout.Height(40), GUILayout.Width(200)))
            {
                var path = Path.Combine(CfgEditor.CfgExcelDir);
                System.Diagnostics.Process.Start(path);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("生成配置代码和资源", GUILayout.Height(40), GUILayout.Width(200)))
            {
                CfgEditor.ImportExcel();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("刷新配置资源", GUILayout.Height(40), GUILayout.Width(200)))
            {
                CfgEditor.RefreshExcelAsset();
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("删除代码和资源", GUILayout.Height(40), GUILayout.Width(200)))
            {
                CfgEditor.Clean();
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndVertical();
        }

        private void ShowConfigScriptablePath()
        {
            ShowLocalPath("配置资源生成路径", "cfgAssetDir");
        }

        private void ShowCfgNameSpace()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent("配置代码命名空间"));
            var prop = serializedObject.FindProperty("cfgCodeNamespace");
            prop.stringValue = EditorGUILayout.DelayedTextField(prop.stringValue);
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }

        private void ShowLocalPath(string title, string propName)
        {
            GUILayout.BeginVertical();
            GUILayout.Label(new GUIContent(title));
            var prop = serializedObject.FindProperty(propName);
            GUILayout.Label(new GUIContent(prop.stringValue));
            if (GUILayout.Button("选择", GUILayout.Width(200)))
            {
                var path = EditorUtility.OpenFolderPanel(title, prop.stringValue, "");
                var parentDir = Path.Combine(Application.dataPath, Peach.PeachAssetsDir);
                if (path.Contains(parentDir))
                {
                    var temp = path.Replace(parentDir, "");
                    if (temp.Length > 1 && temp[0] == '/')
                    {
                        temp = temp.Substring(1);
                    }
                    serializedObject.FindProperty(propName).stringValue = temp;
                }
                else
                {
                    EditorUtility.DisplayDialog("错误的路径", $"请选择工程{Peach.PeachAssetsDir}文件夹下的路径", "确定");
                }
            }
            GUILayout.Space(20);
            GUILayout.EndVertical();
        }
    }
}
