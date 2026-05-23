using UnityEngine;

namespace ThanhDV.Utilities
{
    /// <summary>
    /// Singleton for pure C# classes (Non-MonoBehaviour).
    /// </summary>
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                        Debug.Log($"<color=yellow>[Singleton] {_instance.GetType().Name} created!</color>");
                    }
                    return _instance;
                }
            }
        }

        public static bool Exists => _instance != null;
    }

    /// <summary>
    /// Singleton for MonoBehaviour. Safely handles initialization and destruction.
    /// </summary>
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
        }

        public static T Instance
        {
            get
            {
                if (_instance == null && !ReferenceEquals(_instance, null))
                {
                    Debug.Log($"<color=yellow>[Singleton] Instance '{typeof(T)}' already destroyed. Won't create again - returning null.</color>");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindFirstObjectByType<T>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name);
                            _instance = singletonObject.AddComponent<T>();

                            Debug.Log($"<color=yellow>[Singleton] {typeof(T).Name} instance created!</color>");
                        }
                    }

                    return _instance;
                }
            }
        }

        public static bool Exists => _instance != null;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.Log($"<color=yellow>[Singleton] Another instance of {typeof(T)} detected! Destroying new one.</color>");
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// A Singleton that persists across Scenes (DontDestroyOnLoad).
    /// </summary>
    public class PersistentMonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
        }

        public static T Instance
        {
            get
            {
                if (_instance == null && !ReferenceEquals(_instance, null))
                {
                    Debug.Log($"<color=yellow>[Singleton] Instance '{typeof(T)}' already destroyed. Won't create again - returning null.</color>");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindFirstObjectByType<T>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name);
                            _instance = singletonObject.AddComponent<T>();

                            Debug.Log($"<color=yellow>[Singleton] {typeof(T).Name} instance created!</color>");
                        }
                    }

                    return _instance;
                }
            }
        }

        public static bool Exists => _instance != null;

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Debug.Log($"<color=yellow>[Singleton] Another instance of {typeof(T)} detected! Destroying new one.</color>");
                Destroy(gameObject);
            }
        }
    }
}