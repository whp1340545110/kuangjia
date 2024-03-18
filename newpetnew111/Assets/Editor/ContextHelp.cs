using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace Star
{
    public static class ContextHelp
    {
        [MenuItem("CONTEXT/Image/addButton")]
        public static void SetButton(MenuCommand img)
        {
            var x = img.context as Image;
            var btn = x.gameObject.AddSingleComponent<Button>();
            var clickAudio = x.gameObject.AddSingleComponent<ClickAudio>();
            btn.transition = Selectable.Transition.Animation;
            var ani = x.gameObject.AddSingleComponent<Animator>();
            ani.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/AddressablesAssets/Common/commonBtn.controller");
        }
        [MenuItem("CONTEXT/Image/delButton")]
        public static void DelButton(MenuCommand img)
        {
            var x = img.context as Image;
            x.gameObject.RemoveComponent<Button>();
            x.gameObject.RemoveComponent<Animator>();
        }
        [MenuItem("CONTEXT/Text/addTextPro")]
        public static void SetTextPro(MenuCommand text)
        {
            var x = text.context as Text;
            var o = x.gameObject;
            var c = x.text;
            o.RemoveComponent<Text>();
            var p = o.AddComponent<TextMeshProUGUI>();
            p.text = c;
        }
        [MenuItem("CONTEXT/TextMeshProUGUI/delTextPro")]
        public static void DelTextPro(MenuCommand text)
        {
            var x = text.context as TextMeshProUGUI;
            var o = x.gameObject;
            var c = x.text;
            o.RemoveComponent<TextMeshProUGUI>();
            var p = o.AddComponent<Text>();
            p.text = c;
        }
        public static void RemoveComponent<T>(this GameObject ob)
            where T:Behaviour
        {
            var t = ob.GetComponent<T>();
            if (null != t)
                GameObject.DestroyImmediate(t);
        }
        public static T AddSingleComponent<T>(this GameObject ob)
            where T: Behaviour
        {
            var t = ob.GetComponent<T>();
            if (null == t)
                t = ob.AddComponent<T>();
            return t;
        }

        [MenuItem("Tools/AddReporter")]
        public static void AddReporter() { 
        
            GameObject go= AssetDatabase.LoadAssetAtPath<GameObject>("Assets/AddressablesAssets/Common/reporter.prefab");
            GameObject.Instantiate(go);
            go.name = "reporter";
        }
    }
}
