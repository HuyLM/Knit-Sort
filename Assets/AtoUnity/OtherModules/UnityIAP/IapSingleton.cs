using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AtoGame.AtoUnity.IAP
{
    public abstract class IapSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T instance;

        /**<summary> If you want to check null, use 'if (T.Initialized)' instead of 'if (T.Instance != null)' </summary> */
        public static bool Initialized
        {
            get { return instance != null; }
        }

        public static T Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = FindObjectOfType<T>(true);
                if (instance == null)
                {
                    Debug.LogErrorFormat("[Singleton] Class {0} must be added to scene before run!", typeof(T));
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            this.transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            if (instance == null)
                instance = this as T;
            else if (this != instance)
            {
                Debug.LogWarningFormat("[MonoSingleton] Class {0} is initialized multiple times", typeof(T).FullName);
                DestroyImmediate(gameObject);
                return;
            }

            OnAwake();
        }

        protected abstract void OnAwake();

        /**<summary> Call T.Instance.Preload() at the first script startup to pre init. </summary>*/
        public virtual void Preload() { }

        protected virtual void OnDestroy()
        {
            instance = null;
        }
    }
}