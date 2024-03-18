using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildPathSettings : ScriptableObject
{
    public string buildPath = "";
    public string copyPath = "";
    
    [MenuItem("Build/Android-Mono")]
    public static void BuildMonoAndroid()
    {
        BuildExportAndroid(ScriptingImplementation.Mono2x);
    }

    [MenuItem("Build/Android-IL2CPP")]
    public static void BuildCPPAndroid()
    {
        BuildExportAndroid(ScriptingImplementation.IL2CPP);
    }
    
    public static void BuildExportAndroid(ScriptingImplementation scriptingImplementation)
    {
        if (!AssetDatabase.IsValidFolder("Assets/BuildTools"))
        {
            AssetDatabase.CreateFolder("Assets", "BuildTools");
        }
        var so = AssetDatabase.LoadAssetAtPath<BuildPathSettings>("Assets/BuildTools/BuildPathSettings.asset");
        if (!so)
        {
            so = BuildPathSettings.CreateInstance<BuildPathSettings>();
            AssetDatabase.CreateAsset(so, "Assets/BuildTools/BuildPathSettings.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("拷贝失败", "编辑你的项目路径", "编辑");
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/BuildTools/BuildPathSettings.asset");
            return;
        }
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, scriptingImplementation);

        if (scriptingImplementation == ScriptingImplementation.IL2CPP)
        {
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        }
        else
        {
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
        }
        AssetDatabase.Refresh();
        
        string path = so.buildPath;
        if (System.IO.Directory.Exists(path))
        {
            System.IO.Directory.Delete(path, true);
        }
        System.IO.Directory.CreateDirectory(path);
        //
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);



        PlayerSettings.enableInternalProfiler = false;
        BuildOptions option = BuildOptions.AcceptExternalModificationsToPlayer;
        BuildResult result = BuildPipeline.BuildPlayer(GetScenePaths(true), path, BuildTarget.Android, option).summary.result;
        //
        if (result != BuildResult.Succeeded)
        {
            Debug.LogError("BuildAndroidProj Error");

        }
        else
        {
            Debug.Log("BuildAndroidProj Done!");
            CopyBuild();
        }
    }
    
    /// 获取出档的场景列表
    static EditorBuildSettingsScene[] GetScenePaths(bool isFilter)
    {
        var names = new List<EditorBuildSettingsScene>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;

            if (!e.enabled)
                continue;
            names.Add(e);
        }
        return names.ToArray();
    }
    
    public static void CopyBuild()
    {
        var settings = AssetDatabase.LoadAssetAtPath<BuildPathSettings>("Assets/BuildTools/BuildPathSettings.asset");
        EditorUtility.DisplayCancelableProgressBar("执行中...", "拷贝中", 0);
        CopyDir(Path.Combine(settings.buildPath, PlayerSettings.productName, "src/main/assets"), settings.copyPath);
        CopyDir(Path.Combine(settings.buildPath, PlayerSettings.productName, "src/main/jniLibs"), settings.copyPath);
        EditorUtility.ClearProgressBar();
    }
    
    public static void CopyDir(string srcPath, string aimPath)
    {
        try
        {

            //如果不存在目标路径，则创建之

            if (!System.IO.Directory.Exists(aimPath))
            {
                System.IO.Directory.CreateDirectory(aimPath);
            }
            //令目标路径为aimPath\srcPath
            string srcdir = System.IO.Path.Combine(aimPath, System.IO.Path.GetFileName(srcPath));
            //如果源路径是文件夹，则令目标目录为aimPath\srcPath\
            if (Directory.Exists(srcPath))
                srcdir += Path.DirectorySeparatorChar;
            // 如果目标路径不存在,则创建目标路径
            if (System.IO.Directory.Exists(srcdir))
            {
                Directory.Delete(srcdir, true);
            }
            System.IO.Directory.CreateDirectory(srcdir);
            //获取源文件下所有的文件
            String[] files = Directory.GetFileSystemEntries(srcPath);
            //int index = 0;
            foreach (string element in files)
            {
                //如果是文件夹，循环
                if (Directory.Exists(element))
                    CopyDir(element, srcdir);
                else
                    File.Copy(element, srcdir + Path.GetFileName(element), true);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

}