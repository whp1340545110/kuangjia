using System;
using UnityEngine;

namespace PFramework.Cfg
{
    /// <summary>
    /// excel 当行
    /// </summary>
    [Serializable]
    public abstract class EERowData
    {
        public object GetKeyProtertyValue()
        {
            var keyProperty = EEUtility.GetRowDataKeyProperty(GetType());
            return keyProperty == null ? null : keyProperty.GetValue(this);
        }

        public virtual void OnEntryLoad() { }
    }

    /// <summary>
    /// 游戏配置基础对象
    /// </summary>
    public class CfgResObj : ScriptableObject
    {
        //加载后对数据进行处理
        public virtual void OnCfgLoad() { }
    }


    /// <summary>
    /// excel 行数据集
    /// </summary>
    [Serializable]
    public abstract class EERowDataCollection : CfgResObj
    {
        public string ExcelFileName;
        public string ExcelSheetName;
        public string KeyFieldName;
        public abstract void AddEntry(EERowData data);
        public abstract int GetEntryCount();

    }

    public abstract class CfgBase : EERowDataCollection
    {
        public abstract void SetLanguage(SystemLanguage language);
    }

    public abstract class LocalizationCfgBase: EERowDataCollection
    {

    }

    /// <summary>
    /// excel 字段特性
    /// </summary>
    public class EEKeyPropertyAttribute : Attribute
    {

    }

    /// <summary>
    /// excel 评论字段特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EECommentAttribute : Attribute
    {
        public readonly string content;

        public EECommentAttribute(string text)
        {
            content = text;
        }
    }

}
