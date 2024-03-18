using UnityEngine;

public class launch : MonoBehaviour
{
    private void Awake()
    {
#if DEV
            DontDestroyOnLoad(gameObject);
#else
            gameObject.SetActive(false);
#endif

#if !LOG_ENABLE
         Debug.unityLogger.logEnabled = true;
#endif
    }
}
