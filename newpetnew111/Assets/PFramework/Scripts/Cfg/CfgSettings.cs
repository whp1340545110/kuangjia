using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFramework.Cfg
{
    public class CfgSettings : SettingsBase
    {
        public override int PriotyInWindow => 2;
        public override string SettingsName => "游戏配置路径设置";

        //excel 转换成scriptobj资源目录
        public string cfgAssetDir = "Resources/Peach/Cfgs";

        public string cfgCodeNamespace = "Game.Cfg";

        /// <summary>
        /// 配置加载目录
        /// </summary>
        public string cfgAssetsLoadPath = "Peach/Cfgs";

        /// <summary>
        /// 字体加载目录
        /// </summary>
        public string localFontsLoadPath = "Peach/Fonts";
    }
}
