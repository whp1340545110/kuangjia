using UnityEngine;
using UnityEditor;
using System.IO;
using PeachEditor.Res;
using BuildResult = UnityEditor.Build.Reporting.BuildResult;

public class BuildSystem
{
    const string build_output_path = "C:/Users/EDZ/Desktop/ceshi";
    const string android_project_path = "C:/Users/EDZ/Desktop/ceshi1/lll";

   

    [MenuItem("Tools/BuildAction")]
    public static void Build()
    {
        var strintPath = build_output_path + "/"+"lll";
        bool isExist = Directory.Exists(strintPath);
        Debug.Log(isExist);
        Directory.Move(strintPath, android_project_path);
        return;
        PrepareAssets();

        if (Directory.Exists(build_output_path))
        {
            Directory.Delete(build_output_path, true);
        }

        Directory.CreateDirectory(build_output_path);

        var productName = PlayerSettings.productName;
        

        var buildReport = BuildPipeline.BuildPlayer(GetBuildScenes(), build_output_path, BuildTarget.Android, BuildOptions.None);

        var result = buildReport.summary.result;
        if (result == BuildResult.Succeeded)
        {
            var unityProjectPath= build_output_path;
            unityProjectPath = Path.Combine(unityProjectPath, "unityLibrary/src/main");

            var androidProjectPath = Path.Combine(android_project_path, "unityLibrary/src/main");

            ModeDir("assets", unityProjectPath, androidProjectPath);
            ModeDir("jniLibs", unityProjectPath, androidProjectPath);
        }

        EditorUtility.DisplayDialog("BuildOutput", buildReport.summary.result.ToString(), "ok");

        AssetDatabase.Refresh();
    }

    static void PrepareAssets()
    {
        var assetPath = Path.Combine(Application.dataPath, "AddressableAssetsData");
        if (Directory.Exists(assetPath))
        {
            Directory.Delete(assetPath, true);
        }

        AssetDatabase.Refresh();

        ResEditorUtility.AddGroups();
        ResEditorUtility.AutoBuildDefaultAddressables();
    }

    static EditorBuildSettingsScene[] GetBuildScenes()
    {
        return EditorBuildSettings.scenes;
    }

    static void ModeDir(string dirName,string unityProjectPath,string androidProjectPath)
    {
        var unityDirPath = Path.Combine(unityProjectPath, dirName);

        var androidDirPath = Path.Combine(androidProjectPath, dirName);
        if (Directory.Exists(androidDirPath))
        {
            Directory.Delete(androidDirPath,true);
        }
        File.Copy(unityDirPath, androidProjectPath);
        /*Directory.Move(unityDirPath, androidDirPath);*/
    }
}
