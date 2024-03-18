using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PFramework;
using PFramework.Cfg;
using UnityEditor;
using UnityEngine;

namespace PeachEditor.Cfg
{
    /// <summary>
    ///     Excel Converter
    /// </summary>
    public static partial class EEConverter
    {
        public static void GenerateScriptableObjects(string xlsxDir, string assetDir)
        {
            try
            {
                xlsxDir = xlsxDir.Replace("\\", "/");
                assetDir = assetDir.Replace("\\", "/");
                if (!assetDir.EndsWith("/"))
                {
                    assetDir += "/";
                }
                if (Directory.Exists(assetDir))
                {
                    PFramework.FileUtil.ClearDirectory(assetDir);
                }
                //
                var filePaths = Directory.GetFiles(xlsxDir);
                var count = 0;
                for (var i = 0; i < filePaths.Length; ++i)
                {
                    var filePath = filePaths[i].Replace("\\", "/");
                    if (!IsExcelFile(filePath)) continue;
                    UpdateProgressBar(i, filePaths.Length, "");
                    ToScriptableObject(filePath, assetDir);
                    count++;
                }
                PDebug.Log("Assets are generated successfully.");
                ClearProgressBar();
                AssetDatabase.Refresh();
                PDebug.Log(string.Format("Import done. {0} sheets were imported.", count));
            }
            catch (Exception e)
            {
                PDebug.LogError(e.ToString());
                ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }

        private static void ToScriptableObject(string excelPath, string outputDir)
        {
            try
            {
                var book = EEWorkbook.Load(excelPath);
                if (book == null)
                    return;
                foreach (var sheet in book.sheets)
                {
                    if (sheet == null)
                        continue;
                    if (!IsValidSheet(sheet))
                        continue;
                    var sheetData = ToSheetDataRemoveEmptyColumn(sheet);
                    ToScriptableObject(excelPath, sheet.name, outputDir, sheetData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                AssetDatabase.Refresh();
            }
        }

        private static void ToScriptableObject(string excelPath, string sheetName, string outputDir, SheetData sheetData)
        {
            try
            {
                var sheetClassName = GetSheetClassName(sheetName);
                var asset = ScriptableObject.CreateInstance(sheetClassName);
                var dataCollect = asset as EERowDataCollection;
                if (dataCollect == null)
                    return;
                dataCollect.ExcelFileName = Path.GetFileName(excelPath);
                dataCollect.ExcelSheetName = sheetName;
                var className = GetRowDataClassName(sheetName, true);
                var dataType = Type.GetType(className);
                if (dataType == null)
                {
                    var asmb = Assembly.LoadFrom(Environment.CurrentDirectory + "/Library/ScriptAssemblies/Assembly-CSharp.dll");
                    dataType = asmb.GetType(className);
                }
                if (dataType == null)
                {
                    Debug.LogError(className + " not exist !");
                    return;
                }

                //var dataCtor = dataType.GetConstructor(Type.EmptyTypes);
                var dataCtor = dataType.GetConstructor(new[] { typeof(List<List<string>>), typeof(int), typeof(int) });
                if (dataCtor == null)
                    return;
                var keySet = new HashSet<object>();
                bool isContainsKey = EEUtility.GetRowDataKeyProperty(asset.GetType()) != null;
                for (var row = S_DataStartIndex; row < sheetData.RowCount; ++row)
                {
                    for (var col = 1; col < sheetData.ColumnCount; ++col)
                        sheetData.Set(row, col, sheetData.Get(row, col).Replace("\n", "\\n"));

                    //跳过表里可能出现的空行
                    if(sheetData.Table[row].All((_) => _.IsNullOrEmpty()))
                    {
                        continue;
                    }

                    //if(sheetData.Table.All((_) =>_.isNu))
                    //从第一列开始获取数据 
                    var inst = dataCtor.Invoke(new object[] { sheetData.Table, row, 1 }) as EERowData;
                    if (inst == null)
                        continue;
                    //inst._init(sheetData.Table, row, 0);
                    var key = inst.GetKeyProtertyValue();
                    if (isContainsKey)
                    {
                        if (key == null)
                        {
                            Debug.LogError("带key表 未设置行Key Id " + sheetName);
                            continue;
                        }
                        if (!keySet.Contains(key))
                        {
                            dataCollect.AddEntry(inst);
                            keySet.Add(key);
                        }
                        else
                        {
                            Debug.LogError(string.Format("带key表重复配置行: [{0}] in Sheet {1}", key, sheetName));
                        }
                    }
                    else
                    {
                        dataCollect.AddEntry(inst);
                    }
                    var keyField = EEUtility.GetRowDataKeyProperty(dataType);
                    if (keyField != null)
                    {
                        dataCollect.KeyFieldName = keyField.Name;
                    }
                }

                //资源 asset scriptobject
                var itemPath = outputDir + GetAssetFileName(sheetName);
                itemPath = itemPath.Substring(itemPath.IndexOf("Assets", StringComparison.Ordinal));
                AssetDatabase.CreateAsset(asset, itemPath);

                //AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

    }
}