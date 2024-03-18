using PFramework.Cfg;
using UniRx;
using UnityEngine;

namespace PFramework
{
    public static partial class Peach
    {
        public static CfgManager CfgMgr => CfgManager.Instance;

        public static ReactiveProperty<SystemLanguage> Language => CfgMgr.Language;

        public static T GetCfg<T>() where T : CfgBase
        {
            return CfgMgr.GetCfg<T>();
        }

        public static void ClearAll()
        {
            CfgMgr.ClearAll();
        }
    }
}
