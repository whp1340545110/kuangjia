namespace PeachEditor.Res
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Build;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEditor.Build.Pipeline.Utilities;
    using UnityEngine;
    using PFramework.Res;
    using System.Text;
    using UnityEditor.AddressableAssets.Settings.GroupSchemas;

    public class ResEditorUtility
    {
        private static ResSettings _settings;
        public static ResSettings Settings
        {
            get
            {
                if(_settings == null)
                {
                    _settings = ResSettings.GetSettings<ResSettings>();
                }
                return _settings;
            }
        }

        public static string AARootAssetsPath = ResSettings.GetSettings<ResSettings>().AARootPath;

        public static void AutoBuildDefaultAddressables()
        {
            OnCleanAll();
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            for (int i = 0; i < settings.DataBuilders.Count; i++)
            {
                var builder = settings.GetDataBuilder(i);
                if (builder.CanBuildData<AddressablesPlayerBuildResult>() && builder.Name == "Default Build Script")
                {
                    OnBuildScript(i);
                    break;
                }
            }
        }

        static void OnCleanAll()
        {
            OnCleanAddressables(null);
        }

        static void OnCleanAddressables(object builder)
        {
            AddressableAssetSettings.CleanPlayerContent(builder as IDataBuilder);
        }

        static void OnCleanSBP()
        {
            BuildCache.PurgeCache(false);
        }


        static void OnBuildScript(object context)
        {
            OnSetActiveBuildScript(context);
            OnBuildPlayerData();
        }

        static void OnBuildPlayerData()
        {
            AddressableAssetSettings.BuildPlayerContent();
        }

        static void OnSetActiveBuildScript(object context)
        {
            AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = (int)context;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void AddGroups()
        {
            EditorUtility.DisplayCancelableProgressBar("执行中...", "生成groups", 0);

            if (AddressableAssetSettingsDefaultObject.Settings == null)
            {
                AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create(AddressableAssetSettingsDefaultObject.kDefaultConfigFolder, AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
            }

            var groupRootPathes = AssetDatabase.GetSubFolders(Settings.AARootPath);
            foreach (var groupRootPath in groupRootPathes)
            {
                CreateGroup(groupRootPath);
            }

            var groups = new List<AddressableAssetGroup>();
            groups.AddRange(AddressableAssetSettingsDefaultObject.Settings.groups);


            foreach (var group in groups)
            {
                if(group.entries.Count == 0)
                {
                    AddressableAssetSettingsDefaultObject.Settings.RemoveGroup(group);
                }
            }

            EditorUtility.ClearProgressBar();

            //这个地方要检测所有资源文件夹下的依赖关系，如果某个文件夹依赖了其他文件夹下的文件需要给出提示
            CheckDependence(groupRootPathes);
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        private static void CreateGroup(string groupRootPath)
        {
            var groupName = GetFolderName(groupRootPath);

            var settings = AddressableAssetSettingsDefaultObject.Settings;

            var group = settings.FindGroup(groupName);

            if (group == null)
            {
                AddressableAssetGroupTemplate groupTemplate = settings.GetGroupTemplateObject(0) as AddressableAssetGroupTemplate;
                group = settings.CreateGroup(groupName, false, false, true, null, groupTemplate.GetTypes());
                groupTemplate.ApplyToAddressableAssetGroup(group);
                group.GetSchema<BundledAssetGroupSchema>().Compression = BundledAssetGroupSchema.BundleCompressionMode.LZMA;
            }
            List<AddressableAssetEntry> entries = new List<AddressableAssetEntry>(group.entries);
            foreach (var entry in entries)
            {
                group.RemoveAssetEntry(entry);
            }

            SetAssetsEntris(groupRootPath, group);
        }

        private static void SetAssetsEntris(string folder, AddressableAssetGroup group)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            var assetsPath = new List<string>();
            if (Directory.Exists(folder))
            {
                DirectoryInfo directory = new DirectoryInfo(folder);
                FileInfo[] files = directory.GetFiles("*", SearchOption.TopDirectoryOnly);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta") || files[i].Name.Equals(".DS_Store"))
                    {
                        continue;
                    }
                    assetsPath.Add(files[i].Name);
                }

                DirectoryInfo[] directories = directory.GetDirectories();
                foreach (var dir in directories)
                {
                    if (dir.Name.IndexOf("_") == 0)
                    {
                        assetsPath.Add(dir.Name);
                    }
                }
            }
            foreach (var assetPath in assetsPath)
            {
                var rootPath = Settings.AARootPath;
                var asset = AssetDatabase.LoadAssetAtPath<Object>(Path.Combine(folder, assetPath));
                string guid;
                long localId;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out guid, out localId))
                {
                    var entry = settings.CreateOrMoveEntry(guid, group, false);
                    entry.address = folder.Substring(rootPath.Length + 1, folder.Length - rootPath.Length - 1) + "/" + Path.GetFileNameWithoutExtension(assetPath);
                }
            }
            var subfolders = AssetDatabase.GetSubFolders(folder);
            foreach (var subfolder in subfolders)
            {
                var folderName = GetFolderName(subfolder);
                if (folderName.IndexOf("_") != 0)
                {
                    SetAssetsEntris(subfolder, group);
                }
            }
        }

        private static string GetFolderName(string path)
        {
            return path.Substring(path.LastIndexOf(Path.AltDirectorySeparatorChar) + 1, path.Length - 1 - path.LastIndexOf(Path.AltDirectorySeparatorChar));
        }


        /// <summary>
        /// 检测资源之间的依赖关系
        /// </summary>
        /// <param name="groupPathes">分组的目标文件夹</param>
        private static void CheckDependence(string[] groupPathes)
        {
            if (!Settings.isShowWarnning)
            {
                return;
            }

            int line = 0;

            StringBuilder warnningSB = new StringBuilder();

            foreach (var groupFolder in groupPathes)
            {
                var groupName = groupFolder.Split(Path.AltDirectorySeparatorChar)[2];
                var mainAssetPathes = GetMainAssetsPathes(groupFolder);
                bool isNeedAppendGroupName = true;
                foreach(var assetPath in mainAssetPathes)
                {
                    var dependences = AssetDatabase.GetDependencies(assetPath);
                    HashSet<string> contentSet = new HashSet<string>();
                    foreach (var dependence in dependences)
                    {
                        var dependenceGroupName = GetAssetGroupFolder(dependence);
                        bool isNeedWarnning = !dependenceGroupName.IsNullOrEmpty() && !dependenceGroupName.Equals(groupName)
                            && !Settings.commonAssetsGroup.Contains(dependenceGroupName);
                        if (isNeedWarnning)
                        {
                            if (isNeedAppendGroupName)
                            {
                                warnningSB.Append($"{groupName} 依赖有:\n");
                                line++;
                                isNeedAppendGroupName = false;
                            }
                            var content = $"\t{GetAssetLocalGroupPath(assetPath)}-->{dependenceGroupName}\n";
                            if(!contentSet.Contains(content))
                            {
                                contentSet.Add(content);
                                warnningSB.Append(content);
                                line++;
                            }
                        }
                        if(line > 25)
                        {
                            break;
                        }
                    }
                }
                if(line > 25)
                {
                    warnningSB.Append("...");
                    break;
                }
            }

            if (!warnningSB.ToString().IsNullOrEmpty())
            {
                warnningSB.Insert(0,"Group之间存在依赖关系:\n");
                EditorUtility.DisplayDialog("警告", warnningSB.ToString(), "确定"); ;
            }
        }

        private static string GetAssetGroupFolder(string assetPath)
        {
            string groupName = string.Empty;
            var rootPath = Settings.AARootPath;
            if (assetPath.Contains(rootPath))
            {
                var rootLocalPath = assetPath.Replace(rootPath + Path.AltDirectorySeparatorChar, "");
                var strs = rootLocalPath.Split(Path.AltDirectorySeparatorChar);
                if(strs.Length >= 2)
                {
                    groupName = strs[0];
                }
            }
            return groupName;
        }

        private static string GetAssetLocalGroupPath(string assetPath)
        {
            string assetLocalGroupPath = string.Empty;
            var rootPath = Settings.AARootPath;
            if (assetPath.Contains(rootPath))
            {
                assetLocalGroupPath = assetPath.Replace(rootPath + Path.AltDirectorySeparatorChar, "");
            }
            return assetLocalGroupPath;
        }

        private static List<string> GetMainAssetsPathes(string folder)
        {
            List<string> assetsPathes = new List<string>();
            if (Directory.Exists(folder))
            {
                DirectoryInfo direction = new DirectoryInfo(folder);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta") || files[i].Name.Equals(".DS_Store"))
                    {
                        continue;
                    }
                    var assetPath = "Assets" + files[i].FullName.Replace(Application.dataPath, "");
                    assetsPathes.Add(assetPath);
                }
            }
            return assetsPathes;
        }

        public static void CreateAARoots()
        {
            var uiRootPath = Settings.AARootPath;
            if (!Directory.Exists(uiRootPath))
            {
                Directory.CreateDirectory(uiRootPath);
            }
        }

        [OnSettingsInit(prioty = 100)]
        public static void CreateAssetsDir()
        {
            CreateAARoots();
        }

    }
}
