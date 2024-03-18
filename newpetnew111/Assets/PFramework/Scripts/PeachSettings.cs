using System;
using System.Collections.Generic;
using UnityEngine;

namespace PFramework
{
    public class SettingsBase : ScriptableObject, IComparable<SettingsBase>
    {
        /// <summary>
        /// 显示在window上的设置Titile
        /// </summary>
        public virtual string SettingsName => GetType().Name;

        /// <summary>
        /// settingsWindow排序,越低位置越靠前
        /// </summary>
        public virtual int PriotyInWindow => 100;

        /// <summary>
        /// 是否在SettingsWindow默认收起
        /// </summary>
        public virtual bool IsFade => true;

        public static T GetSettings<T>() where T : SettingsBase
        {
            return LoadFromResources<T>();
        }

        private static T LoadFromResources<T>() where T : SettingsBase
        {
            var name = "Peach/Settings/" + typeof(T).Name;
            return Resources.Load<T>(name);
        }

        public int CompareTo(SettingsBase other)
        {
            return PriotyInWindow - other.PriotyInWindow;
        }
    }

    public class PeachSettings : SettingsBase
    {
        public override string SettingsName => "主要设置";

        public override int PriotyInWindow => -1;

        public override bool IsFade => false;

        public string resManager = "Peach.Res.ResManager";

        public List<string> otherManagers = new List<string>();

        public bool hotFix = false;

        public List<string> customerDefineSymbols = new List<string>();
    }
}
