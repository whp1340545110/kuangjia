using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PFramework
{
    public class PDebug
    {
        public static void LogError(object message,params object[] format)
        {
            if (message is string @string)
            {
                if (format.Length > 0)
                {
                    Debug.LogErrorFormat(@string, format);
                }
                else
                {
                    Debug.LogError(message);
                }
            }
            else
            {
                Debug.LogError(message);
            }
        }

        public static void LogWarning(object message,params object[] format)
        {
            if (message is string @string)
            {
                if (format.Length > 0)
                {
                    Debug.LogWarningFormat(@string, format);
                }
                else
                {
                    Debug.LogWarning(message);
                }
            }
            else
            {
                Debug.LogWarning(message);
            }
        }

        public static void Log(object message,params object[] format)
        {
            if (message is string @string)
            {
                if(format.Length > 0)
                {
                    Debug.LogFormat(@string, format);
                }
                else
                {
                    Debug.Log(message);
                }
            }
            else
            {
                Debug.Log(message);
            }
        }
    }
}
