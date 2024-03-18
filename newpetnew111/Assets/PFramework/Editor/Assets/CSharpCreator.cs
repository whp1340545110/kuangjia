using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using System.IO;

namespace PeachEditor.Assets
{
#pragma warning disable 0618
    public partial class CSharpCreator : AssetModificationProcessor
    {
        [MenuItem("Assets/Create/A-Peach/C# Script",priority = 0)]
        public static void CreatePCShapScript()
        {
            CreateScript("NewScript.cs", "Peach/EditorTemplate/C#_Peach.cs");
        }


        [MenuItem("Assets/Create/A-Peach/Manager Script", priority = 0)]
        public static void CreateManagerScript()
        {
            CreateScript("NewManager.cs", "Peach/EditorTemplate/C#_Peach_Manager.cs");
        }

        public static void CreateScript(string scrtipName, string resouceFile)
        {
            string path = GetSelectedPath();
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateScriptAction>(),
                path + "/" + scrtipName, null, resouceFile);
        }

        private static string GetSelectedPath()
        {
            //默认路径为Assets
            string selectedPath = "Assets";

            //获取选中的资源
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
            if (selection.Length != 1)
                return "";
            //遍历选中的资源以返回路径
            foreach (Object obj in selection)
            {
                selectedPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(selectedPath) && File.Exists(selectedPath))
                {
                    selectedPath = Path.GetDirectoryName(selectedPath);
                    break;
                }
            }

            return selectedPath;
        }
    }
#pragma warning restore 0618

    public class CreateScriptAction : EndNameEditAction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="pathName"></param>
        /// <param name="content">文件模板</param>
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            //获取要创建资源的绝对路径
            string fullName = Path.GetFullPath(pathName);

            var textAssets = Resources.Load<TextAsset>(resourceFile);

            string content = textAssets.text;

            //获取资源的文件名
            string fileName = Path.GetFileNameWithoutExtension(pathName);

            //替换默认的文件名
            content = content.Replace("#NAME", fileName);

            File.WriteAllText(fullName, content);

            //刷新本地资源
            AssetDatabase.ImportAsset(pathName);
            AssetDatabase.Refresh();

            //创建资源
            Object obj = AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
            //高亮显示该资源
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }
}
