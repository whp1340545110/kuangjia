using UnityEngine;
using UnityEditor;
using PFramework.Data;
using System.IO;
using PFramework;

namespace PeachEditor.Data
{
    [CustomEditor(typeof(DataSettings))]
    public class DataSettingsEditor : Editor
    {
        private bool _isShowExtraSettings;
        private DataSettings _setttings;
        private void OnEnable()
        {
            _setttings = target as DataSettings;
        }

        public override void OnInspectorGUI()
        {
            _isShowExtraSettings = EditorGUILayout.Foldout(_isShowExtraSettings, "Extra Settings");
            if (_isShowExtraSettings)
            {
                ShowExtraSettings();
            }
            ShowToolButtons();
        }
        private void ShowExtraSettings()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dataRootPath"), new GUIContent("存档根目录名"), true);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isAesCrypted"), new GUIContent("是否进行AES加密"));
            if (_setttings.isAesCrypted)
            {
                EditorGUILayout.LabelField("密钥:   " + _setttings.aesCryptKey);
                if (GUILayout.Button("重新生成加密密钥"))
                {
                    serializedObject.FindProperty("aesCryptKey").stringValue = SecurityUtils.CreateAESCriptKey();
                }
            }
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void ShowToolButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("清空存档", GUILayout.Height(40), GUILayout.Width(200)))
            {

                var path = GetDataDirectoryPath();
                PFramework.FileUtil.ClearDirectory(path);
                PDebug.Log("存档已清空");
            }

            if (GUILayout.Button("打开存档目录", GUILayout.Height(40), GUILayout.Width(200)))
            {
                var path = GetDataDirectoryPath();

                System.Diagnostics.Process.Start(path);

            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("清空PlayerPrefs缓存", GUILayout.Height(40), GUILayout.Width(200)))
            {
                PlayerPrefs.DeleteAll();
                PDebug.Log("PlayerPrefs已清空");
            }
        }

        private string GetDataDirectoryPath()
        {
            var path = Path.Combine(Application.persistentDataPath, serializedObject.FindProperty("dataRootPath").stringValue);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}
