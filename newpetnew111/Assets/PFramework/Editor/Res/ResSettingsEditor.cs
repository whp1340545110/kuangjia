namespace PeachEditor.Res
{
    using UnityEngine;
    using UnityEditor;
    using PFramework.Res;
    [CustomEditor(typeof(ResSettings), true)]
    public class ResSettingsEditor : Editor
    {
        private bool _isShowExtraSettings;
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("aaRootAssetPath"), new GUIContent("Addressables资源根目录"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("netResCachePath"), new GUIContent("网络下载资源根目录"), true);
            var isShowWarnning = serializedObject.FindProperty("isShowWarnning");
            EditorGUILayout.PropertyField(isShowWarnning, new GUIContent("添加Group是否需要提示"), true);
            if (isShowWarnning.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("commonAssetsGroup"), new GUIContent("公共资源Group列表"), true);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowToolButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("AABuild", GUILayout.Height(40), GUILayout.Width(200)))
            {
                ResEditorUtility.AddGroups();
                ResEditorUtility.AutoBuildDefaultAddressables();
                GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("清空网络资源缓存", GUILayout.Height(40), GUILayout.Width(200)))
            {
                ResUtility.ClearNetResCache();
            }
        }
    }
}
