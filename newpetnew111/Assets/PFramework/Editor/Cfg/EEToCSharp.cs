using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PFramework;
using UnityEditor;
using UnityEngine;

namespace PeachEditor.Cfg
{
    /// <summary>
    ///     Excel Converter
    /// </summary>
    public static partial class EEConverter
    {
        public static void GenerateCSharpFiles(string excelPath, string csPath, string inspectDir, string cfgAssetDir)
        {
            try
            {
                excelPath = excelPath.Replace("\\", "/");
               
                csPath = csPath.Replace("\\", "/");
               
                var tmpPath = Environment.CurrentDirectory + "/EasyExcelTmp/";
                var tmpEditorPath = Environment.CurrentDirectory + "/EasyExcelTmp/Editor/";
              
                if (Directory.Exists(tmpPath))
                    Directory.Delete(tmpPath, true);

                Directory.CreateDirectory(tmpPath);
                Directory.CreateDirectory(tmpEditorPath);

                excelPath = excelPath.Replace("\\", "/");
                csPath = csPath.Replace("\\", "/");
                if (!csPath.EndsWith("/"))
                    csPath += "/";

                var csChanged = false;
                var filePaths = Directory.GetFiles(excelPath);
              
                for (var i = 0; i < filePaths.Length; ++i)
                {
                    Debug.Log(filePaths[i]);
                    var excelFilePath = filePaths[i].Replace("\\", "/");
                    if (i + 1 < filePaths.Length)
                        UpdateProgressBar(i + 1, filePaths.Length, "");
                    else
                        ClearProgressBar();
                    if (!IsExcelFile(excelFilePath))
                        continue;
                    var newCsDict = ToCSharpArray(excelFilePath);
                    foreach (var newCs in newCsDict)
                    {
                        var cSharpFileName = GetCSharpFileName(newCs.Key);
                        var tmpCsFilePath = tmpPath + cSharpFileName;
                        var csFilePath = csPath + cSharpFileName;
                        var shouldWrite = true;
                        if (File.Exists(csFilePath))
                        {
                            var oldCs = File.ReadAllText(csFilePath);
                            shouldWrite = oldCs != newCs.Value;
                        }

                        if (!shouldWrite)
                            continue;
                        csChanged = true;
                        File.WriteAllText(tmpCsFilePath, newCs.Value, Encoding.UTF8);
                    }
                    //inspect
                    var newInspectorDict = ToCSharpInspectorArray(excelFilePath);
                    foreach (var newCs in newInspectorDict)
                    {
                        var inspectorFileName = GetSheetInspectorFileName(newCs.Key);
                        var tmpInspFilePath = tmpEditorPath + inspectorFileName;
                        var csFilePath = inspectDir + inspectorFileName;
                        var shouldWrite = true;
                        if (File.Exists(csFilePath))
                        {
                            var oldCs = File.ReadAllText(csFilePath);
                            shouldWrite = oldCs != newCs.Value;
                        }

                        if (!shouldWrite)
                            continue;
                        csChanged = true;
                        File.WriteAllText(tmpInspFilePath, newCs.Value, Encoding.UTF8);
                    }
                }

                if (csChanged)
                {
                    EditorPrefs.SetBool(csChangedKey, true);
                    var files = Directory.GetFiles(tmpPath);
                    foreach (var s in files)
                    {
                        var p = s.Replace("\\", "/");
                        File.Copy(s, csPath + p.Substring(p.LastIndexOf("/", StringComparison.Ordinal)), true);
                    }
                    files = Directory.GetFiles(tmpEditorPath);
                    foreach (var s in files)
                    {
                        var p = s.Replace("\\", "/");
                        File.Copy(s, inspectDir + p.Substring(p.LastIndexOf("/", StringComparison.Ordinal)), true);
                    }
                    Directory.Delete(tmpPath, true);
                    AssetDatabase.Refresh();
                    PDebug.Log("Scripts are generated, wait for generating assets...");
                }
                else
                {
                    PDebug.Log("No CSharp files changed, begin generating assets...");
                    ClearProgressBar();
                    GenerateScriptableObjects(excelPath, cfgAssetDir);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                EditorPrefs.SetBool(csChangedKey, false);
                ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }

        private static Dictionary<string, string> ToCSharpArray(string excelPath)
        {
            var lst = new Dictionary<string, string>();
            var book = EEWorkbook.Load(excelPath);
            if (book == null)
                return lst;
            foreach (var sheet in book.sheets)
            {
                if (sheet == null)
                    continue;
                if (!IsValidSheet(sheet))
                {
                    PDebug.Log(string.Format("Skipped sheet {0} in file {1}.", sheet.name, Path.GetFileName(excelPath)));
                    continue;
                }
                var sheetData = ToSheetData(sheet);
                var csTxt = ToCSharp(sheetData, sheet.name);
                lst.Add(sheet.name, csTxt);
            }
            return lst;
        }

        //转换成代码
        private static string ToCSharp(SheetData sheetData, string sheetName)
        {
            try
            {
                var isLocalizationSheet = IsLocalizationSheet(sheetName);
                if (isLocalizationSheet)
                {
                    return ToLocalCfgCSharp(sheetData, sheetName);
                }
                else
                {
                    return ToCfgCSharp(sheetData, sheetName);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }

            return "";
        }


        private static string ToCfgCSharp(SheetData sheetData, string sheetName)
        {
            var rowDataClassName = GetRowDataClassName(sheetName);
            var sheetClassName = GetSheetClassName(sheetName);
            var localDataClassName = GetRowDataClassName(sheetName + "LocalBase");
            var localSheetClassName = sheetClassName + "LocalBase";
            var baseRowClassName = "EERowData";
            var baseSheetClassName = "CfgBase";

            var csFile = new StringBuilder(2048);
            csFile.Append("//------------------------------------------\n");
            csFile.Append("//auto-generated\n");
            csFile.Append("//-------------------------------------------");
            csFile.Append("\n#pragma warning disable 0649");
            csFile.Append("\nusing System;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing PFramework;\nusing PFramework.Cfg;\n\n");

            csFile.Append(string.Format("namespace {0}\n", S_NameSpace));
            csFile.Append("{\n");
            csFile.Append("\t[Serializable]\n");
            csFile.Append($"\tpublic partial class {rowDataClassName} : {baseRowClassName}\n");
            csFile.Append("\t{\n");

            var columnCount = 0;
            for (var col = 1; col < sheetData.ColumnCount; col++)
            {
                if (string.IsNullOrEmpty(sheetData.Get(S_NameRowIndex, col)))
                    break;
                columnCount++;
            }

            // Get field names
            var propertiesName = new string[columnCount];
            var fieldsName = new string[columnCount];
            for (var col = 0; col < columnCount; col++)
            {
                propertiesName[col] = sheetData.Get(S_NameRowIndex, col + 1);
            }

            // Get field types and Key column
            var propertiesLength = new string[columnCount];
            var propertiesExplain = new string[columnCount];
            var propertiesType = new string[columnCount];
            var propertiesIsLocal = new bool[columnCount];
            //var propertiesName = new string[]
            string keyPropertyNameFull = "";
            string keyPropertyName = "";
            string keyPropertyType = "";

            for (var col = 0; col < columnCount; col++)
            {
                var cellInfo = sheetData.Get(S_TypeRowIndex, col + 1);
                var explainInfo = sheetData.Get(S_ExplainIndex, col + 1);
                propertiesLength[col] = null;
                propertiesType[col] = cellInfo;
                propertiesExplain[col] = explainInfo;
                if (cellInfo.EndsWith("]"))
                {
                    var startIndex = cellInfo.IndexOf('[');
                    propertiesLength[col] = cellInfo.Substring(startIndex + 1, cellInfo.Length - startIndex - 2);
                    propertiesType[col] = cellInfo.Substring(0, startIndex);
                }

                var varName = propertiesName[col];
                var varLen = propertiesLength[col];
                var varType = propertiesType[col];
                if (varName.EndsWith(":Key") || varName.EndsWith(":key") || varName.EndsWith(":KEY"))
                {
                    var splits = varName.Split(':');
                    if ((varType.Equals("int") || varType.Equals("string")) && varLen == null)
                    {
                        keyPropertyNameFull = varName;
                        propertiesName[col] = splits[0];
                        keyPropertyName = propertiesName[col];
                        keyPropertyType = varType;
                        fieldsName[col] = "_" + splits[0].Substring(0, 1).ToLower() + splits[0].Substring(1);
                    }
                }
                else if ((varName.EndsWith("Local") || varName.EndsWith("local")))
                {
                    var splits = varName.Split(':');
                    propertiesName[col] = splits[0];
                    propertiesIsLocal[col] = true;
                    fieldsName[col] = "_" + splits[0].Substring(0, 1).ToLower() + splits[0].Substring(1);
                }
                else
                {
                    fieldsName[col] = "_" + varName.Substring(0, 1).ToLower() + varName.Substring(1);

                }
            }

            if (string.IsNullOrEmpty(keyPropertyNameFull))
            {
                Debug.LogWarning("Cannot find Key column in sheet " + sheetName);
            }

            for (var col = 0; col < columnCount; col++)
            {
                var propertyName = propertiesName[col];
                var fieldName = fieldsName[col];
                var propertyLen = propertiesLength[col];
                var propertyType = propertiesType[col];
                var propertyExplain = propertiesExplain[col];
                bool isKeyProperty = !string.IsNullOrEmpty(keyPropertyName) && keyPropertyName == propertyName;
                if (IsSupportedColumnType(propertyType))
                {
                    if (!string.IsNullOrEmpty(propertyExplain))
                    {
                        csFile.AppendFormat("\t\t/// <summary>\n\t\t///{0}\n\t\t/// </summary>\n", propertyExplain);
                    }
                    csFile.Append("\t\t[SerializeField]\n");
                    if (propertyLen == null)
                    {
                        csFile.AppendFormat("\t\tprivate {0} {1};\n", propertyType, fieldName);
                        if (isKeyProperty)
                        {
                            csFile.Append("\t\t[EEKeyProperty]\n");
                        }
                        csFile.AppendFormat("\t\tpublic  {0} {1} {{ get {{ return {2}; }} }}\n\n", propertyType, propertyName, fieldName);
                    }
                    else
                    {
                        csFile.AppendFormat("\t\tprivate {0}[] {1};\n", propertyType, fieldName);
                        if (isKeyProperty)
                        {
                            csFile.Append("\t\t[EEKeyProperty]\n");
                        }
                        csFile.AppendFormat("\t\tpublic  {0}[] {1} {{ get {{ return {2}; }} }}\n\n", propertyType, propertyName, fieldName);
                    }
                }
            }

            csFile.AppendFormat("\n\t\tpublic {0}()" + "\n", rowDataClassName);
            csFile.Append("\t\t{" + "\n");
            csFile.Append("\t\t}\n");

            csFile.Append("\n#if UNITY_EDITOR\n");
            csFile.AppendFormat("\t\tpublic {0}(List<List<string>> sheet, int row, int column)" + "\n", rowDataClassName);
            csFile.Append("\t\t{" + "\n");

            for (var col = 0; col < columnCount; col++)
            {
                var varType = propertiesType[col];
                var varLen = propertiesLength[col];
                var varName = fieldsName[col];


                if (varType.Equals("int") || varType.Equals("float") || varType.Equals("double") ||
                    varType.Equals("long") || varType.Equals("bool"))
                {
                    if (varLen == null)
                    {
                        csFile.Append("\t\t\t" + varType + ".TryParse(sheet[row][column++], out " + varName + ");\n");
                    }
                    else
                    {
                        csFile.Append("\t\t\tstring " + varName + "ArrayString=sheet[row][column++];" + "\n");
                        csFile.Append("\t\t\tif(!string.IsNullOrEmpty(" + varName + "ArrayString))\n");
                        csFile.Append("\t\t\t{\n");
                        csFile.Append("\t\t\t\tstring[] " + varName + "Array = " + varName + "ArrayString.Split(\',\');" + "\n");
                        csFile.Append("\t\t\t\tint " + varName + "Count = " + varName + "Array.Length;" + "\n");
                        csFile.Append("\t\t\t\t" + varName + " = new " + varType + "[" + varName + "Count];\n");
                        csFile.Append("\t\t\t\tfor(int i = 0; i < " + varName + "Count; i++)\n");
                        csFile.Append("\t\t\t\t{\n");
                        csFile.Append("\t\t\t\t\t" + varType + ".TryParse(" + varName + "Array[i], out " + varName + "[i]);\n");
                        csFile.Append("\t\t\t\t}\n");
                        csFile.Append("\t\t\t}\n");
                    }
                }
                else if (varType.Equals("string"))
                {
                    if (varLen == null)
                    {
                        csFile.Append("\t\t\t" + varName + " = sheet[row][column++] ?? \"" + /*varDefault + */"\";\n");
                    }
                    else
                    {
                        csFile.Append("\t\t\tstring " + varName + "ArrayString=sheet[row][column++];" + "\n");
                        csFile.Append("\t\t\tif(!string.IsNullOrEmpty(" + varName + "ArrayString))\n");
                        csFile.Append("\t\t\t{\n");
                        csFile.Append("\t\t\t\tstring[] " + varName + "Array = " + varName + "ArrayString.Split(\',\');" + "\n");
                        csFile.Append("\t\t\t\tint " + varName + "Count = " + varName + "Array.Length;" + "\n");
                        csFile.Append("\t\t\t\t" + varName + " = new " + varType + "[" + varName + "Count];\n");
                        csFile.Append("\t\t\t\tfor(int i = 0; i < " + varName + "Count; i++)\n");
                        csFile.Append("\t\t\t\t{\n");
                        csFile.Append("\t\t\t\t\t" + varName + "[i] = " + varName + "Array[i];\n");
                        csFile.Append("\t\t\t\t}\n");
                        csFile.Append("\t\t\t}\n");
                    }
                }
            }

            csFile.Append("\t\t}\n#endif\n");

            csFile.Append("\t\tpublic void ChangeLocalProperties(" + localDataClassName + " localEntry)\n");
            csFile.Append("\t\t{\n");
            for (var col = 0; col < columnCount; col++)
            {
                var propertyName = propertiesName[col];
                var fieldName = fieldsName[col];
                var propertyType = propertiesType[col];
                bool isLocalProperty = propertiesIsLocal[col];
                bool isKeyProperty = !string.IsNullOrEmpty(keyPropertyName) && keyPropertyName == propertyName;
                if (IsSupportedColumnType(propertyType) && !isKeyProperty)
                {
                    if (isLocalProperty)
                    {
                        csFile.Append($"\t\t\t{fieldName} = localEntry.{propertyName};\n");
                    }
                }
            }
            csFile.Append("\t\t}\n\n");

            csFile.Append("\t\tpartial void OnEntryLoadCustomized();\n\n");
            csFile.Append("\t\tpublic override void OnEntryLoad()\n");
            csFile.Append("\t\t{\n");
            csFile.Append("\t\t\tOnEntryLoadCustomized();\n");
            csFile.Append("\t\t}\n");
            csFile.Append("\t}\n\n");
            //LocalClass

            csFile.Append("\t[Serializable]\n");
            csFile.Append($"\tpublic abstract class {localDataClassName} : EERowData\n");
            csFile.Append("\t{\n");
            for (var col = 0; col < columnCount; col++)
            {
                var propertyName = propertiesName[col];
                var fieldName = fieldsName[col];
                var propertyLen = propertiesLength[col];
                var propertyType = propertiesType[col];
                var propertyExplain = propertiesExplain[col];
                bool isLocalProperty = propertiesIsLocal[col];
                if (IsSupportedColumnType(propertyType) && isLocalProperty)
                {
                    if (!string.IsNullOrEmpty(propertyExplain))
                    {
                        csFile.AppendFormat("\t\t/// <summary>\n\t\t///{0}\n\t\t/// </summary>\n", propertyExplain);
                    }
                    if (propertyLen == null)
                    {
                        csFile.AppendFormat("\t\tpublic abstract {0} {1} {{ get; }}\n\n", propertyType, propertyName);
                    }
                    else
                    {
                        csFile.AppendFormat("\t\tpublic abstract {0}[] {1} {{ get; }}\n\n", propertyType, propertyName);
                    }
                }
            }
            csFile.Append("\t}\n\n");

            //
            // EERowDataCollection class
            csFile.Append("\t[PreferBinarySerialization]\n");
            csFile.Append("\t[Serializable]\n");
            csFile.Append($"\tpublic partial class {sheetClassName} : {baseSheetClassName}\n");
            csFile.Append("\t{\n");
            csFile.AppendFormat("\t\t[SerializeField]\n\t\tprivate List<{0}> _entryList = new List<{0}>();\n\n", rowDataClassName);
            csFile.AppendFormat("\t\tpublic IReadOnlyList<{0}> EntryList => _entryList;\n\n", rowDataClassName);

            bool isKeyExist = !string.IsNullOrEmpty(keyPropertyType) && (!string.IsNullOrEmpty(keyPropertyName));

            if (isKeyExist)
            {
                csFile.AppendFormat("\t\tprivate Dictionary<{0}, {1}> _entryDic;\n\n", keyPropertyType, rowDataClassName);
            }

            csFile.AppendFormat("\t\tpublic override void AddEntry(EERowData data)\n\t\t{{\n\t\t\t_entryList.Add(data as {0});\n\t\t}}\n\n", rowDataClassName);
            csFile.Append("\t\tpublic override int GetEntryCount()\n\t\t{\n\t\t\treturn _entryList.Count;\n\t\t}\n\n");

            csFile.AppendFormat("\t\tpublic {0} GetEntryByIndex(int index)\n", rowDataClassName);
            csFile.Append("\t\t{\n");
            csFile.AppendFormat("\t\t\treturn _entryList[index] as {0};\n\n", rowDataClassName);

            csFile.Append("\t\t}\n\n");

            //生成key获取数据函数
            if (isKeyExist)
            {
                csFile.AppendFormat("\t\tpublic {0} GetEntryByKey({1} kId)\n", rowDataClassName, keyPropertyType);
                csFile.Append("\t\t{\n");
                csFile.AppendFormat("\t\t\t{0} result;\n", rowDataClassName);
                csFile.Append("\t\t\tif (_entryDic.TryGetValue(kId, out result)){\n");
                csFile.Append("\t\t\t\treturn result;\n\t\t\t}\n");
                csFile.Append("\t\t\t return null;\n");
                csFile.Append("\t\t}\n\n");
            }

            csFile.Append("\t\tpartial void OnCfgLoadCustomized();\n\n");

            csFile.Append("\t\tpublic override void OnCfgLoad()\n");
            csFile.Append("\t\t{\n");

            csFile.Append("\t\t\tif (_entryList == null || _entryList.Count == 0)\n");
            csFile.Append("\t\t\t{\n\t\t\t\treturn;\n\t\t\t}\n");

            if (isKeyExist)
            {
                csFile.AppendFormat("\t\t\t_entryDic = new Dictionary<{0}, {1}>(_entryList.Count);\n", keyPropertyType, rowDataClassName);
            }

            csFile.Append("\t\t\tfor (int i = 0; i < _entryList.Count; i++)\n");
            csFile.Append("\t\t\t{\n");
            if (isKeyExist)
            {
                csFile.AppendFormat("\t\t\t\t_entryDic.Add(_entryList[i].{0}, _entryList[i]);\n", keyPropertyName);
            }
            csFile.AppendFormat("\t\t\t\t_entryList[i].OnEntryLoad();\n", keyPropertyName);
            csFile.Append("\t\t\t}\n");

            csFile.Append("\t\t\tOnCfgLoadCustomized();\n");
            csFile.Append("\t\t}\n\n");


            csFile.Append("\t\tpublic override void SetLanguage(SystemLanguage language)\n");
            csFile.Append("\t\t{\n");
            csFile.Append("\t\t\tvar cfgName = GetType().Name+ \"_\" + language;\n");
            csFile.Append($"\t\t\t{localSheetClassName} so = null;\n");
            csFile.Append("\t\t\tvar localCfgObject = Peach.LoadSync<LocalizationCfgBase>(CfgSettings.GetSettings<CfgSettings>().cfgAssetsLoadPath + \"/\" + cfgName);\n");
            csFile.Append("\t\t\tif (localCfgObject != null)\n");
            csFile.Append("\t\t\t{\n");
            csFile.Append($"\t\t\t\tso = localCfgObject as {localSheetClassName};\n");
            csFile.Append("\t\t\t\tso.OnCfgLoad();\n");
            csFile.Append("\t\t\t\tfor (int i = 0; i < _entryList.Count; i++)\n");
            csFile.Append("\t\t\t\t{\n");
            csFile.Append("\t\t\t\t\t _entryList[i].ChangeLocalProperties(so.GetEntryByIndex(i));\n");
            csFile.Append("\t\t\t\t}\n");
            csFile.Append("\t\t\t}\n");
            csFile.Append("\t\t\telse\n");
            csFile.Append("\t\t\t{\n");
            csFile.Append("\t\t\t\t PDebug.LogWarning($\"Load LocalCfg Asset Error {cfgName}\");\n");
            csFile.Append("\t\t\t}\n");
            csFile.Append("\t\t}\n");

            csFile.Append("\t}\n");

            //localizationBaseCfg

            csFile.Append("\n\t[PreferBinarySerialization]\n");
            csFile.Append("\t[Serializable]\n");
            csFile.Append($"\tpublic abstract class {localSheetClassName} : LocalizationCfgBase\n");
            csFile.Append("\t{\n");
            csFile.Append("\t\tpublic abstract " + localDataClassName + " GetEntryByIndex(int index);\n");
            csFile.Append("\t}\n");


            csFile.Append("}\n");
            csFile.Append("#pragma warning restore 0649");
            return csFile.ToString();
        }

        private static string ToLocalCfgCSharp(SheetData sheetData, string sheetName)
        {
            var rowDataClassName = GetRowDataClassName(sheetName);
            var sheetClassName = GetSheetClassName(sheetName);

            var baseRowClassName = sheetName.Split('_')[0] + "LocalBaseEntry";
            var baseSheetClassName = sheetName.Split('_')[0] + "LocalBase";

            var csFile = new StringBuilder(2048);
            csFile.Append("//------------------------------------------\n");
            csFile.Append("//auto-generated\n");
            csFile.Append("//-------------------------------------------");
            csFile.Append("\n#pragma warning disable 0649");
            csFile.Append("\nusing System;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing PFramework;\nusing PFramework.Cfg;\n\n");

            csFile.Append(string.Format("namespace {0}\n", S_NameSpace));
            csFile.Append("{\n");
            csFile.Append("\t[Serializable]\n");
            csFile.Append($"\tpublic class {rowDataClassName} : {baseRowClassName}\n");
            csFile.Append("\t{\n");

            var columnCount = 0;
            for (var col = 1; col < sheetData.ColumnCount; col++)
            {
                if (string.IsNullOrEmpty(sheetData.Get(S_NameRowIndex, col)))
                    break;
                columnCount++;
            }

            // Get field names
            var propertiesName = new string[columnCount];
            var fieldsName = new string[columnCount];
            for (var col = 0; col < columnCount; col++)
            {
                propertiesName[col] = sheetData.Get(S_NameRowIndex, col + 1);
            }

            // Get field types and Key column
            var propertiesLength = new string[columnCount];
            var propertiesExplain = new string[columnCount];
            var propertiesType = new string[columnCount];
            string keyPropertyNameFull = "";
            string keyPropertyName = "";
            string keyPropertyType = "";

            for (var col = 0; col < columnCount; col++)
            {
                var cellInfo = sheetData.Get(S_TypeRowIndex, col + 1);
                var explainInfo = sheetData.Get(S_ExplainIndex, col + 1);
                propertiesLength[col] = null;
                propertiesType[col] = cellInfo;
                propertiesExplain[col] = explainInfo;
                if (cellInfo.EndsWith("]"))
                {
                    var startIndex = cellInfo.IndexOf('[');
                    propertiesLength[col] = cellInfo.Substring(startIndex + 1, cellInfo.Length - startIndex - 2);
                    propertiesType[col] = cellInfo.Substring(0, startIndex);
                }

                var varName = propertiesName[col];
                var varLen = propertiesLength[col];
                var varType = propertiesType[col];
                if (varName.EndsWith(":Key") || varName.EndsWith(":key") || varName.EndsWith(":KEY"))
                {
                    var splits = varName.Split(':');
                    if ((varType.Equals("int") || varType.Equals("string")) && varLen == null)
                    {
                        keyPropertyNameFull = varName;
                        propertiesName[col] = splits[0];
                        keyPropertyName = propertiesName[col];
                        keyPropertyType = varType;
                        fieldsName[col] = "_" + splits[0].Substring(0, 1).ToLower() + splits[0].Substring(1);
                    }
                }
                else
                {
                    fieldsName[col] = "_" + varName.Substring(0, 1).ToLower() + varName.Substring(1);

                }
            }

            if (string.IsNullOrEmpty(keyPropertyNameFull))
            {
                Debug.LogWarning("Cannot find Key column in sheet " + sheetName);
            }

            for (var col = 0; col < columnCount; col++)
            {
                var propertyName = propertiesName[col];
                var fieldName = fieldsName[col];
                var propertyLen = propertiesLength[col];
                var propertyType = propertiesType[col];
                var propertyExplain = propertiesExplain[col];
                bool isKeyProperty = !string.IsNullOrEmpty(keyPropertyName) && keyPropertyName == propertyName;
                if (IsSupportedColumnType(propertyType))
                {
                    if (!string.IsNullOrEmpty(propertyExplain))
                    {
                        csFile.AppendFormat("\t\t/// <summary>\n\t\t///{0}\n\t\t/// </summary>\n", propertyExplain);
                    }
                    csFile.Append("\t\t[SerializeField]\n");
                    if (propertyLen == null)
                    {
                        csFile.AppendFormat("\t\tprivate {0} {1};\n", propertyType, fieldName);
                        if (isKeyProperty)
                        {
                            csFile.Append("\t\t[EEKeyProperty]\n");
                            csFile.AppendFormat("\t\tpublic {0} {1} {{ get {{ return {2}; }} }}\n\n", propertyType, propertyName, fieldName);
                        }
                        else
                        {
                            csFile.AppendFormat("\t\tpublic {0} {1} {2} {{ get {{ return {3}; }} }}\n\n", "override", propertyType, propertyName, fieldName);
                        }
                    }
                    else
                    {
                        csFile.AppendFormat("\t\tprivate {0}[] {1};\n", propertyType, fieldName);
                        if (isKeyProperty)
                        {
                            csFile.Append("\t\t[EEKeyProperty]\n");
                            csFile.AppendFormat("\t\tpublic {0}[] {1} {{ get {{ return {2}; }} }}\n\n", propertyType, propertyName, fieldName);
                        }
                        else
                        {
                            csFile.AppendFormat("\t\tpublic {0} {1}[] {2} {{ get {{ return {3}; }} }}\n\n", "override", propertyType, propertyName, fieldName);
                        }
                    }
                }
            }

            csFile.AppendFormat("\n\t\tpublic {0}()" + "\n", rowDataClassName);
            csFile.Append("\t\t{" + "\n");
            csFile.Append("\t\t}\n");

            csFile.Append("\n#if UNITY_EDITOR\n");
            csFile.AppendFormat("\t\tpublic {0}(List<List<string>> sheet, int row, int column)" + "\n", rowDataClassName);
            csFile.Append("\t\t{" + "\n");

            for (var col = 0; col < columnCount; col++)
            {
                var varType = propertiesType[col];
                var varLen = propertiesLength[col];
                var varName = fieldsName[col];

                if (varType.Equals("int") || varType.Equals("float") || varType.Equals("double") ||
                    varType.Equals("long") || varType.Equals("bool"))
                {
                    if (varLen == null)
                    {
                        csFile.Append("\t\t\t" + varType + ".TryParse(sheet[row][column++], out " + varName + ");\n");
                    }
                    else
                    {
                        csFile.Append("\t\t\tstring " + varName + "ArrayString=sheet[row][column++];" + "\n");
                        csFile.Append("\t\t\tif(!string.IsNullOrEmpty(" + varName + "ArrayString))\n");
                        csFile.Append("\t\t\t{\n");
                        csFile.Append("\t\t\t\tstring[] " + varName + "Array = " + varName + "ArrayString.Split(\',\');" + "\n");
                        csFile.Append("\t\t\tint " + varName + "Count = " + varName + "Array.Length;" + "\n");
                        csFile.Append("\t\t\t" + varName + " = new " + varType + "[" + varName + "Count];\n");
                        csFile.Append("\t\t\tfor(int i = 0; i < " + varName + "Count; i++)\n");
                        csFile.Append("\t\t\t\t" + varType + ".TryParse(" + varName + "Array[i], out " + varName + "[i]);\n");
                        csFile.Append("\t\t\t}\n");
                    }
                }
                else if (varType.Equals("string"))
                {
                    if (varLen == null)
                    {
                        csFile.Append("\t\t\t" + varName + " = sheet[row][column++] ?? \"" + /*varDefault + */"\";\n");
                    }
                    else
                    {
                        csFile.Append("\t\t\tstring " + varName + "ArrayString=sheet[row][column++];" + "\n");
                        csFile.Append("\t\t\tif(!string.IsNullOrEmpty(" + varName + "ArrayString))\n");
                        csFile.Append("\t\t\t{\n");
                        csFile.Append("\t\t\t\tstring[] " + varName + "Array = " + varName + "ArrayString.Split(\',\');" + "\n");
                        csFile.Append("\t\t\t\tint " + varName + "Count = " + varName + "Array.Length;" + "\n");
                        csFile.Append("\t\t\t\t" + varName + " = new " + varType + "[" + varName + "Count];\n");
                        csFile.Append("\t\t\t\tfor(int i = 0; i < " + varName + "Count; i++)\n");
                        csFile.Append("\t\t\t\t\t" + varName + "[i] = " + varName + "Array[i];\n");
                        csFile.Append("\t\t\t}\n");
                    }
                }
            }


            csFile.Append("\t\t}\n#endif\n");



            csFile.Append("\t}\n\n");
            //LocalClass

            //
            // EERowDataCollection class
            csFile.Append("\t[PreferBinarySerialization]\n");
            csFile.Append("\t[Serializable]\n");
            csFile.Append($"\tpublic class {sheetClassName} : {baseSheetClassName}\n");
            csFile.Append("\t{\n");
            csFile.AppendFormat("\t\t[SerializeField]\n\t\tprivate List<{0}> _entryList = new List<{0}>();\n\n", rowDataClassName);
            if (!string.IsNullOrEmpty(keyPropertyType) && !string.IsNullOrEmpty(keyPropertyName))
            {
                csFile.AppendFormat("\t\tprivate Dictionary<{0}, {1}> _entryDic;\n\n", keyPropertyType, rowDataClassName);
            }

            csFile.AppendFormat("\t\tpublic override void AddEntry(EERowData data)\n\t\t{{\n\t\t\t_entryList.Add(data as {0});\n\t\t}}\n\n", rowDataClassName);
            csFile.Append("\t\tpublic override int GetEntryCount()\n\t\t{\n\t\t\treturn _entryList.Count;\n\t\t}\n\n");
            //生成根据索引获取

            csFile.AppendFormat("\t\tpublic override {0} GetEntryByIndex(int index)\n", baseRowClassName);
            csFile.Append("\t\t{\n");
            csFile.AppendFormat("\t\t\treturn _entryList[index] as {0};\n\n", baseRowClassName);

            csFile.Append("\t\t}\n\n");

            //生成key获取数据函数
            if (!string.IsNullOrEmpty(keyPropertyType) && (!string.IsNullOrEmpty(keyPropertyName)))
            {
                csFile.AppendFormat("\t\tpublic {0} GetEntryByKey({1} kId)\n", rowDataClassName, keyPropertyType);
                csFile.Append("\t\t{\n");
                csFile.AppendFormat("\t\t\t{0} result;\n", rowDataClassName);
                csFile.Append("\t\t\tif (_entryDic.TryGetValue(kId, out result)){\n");
                csFile.Append("\t\t\t\treturn result;\n\t\t\t}\n");
                csFile.Append("\t\t\t return null;\n");
                csFile.Append("\t\t}\n\n");
            }

            if (!string.IsNullOrEmpty(keyPropertyType) && (!string.IsNullOrEmpty(keyPropertyName)))
            {
                csFile.Append("\t\tpublic override void OnCfgLoad()\n");
                csFile.Append("\t\t{\n");
                csFile.Append("\t\t\tif (_entryList == null || _entryList.Count == 0)\n");
                csFile.Append("\t\t\t{\n\t\t\t\treturn;\n\t\t\t}\n");
                csFile.AppendFormat("\t\t\t_entryDic = new Dictionary<{0}, {1}>(_entryList.Count);\n", keyPropertyType, rowDataClassName);
                csFile.Append("\t\t\tfor (int i = 0; i < _entryList.Count; i++)\n");
                csFile.Append("\t\t\t{\n");
                csFile.AppendFormat("\t\t\t\t_entryDic.Add(_entryList[i].{0}, _entryList[i]);\n", keyPropertyName);
                csFile.Append("\t\t\t}\n");
                csFile.Append("\t\t}\n");
            }
            csFile.Append("\t}\n");
            csFile.Append("}\n");
            csFile.Append("\n#pragma warning restore 0649");
            return csFile.ToString();
        }

        private static Dictionary<string, string> ToCSharpInspectorArray(string excelPath)
        {
            var lst = new Dictionary<string, string>();
            var book = EEWorkbook.Load(excelPath);
            if (book == null)
                return lst;
            foreach (var sheet in book.sheets)
            {
                if (sheet == null)
                    continue;
                if (!IsValidSheet(sheet))
                    continue;
                var csTxt = ToCSharpInspector(sheet.name);
                lst.Add(sheet.name, csTxt);
            }
            return lst;
        }

        //资产检查器 样式
        private static string ToCSharpInspector(string sheetName)
        {
            try
            {
                var inspectorClassName = GetSheetInspectorClassName(sheetName);
                var csFile = new StringBuilder(1024);
                csFile.Append("//----------------------------\n");
                csFile.Append("//auto-generated\n");
                csFile.Append("//----------------------------");
                csFile.Append("\nusing UnityEditor;\nusing PeachEditor.Cfg;\n\n");
                csFile.Append(string.Format("namespace {0}\n", S_NameSpace));
                csFile.Append("{\n");
                csFile.Append(string.Format("\t[CustomEditor(typeof({0}))]\n", sheetName));
                csFile.Append("\tpublic class " + inspectorClassName + " : EEAssetInspector\n");
                csFile.Append("\t{\n");

                csFile.Append("\n");

                //
                csFile.Append("\t}\n");
                csFile.Append("}\n");

                return csFile.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }

            return "";
        }
    }
}