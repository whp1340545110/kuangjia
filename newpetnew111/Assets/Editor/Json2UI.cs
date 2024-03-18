using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using LitJson;
using System.IO;
using TMPro;

namespace poke
{
    public class Json2UI : Editor
    {
        public enum eArtType
        {
            eImg,
            eText,
        }

        [MenuItem("Tools/json2UI")]
        public static void CreateUI()
        {
            var select = Selection.activeObject as TextAsset;
            if (null == select)
            {
                Debug.LogError("选中对应的json文件");
                return;
            }
            var ob = LitJson.JsonMapper.ToObject(select.text);
            ComNode node = new ComNode().Deserialize(ob) as ComNode;
            var root = GameObject.Find("UGUI/Root");
            var viewOb = (new ComGob(node, root.transform)).GetOb();
            viewOb.gameObject.layer = LayerMask.NameToLayer("UI");
            var transe = viewOb.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transe.Length; i++)
            {
                transe[i].gameObject.layer = LayerMask.NameToLayer("UI");
            }
        }

        #region json2UIOb
        private class ComGob
        {
            private GameObject mOb_ = null;
            public ComGob(ComNode node, Transform parent)
            {
                var ob = new GameObject(node.Name);
                ob.AddComponent<RectTransform>();
                ob.transform.SetParent(parent);
                ob.transform.localScale = Vector3.one;
                ob.transform.localPosition = Vector3.zero;
                ob.transform.localRotation = Quaternion.identity;
                for (int i = 0; i < node.comNodes.Count; i++)
                {
                    var item = node.comNodes[i];
                    if (item.isCom)
                        new ComGob(item as ComNode, ob.transform);
                    else
                        new ArtGob(item as ArtNode, ob.transform);
                }
                mOb_ = ob;
            }
            public GameObject GetOb()
            {
                return mOb_;
            }
        }
        private class ArtGob
        {
            private static string[] mFindPath = new string[] { "Assets/AddressablesAssets" };
            public ArtGob(ArtNode node, Transform parent)
            {
                var ob = new GameObject(node.Name);
                ob.transform.SetParent(parent);
                ob.transform.localScale = Vector3.one;
                ob.transform.localPosition = node.GetMiddlePoint();
                ob.transform.localRotation = Quaternion.identity;
                switch (node.rootType)
                {
                    case eArtType.eImg:
                        var img = ob.AddComponent<Image>();
                        var paths = GetFileNamePath(node.Name + ".png", "t:sprite", mFindPath);
                        if (paths.Count > 0)
                        {
                            img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(paths[0]);
                            img.type = img.sprite.border != Vector4.zero ? Image.Type.Sliced : Image.Type.Simple;
                            Debug.Log(paths[0]);
                        }
                        img.rectTransform.sizeDelta = new Vector2(node.bound.width, node.bound.height);
                        break;
                    case eArtType.eText:
                        var tex = ob.AddComponent<Text>();
                        tex.text = node.Name;
                        tex.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/AddressablesAssets/Common/Font/aachenn.otf");
                        tex.rectTransform.sizeDelta = new Vector2(node.bound.width, node.bound.height);
                        tex.color = node.color;
                        Debug.Log(tex.color);
                        tex.alignment = TextAnchor.MiddleCenter;
                        tex.horizontalOverflow = HorizontalWrapMode.Overflow;
                        tex.verticalOverflow = VerticalWrapMode.Overflow;
                        /*     tex.enableWordWrapping = true;*/
                        /* tex.alignment = TextAlignmentOptions.Center;*/
                        tex.fontSize = (int)node.texSize;
                        break;
                    default:
                        break;
                }
            }
        }

        public class NodeBase
        {
            public bool isCom;
            public string Name;
            virtual public NodeBase Deserialize(LitJson.JsonData jd)
            {
                isCom = bool.Parse(jd["isCom"].ToString());
                Name = jd["Name"].ToString();
                return this;
            }
        }

        public class ComNode : NodeBase
        {
            public List<NodeBase> comNodes = new List<NodeBase>();
            public override NodeBase Deserialize(JsonData jd)
            {
                base.Deserialize(jd);
                var nodes = jd["comNodes"] as JsonData;
                for (int i = 0; i < nodes.Count; i++)
                {
                    var item = nodes[i];
                    if (bool.Parse(item["isCom"].ToString()))
                        comNodes.Add(new ComNode().Deserialize(item));
                    else
                        comNodes.Add(new ArtNode().Deserialize(item));
                }
                return this;
            }
        }

        public class ArtNode : NodeBase
        {
            public eArtType rootType;
            public Rect bound;
            public Color color;
            public float texSize;


            public override NodeBase Deserialize(JsonData jd)
            {
                base.Deserialize(jd);
                rootType = (eArtType)int.Parse((jd["rootType"].ToString()));
                var jsB = jd["bound"] as JsonData;
                bound = new Rect(
                    float.Parse(jsB[0].ToString()),
                    float.Parse(jsB[1].ToString()),
                    float.Parse(jsB[2].ToString()),
                    float.Parse(jsB[3].ToString())
                    );
                var jsC = jd["color"] as JsonData;
                color = new Color(
                    float.Parse(jsB[0].ToString()),
                    float.Parse(jsB[1].ToString()),
                    float.Parse(jsB[2].ToString()),
                    float.Parse(jsB[3].ToString())
                    );
                texSize = float.Parse(jd["texSize"].ToString());
                return this;
            }
            public Vector3 GetMiddlePoint()
            {
              /*  return new Vector3(bound.x - 360 + (bound.width / 2), 640 - bound.y  - (bound.height / 2), 0);  //720*1280*/
                return new Vector3(bound.x - 540 + (bound.width / 2), 960 - bound.y - (bound.height / 2), 0);  //1080*1920
                //return new Vector3(bound.x - 480 + (bound.width / 2), 720 - bound.y  - (bound.height / 2), 0);  //960*1440
            }
        }

        public static List<string> GetFileNamePath(string extentionName, string match, string[] searchPath)
        {
            var f_info = new FileInfo(extentionName);
            var l = new List<string>();
            var paths = AssetDatabase.FindAssets(match + " " + extentionName.Replace(f_info.Extension, ""), searchPath);
            for (int i = 0; i < paths.Length; i++)
            {
                var p = AssetDatabase.GUIDToAssetPath(paths[i]);
                FileInfo info = new FileInfo(p);
                if (info.Name.Equals(f_info.Name))
                    l.Add(p);
            }
            return l;
        }
        #endregion
    }
}
