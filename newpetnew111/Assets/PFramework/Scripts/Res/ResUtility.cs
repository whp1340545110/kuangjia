using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PFramework.Res
{
    public static class ResUtility
    {
        public static string GetNetCacheRootPath()
        {
            return Path.Combine(Application.persistentDataPath, ResSettings.GetSettings<ResSettings>().netResCachePath);
        }

        public static void ClearNetResCache()
        {
            var netResPath = GetNetCacheRootPath();
            if (Directory.Exists(netResPath))
            {
                Directory.Delete(netResPath, true);
                PDebug.Log("Delete {0}", netResPath);
            }
            else
            {
                PDebug.Log("{0} not exists", netResPath);
            }
        }
    }
}
