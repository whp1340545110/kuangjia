using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PFramework;
using System.Reflection;
using UnityEditor.AnimatedValues;
using System;

namespace PeachEditor
{
    [CustomEditor(typeof(PeachSettings))]
    public class MainSettingsEditor : Editor
    {
        private PeachSettings _mainSettings;
        private List<string> _workingList;
        private List<Type> _managerTypes;

        private AnimBool _isShowResMgr;
        private AnimBool _isShowOtherMgr;

        [SerializeField]
        private bool _isEventMgrEditable;
        [SerializeField]
        private bool _isResMgrEditable;
        [SerializeField]
        private bool _isOtherMgrEditable;
        [SerializeField]
        private int _toolBarIndex;

        private void OnEnable()
        {
            _isShowResMgr = new AnimBool(false);
            _isShowOtherMgr = new AnimBool(false);
            _mainSettings = target as PeachSettings;
            InitWorkingSymbols();
            InitManagers();
            Debug.Log("=========================");
        }

        private void InitWorkingSymbols()
        {
            _workingList = new List<string>();
#if UNITY_IOS
            var workingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
#elif UNITY_ANDROID
            var workingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
#else
            var workingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Unknown);
#endif
            _workingList.AddRange(workingSymbols.Split(';'));
        }

        private void InitManagers()
        {
            _managerTypes = new List<System.Type>();
            var asmbs = PEditorUtility.GetRunTimeAssemblies();
            foreach(var asmb in asmbs)
            {
                var types = asmb.GetTypes();
                foreach (var aType in types)
                {
                    if (aType.GetInterface(typeof(IManager).Name) != null && !aType.IsAbstract && !aType.IsInterface)
                    {
                        _managerTypes.Add(aType);
                    }
                }
            }

            //移除掉命名空间中没有的Type
            List<string> removes = new List<string>();
            foreach(var str in _mainSettings.otherManagers)
            {
                if (_managerTypes.FindIndex(0, _managerTypes.Count, (t) => { return t.FullName == str; }) == -1)
                {
                    removes.Add(str);
                }
            }

            foreach(var removal in removes)
            {
                var index = _mainSettings.otherManagers.IndexOf(removal);
                var otherManagersProperty = serializedObject.FindProperty("otherManagers");
                otherManagersProperty.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            _toolBarIndex = GUILayout.Toolbar(_toolBarIndex, new string[] { "自定义宏","管理器"});
            if (EditorGUI.EndChangeCheck())
            {
                GUI.FocusControl(null);
            }

            EditorGUI.BeginChangeCheck();
            switch (_toolBarIndex)
            {
                case 0:
                    ShowSymbolsSettings();
                    break;
                case 1:
                    ShowManagerSettings();
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

        }

        private void ShowSymbolsSettings()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("customerDefineSymbols"), new GUIContent("预定义"), true);
            foreach (var str in _mainSettings.customerDefineSymbols)
            {

                bool use = _workingList.Contains(str);
                var newUse = EditorGUILayout.Toggle(str, use);
                if (!newUse)
                {
                    _workingList.Remove(str);
                }
                else if (!use && newUse)
                {
                    _workingList.Add(str);
                }
            }
            if (GUILayout.Button(new GUIContent("保存"), GUILayout.Width(100)))
            {
                var newSymbols = "";
                foreach (var str in _workingList)
                {
                    newSymbols += str;
                    newSymbols += ";";
                }
#if UNITY_IOS
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, newSymbols);
#elif UNITY_ANDROID
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, newSymbols);
#endif
            }
        }

        private void ShowManagerSettings()
        {
            var resManagers = _managerTypes.FindAll((t) =>
            {
                var subclassNames = PEditorUtility.GetSubClassName(typeof(ResManagerBase));
                return subclassNames.Contains(t.Name);
            });
            var others = _managerTypes.FindAll((t) => !resManagers.Contains(t));

            if (GUILayout.Button("资源管理器", GUILayout.Width(100)))
            {
                _isShowResMgr.target = !_isShowResMgr.target;
            }
            if (_isShowResMgr.target)
            {
                _isResMgrEditable = EditorGUILayout.BeginToggleGroup("", _isResMgrEditable);
                var prop = serializedObject.FindProperty("resManager");
                foreach (var aType in resManagers)
                {
                    var isUsing = prop.stringValue == aType.FullName;
                    bool newUse = GUILayout.Toggle(isUsing, aType.FullName);
                    if (!isUsing && newUse)
                    {
                        prop.stringValue = aType.FullName;
                    }
                }
                EditorGUILayout.EndToggleGroup();
            }

            ShowOtherManagerToggles(others);
        }

        private void ShowOtherManagerToggles(List<Type> others)
        {
            //Debug.Log("Before Tab =" + _mainSettings.currentTab);
            if (GUILayout.Button("其他管理器", GUILayout.Width(100)))
            {
                _isShowOtherMgr.target = !_isShowOtherMgr.target;
            }
            if (_isShowOtherMgr.target)
            {
                _isOtherMgrEditable = EditorGUILayout.BeginToggleGroup("", _isOtherMgrEditable);
                List<List<System.Type>> vertical = new List<List<System.Type>>();

                for (int i = 0; i < others.Count; i++)
                {
                    var verticalIndex = i / 3;
                    if (vertical.Count <= verticalIndex)
                    {
                        vertical.Add(new List<System.Type>());
                    }
                    vertical[verticalIndex].Add(others[i]);
                }
                EditorGUILayout.BeginVertical();

                foreach (var horizontal in vertical)
                {
                    EditorGUILayout.BeginHorizontal();
                    foreach (var aType in horizontal)
                    {
                        var isUsing = _mainSettings.otherManagers.Contains(aType.FullName);
                        bool newUse = GUILayout.Toggle(isUsing, aType.FullName, GUILayout.ExpandWidth(true));
                        if (!isUsing && newUse)
                        {
                            var otherManagersProperty = serializedObject.FindProperty("otherManagers");
                            var size = otherManagersProperty.arraySize;
                            otherManagersProperty.InsertArrayElementAtIndex(size);
                            otherManagersProperty.GetArrayElementAtIndex(size).stringValue = aType.FullName;
                        }
                        else if (isUsing && !newUse)
                        {
                            var index = _mainSettings.otherManagers.IndexOf(aType.FullName);
                            var otherManagersProperty = serializedObject.FindProperty("otherManagers");
                            otherManagersProperty.DeleteArrayElementAtIndex(index);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndToggleGroup();
            }
        }

        [OnSettingsInit(prioty = 100)]
        public static void OnSettingsInitCallback()
        {
            var managerTypes = new List<Type>();
            var asmbs = PEditorUtility.GetRunTimeAssemblies();
            foreach(var asmb in asmbs)
            {
                var types = asmb.GetTypes();
                foreach (var aType in types)
                {
                    if (aType.GetInterface(typeof(IManager).Name) != null && !aType.IsAbstract && !aType.IsInterface)
                    {
                        managerTypes.Add(aType);
                    }
                }
            }

            var resManagers = managerTypes.FindAll((t) =>
            {
                var subclassNames = PEditorUtility.GetSubClassName(typeof(ResManagerBase));
                return subclassNames.Contains(t.Name);
            });

            var settingsSerilizedObject = new SerializedObject(Peach.GetSettings<PeachSettings>());

            settingsSerilizedObject.FindProperty("resManager").stringValue = resManagers[0].FullName;

            managerTypes.RemoveAll(resManagers.Contains);

            var otherManagersProperty = settingsSerilizedObject.FindProperty("otherManagers");
            otherManagersProperty.ClearArray();
            foreach(var manager in managerTypes)
            {
                var size = otherManagersProperty.arraySize;
                otherManagersProperty.InsertArrayElementAtIndex(size);
                otherManagersProperty.GetArrayElementAtIndex(size).stringValue = manager.FullName;
            }
            settingsSerilizedObject.ApplyModifiedProperties();
            AssetDatabase.Refresh();
        }
    }
}
