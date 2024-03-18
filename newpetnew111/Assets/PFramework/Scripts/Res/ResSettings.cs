namespace PFramework.Res
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    public class ResSettings :  SettingsBase
    {
        public override string SettingsName => "资源管理器设置";

        public override int PriotyInWindow => 3;

        public string aaRootAssetPath = "AddressablesAssets";
        public string AARootPath => Path.Combine("Assets", aaRootAssetPath);
        public string netResCachePath = "NetResources";
        public bool isShowWarnning = true;
        public List<string> commonAssetsGroup = new List<string>() { "Common" };
    }
}
