using System.Collections;
using System.Collections.Generic;
using PFramework.Page;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace PeachEditor.Page
{
    [CustomEditor(typeof(PageBase), true)]
    public class PageBaseEditor : Editor
    {
        private AnimBool _showPopupField;
        private AnimBool _showPageField;
        private AnimBool _showAniFiled;
        private AnimBool _showBaseFiled;
        private PageBase _page;
        private void OnEnable()
        {
            _page = target as PageBase;
            _showPopupField = new AnimBool(false);
            _showPopupField.valueChanged.AddListener(Repaint);
            _showPageField = new AnimBool(false);
            _showPageField.valueChanged.AddListener(Repaint);
            _showBaseFiled = new AnimBool(false);
            _showBaseFiled.valueChanged.AddListener(Repaint);
            _showAniFiled = new AnimBool(false);
            _showAniFiled.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 20;
            titleStyle.normal.textColor = new Color(46f / 256f, 163f / 256f, 256f / 256f, 256f / 256f);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Box(new GUIContent("Common"), titleStyle);
            var baseContent = _showBaseFiled.target ? "收起" : "展开";
            if (GUILayout.Button(baseContent))
            {
                _showBaseFiled.target = !_showBaseFiled.target;
            }
            if (EditorGUILayout.BeginFadeGroup(_showBaseFiled.faded))
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box(new GUIContent("弹窗属性"), titleStyle);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_isPopup"), new GUIContent("是否为弹窗"));
                if (_page.isPopup)
                {
                    var popupcContent = _showPopupField.target ? "收起" : "展开";
                    if (GUILayout.Button(popupcContent))
                    {
                        _showPopupField.target = !_showPopupField.target;
                    }
                    if (EditorGUILayout.BeginFadeGroup(_showPopupField.faded))
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_isStackPopup"), new GUIContent("弹窗堆栈"));
                    }
                    EditorGUILayout.EndFadeGroup();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Box(new GUIContent("页面属性"), titleStyle);
                EditorGUILayout.EndHorizontal();
                var pageContent = _showPageField.target ? "收起" : "展开";
                if (GUILayout.Button(pageContent))
                {
                    _showPageField.target = !_showPageField.target;
                }
                if (EditorGUILayout.BeginFadeGroup(_showPageField.faded))
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_isSingleton"), new GUIContent("单例"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_isCached"), new GUIContent("缓存"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_isCloseOnSceneUnload"), new GUIContent("卸载场景时关闭"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_isAutoRelease"), new GUIContent("关闭时自动释放资源"));
                    if (!_page.isPopup)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_uiLayer"), new GUIContent("层级"));
                    }
                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                GUILayout.Box(new GUIContent("动画"), titleStyle);
                var aniContent = _showAniFiled.target ? "收起" : "展开";
                if (GUILayout.Button(aniContent))
                {
                    _showAniFiled.target = !_showAniFiled.target;
                }
                if (EditorGUILayout.BeginFadeGroup(_showAniFiled.faded))
                {
                    var useAniProperty = serializedObject.FindProperty("_useAnimation");
                    EditorGUILayout.PropertyField(useAniProperty, new GUIContent("使用动画"));
                    if (useAniProperty.boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_createAni"), new GUIContent("弹出动画"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_closeAni"), new GUIContent("关闭动画"));
                    }

                }
                EditorGUILayout.EndFadeGroup();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();
            GUILayout.Box(new GUIContent("Target"), titleStyle);
            DrawPropertiesExcluding(serializedObject, new string[] { "_isAutoRelease","_isCloseOnSceneUnload", "_isPopup", "_isStackPopup", "_isSingleton", "_isCached", "_uiLayer", "_useAnimation", "_createAni", "_resumeAni", "_closeAni" });
            serializedObject.ApplyModifiedProperties();
        }
    }
}
