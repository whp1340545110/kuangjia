namespace PFramework.SdkBridge
{
    using UnityEngine;
    public class DefaultEventBridge : IEventBridge
    {
        public bool IsInitialized => true;

        public void LogEvent(string key, object obj, EventPlatform eventPlatform = EventPlatform.All)
        {
            string param = "";
            if (obj != null)
            {
                param = JsonUtility.ToJson(obj);
            }
            Debug.Log("log event: " + key + " param: " + param);
        }
    }
}
