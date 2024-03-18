using System;
using System.Collections.Generic;
using PFramework;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace PeachEditor
{
    public class SettingsWindow : EditorWindow
    {
        const string SettingsShowStringFormat = "peach_settings_show_{0}";
        [MenuItem("PFramework/SettingsWindow", priority = 1000)]
        internal static void ShowWindow()
        {
            var window = GetWindow<SettingsWindow>();
            window.Show();
        }

        [SerializeField]
        private List<SerializedObject> _settingsObject = new List<SerializedObject>();

        [SerializeField]
        private List<AnimBool> _settingsAnimBools = new List<AnimBool>();

        [SerializeField]
        private List<Editor> _settingsEditor = new List<Editor>();

        private GUIStyle _titleStyle;
        private Vector2 _scrollStartPos = Vector2.zero;

        private void OnEnable()
        {
            titleContent = new GUIContent("SettingsWindow");
            GetSettings();
            SetTitleStyle();
        }

        private void SetTitleStyle()
        {
            _titleStyle = new GUIStyle();
            _titleStyle.fontSize = 20;
            _titleStyle.normal.textColor = new Color(46f / 256f, 163f / 256f, 256f / 256f, 256f / 256f);
            _titleStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnGUI()
        {
            if (_settingsObject.Count == 0)
            {
                if (GUILayout.Button("初始化"))
                {
                    SettingsInitializer.InitSettings();
                    GetSettings();
                }
                return;
            }
            try
            {
                ShowSettings();
            }
            catch (Exception e)
            {
                if (!(e is ExitGUIException))
                {
                    Debug.LogError(e);

                    Clear();

                    GUIUtility.ExitGUI();
                }
            }
        }

        private void ShowSettings()
        {
            _scrollStartPos = GUILayout.BeginScrollView(_scrollStartPos);
            for (int i = 0; i < _settingsObject.Count; i++)
            {
                SerializedObject serializedObject = _settingsObject[i];
                Editor editor = _settingsEditor[i];

                var settings = serializedObject.targetObject as SettingsBase;

                if (!settings.IsFade)
                {
                    GUILayout.Box(new GUIContent(settings.SettingsName), _titleStyle);
                    GUILayout.BeginVertical();
                    editor.OnInspectorGUI();
                    GUILayout.EndVertical();
                    continue;
                }

                AnimBool animBool = _settingsAnimBools[i];
                GUILayout.Box(new GUIContent((serializedObject.targetObject as SettingsBase).SettingsName), _titleStyle);

                var baseContent = animBool.target ? "收起" : "展开";

                if (GUILayout.Button(baseContent))
                {
                    animBool.target = !animBool.target;
                    PlayerPrefs.SetInt(string.Format(SettingsShowStringFormat, serializedObject.targetObject.name), animBool.target ? 1 : 0);
                    serializedObject.ApplyModifiedProperties();
                }
                if (animBool.target)
                {
                    GUILayout.BeginVertical();
                    editor.OnInspectorGUI();
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndScrollView();
        }

        private void GetSettings()
        {
            if (_settingsObject.Count > 0)
            {
                return;
            }
            Clear();
            var types = SettingsEditorUtility.FindAllSettingTypes();
            var settingsObjects = SettingsEditorUtility.GetSettingsList(types);
            if (types.Count != settingsObjects.Count)
            {
                return;
            }
            settingsObjects.Sort();
            foreach (var setting in settingsObjects)
            {
                _settingsObject.Add(new SerializedObject(setting));
                _settingsAnimBools.Add(new AnimBool(PlayerPrefs.GetInt(string.Format(SettingsShowStringFormat, setting.name), 0) == 1));
                Editor e = Editor.CreateEditor(setting);
                _settingsEditor.Add(e);
                e.hideFlags = HideFlags.None;
            }
        }

        private void OnDisable()
        {
            Clear();
        }

        private void Clear()
        {
            _settingsObject.Clear();
            _settingsAnimBools.Clear();
            _settingsEditor.ForEach((_) => DestroyImmediate(_));
            _settingsEditor.Clear();
        }
    }
}
