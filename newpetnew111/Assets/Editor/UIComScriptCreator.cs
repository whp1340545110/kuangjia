using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class UIComScriptCreator : Editor
{
    private enum eComType
    {
        eNull,
        eTextPro,
        eText,
        eRectTransfrom,
        eImg,
        eButton,
        eParticleSystem,
        eSkeletonGraphic,
    }
    private static Dictionary<string, eComType> _str2TypeDic = new Dictionary<string, eComType>
    {
        { "_txtpro", eComType.eTextPro },
        { "_txt", eComType.eText },
        { "_tr", eComType.eRectTransfrom },
        { "_img", eComType.eImg },
        { "_btn", eComType.eButton },
        { "_ps", eComType.eParticleSystem },
        { "_sp",eComType.eSkeletonGraphic},
    };
    private static Dictionary<eComType, string> _typeStrsDic = new Dictionary<eComType, string>()
    {
        { eComType.eTextPro, "TextMeshProUGUI"},
        { eComType.eText, "Text"},
        { eComType.eImg, "Image"},
        { eComType.eRectTransfrom, "RectTransform"},
        { eComType.eButton, "Button"},
        { eComType.eParticleSystem, "ParticleSystem"},
        { eComType.eSkeletonGraphic,"SkeletonGraphic"},
    };
    private const string _scriptComContent = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PVM
{{
    public partial class {0} : MonoBehaviour
    {{
        {1}
        public static {0} Get(GameObject obj)
        {{
            var mono = obj.GetComponent<{0}>();
            if (mono == null)
            {{
                mono = obj.AddComponent<{0}>();
                mono.BindCom();
            }}
            return mono;
        }}
        private void BindCom()
        {{
            {2}
        }}
    }}
}}";
    private const string _scriptContent = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spine.Unity;
using PFramework.Page;

namespace PVM
{{
    public partial class {0} : PageBase
    {{
        {1}
        public void BindCom()
        {{
            {2}
        }}
    }}
}}";
    private const string _comStr = @"[SerializeField] private {0} {1};
        ";
    private const string _comPathStr = @"{0} = transform.Find(""{1}"").GetComponent<{2}>();
            ";
    private const string _scriptPath = "Scripts/Auto/{0}Part.cs";

    [MenuItem("Tools/AutoGenerateUI _F5")]
    public static void AutoUIScript()
    {
        Debug.Log("pppppppppp");
        AutoScript(false);
    }
    [MenuItem("Tools/AutoGenerateItem _F1")]
    public static void AutoComScript()
    {
        AutoScript(true);
    }
    private static void AutoScript(bool _isCom)
    {
        var ins = Selection.activeGameObject;
        if (ins == null)
        {
            Debug.LogError("please choose a ui node");
            return;
        }
        var className = ins.name;
        var lst = ins.GetComponentsInChildren<Transform>(true);
        var comStrs = "";
        var bindStrs = "";
        for (int i = 0; i < lst.Length; i++)
        {
            var _itemPropertyStr = GetPropertyStrs(lst[i]);
            if (!string.IsNullOrEmpty(_itemPropertyStr))
            {
                comStrs += _itemPropertyStr;
                bindStrs += GetMethodStrs(lst[i]);
            }
        }
        var _scriptContentStrs = _isCom ? _scriptComContent : _scriptContent;
        var _scriptAllContent = string.Format(_scriptContentStrs, className, comStrs, bindStrs);
        if (!string.IsNullOrEmpty(comStrs) && !string.IsNullOrEmpty(bindStrs))
        {
            CreateFile(_scriptAllContent, className);
        }
    }
    private static string GetPropertyStrs(Transform child)
    {
        var singlePath = "";
        var childName = child.name;
        var _type = GetComType(childName);
        if (_type != eComType.eNull)
        {
            singlePath = string.Format(_comStr, _typeStrsDic[_type], childName);
        }
        return singlePath;
    }
    private static string GetMethodStrs(Transform child)
    {
        var path = child.name;
        var cur = child;
        while (cur.parent != null)
        {
            cur = cur.parent;
            if (cur.gameObject.Equals(Selection.activeGameObject))
            {
                break;
            }
            path = path.Insert(0, cur.name + "/");
        }
        return string.Format(_comPathStr, child.name, path, _typeStrsDic[GetComType(child.name)]);
    }
    private static eComType GetComType(string name)
    {
        var type = eComType.eNull;
        var startIndex = name.LastIndexOf("_");
        if (startIndex != -1)
        {
            var flag = name.Substring(startIndex);
            if (_str2TypeDic.ContainsKey(flag))
            {
                type = _str2TypeDic[flag];
            }
        }
        return type;
    }
    private static void CreateFile(string content, string className)
    {
        var allPath = Application.dataPath + "/" + string.Format(_scriptPath, className);
        if (File.Exists(allPath))
        {
            File.Delete(allPath);
        }
        File.WriteAllText(allPath, content);
        Debug.Log("generate success");
        AssetDatabase.Refresh();
    }
}