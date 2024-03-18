using UnityEngine;

namespace Singleton
{
    public partial class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 0)
                        {
                            DontDestroyOnLoad(_instance.gameObject);
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);
                        }
                    }

                    return _instance;
                }
            }
        }

        private static bool applicationIsQuitting = false;

        public void OnDestroy()
        {
            applicationIsQuitting = true;
            OnSingletonDestory();
        }

        protected virtual void OnSingletonDestory()
        {

        }
    }

    public partial class CSharpSingleton<T> where T : class, new()
    {
        private static T _instance;
        private static string S_name;

        public static T Instance
        {
            get
            {
                S_name = "Singleton" + typeof(T).ToString();
                lock (S_name)
                {
                    if (_instance == null)
                        _instance = new T();
                }

                return _instance;
            }
        }
    }
}