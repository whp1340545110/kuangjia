namespace PeachEditor.Page
{
    using PFramework.Page;
    using UnityEditor;
    using Assets;
	public class PageMenuItems
	{
		[MenuItem("GameObject/PFramework/DefaultCanvas", false, 10)]
		static void CreateDefaultCanvas(MenuCommand menuCommand)
		{
            var canvas = PageUtility.CreateCanvas(E_PageLayer.Default);
			Selection.activeObject = canvas.gameObject;//将新建物体设为当前选中物体
		}


        [MenuItem("Assets/Create/A-Peach/Page Script", priority = 0)]
        public static void CreatePageScript()
        {
            CSharpCreator.CreateScript("NewPage.cs", "Peach/EditorTemplate/C#_Peach_Page.cs");
        }
    }
}

