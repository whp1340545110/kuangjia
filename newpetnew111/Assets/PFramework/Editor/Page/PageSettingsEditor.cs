using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PFramework.Page;

namespace PeachEditor.Page
{
    [CustomEditor(typeof(PageSettings), true)]
    public class PageSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("targetScreenSize"), new GUIContent("目标屏幕比例"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isClearUIOnSceneUnload"), new GUIContent("切换场景时清空UI"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("orderSpace"), new GUIContent("不同层级之前UI的order间距"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraDepth"), new GUIContent("UI相机渲染层级"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraClearFlags"), new GUIContent("UI相机ClearFlag"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("orthographic"), new GUIContent("UI相机是否正交"), true);
            var isCameraTargetLayerMask = serializedObject.FindProperty("isUICameraTargetLayerMask");
            EditorGUILayout.PropertyField(isCameraTargetLayerMask, new GUIContent("UI是否指定裁剪LayerMask"), true);
            if (isCameraTargetLayerMask.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraCullingMask"), new GUIContent("UI相机裁剪LayerMask"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("uiLayer"), new GUIContent("UI指定Layer"), true);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
