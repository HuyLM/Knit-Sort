using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base.Helper
{
    public class CoroutineHelper : MonoBehaviour
    {

        static CoroutineHelper _singleton;
        static Dictionary<string, IEnumerator> _routines = new Dictionary<string, IEnumerator>(100);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitializeType()
        {
            _singleton = new GameObject($"#{nameof(CoroutineHelper)}").AddComponent<CoroutineHelper>();
            DontDestroyOnLoad(_singleton);
        }

        public static Coroutine Start(IEnumerator routine)
        {
           return _singleton.StartCoroutine(routine);
        }
        public static Coroutine Start(IEnumerator routine, string id)
        {
            var coroutine = _singleton.StartCoroutine(routine);
            if (!_routines.ContainsKey(id))
            {
                _routines.Add(id, routine);
            }
            else
            {
                _singleton.StopCoroutine(_routines[id]);
                _routines[id] = routine;
            }
            return coroutine;
        }
        public static void Stop(IEnumerator routine)
        {
            _singleton.StopCoroutine(routine);
        }
        public static void Stop(string id)
        {
            if (_routines.TryGetValue(id, out var routine))
            {
                _singleton.StopCoroutine(routine);
                _routines.Remove(id);
            }
            else
            {
                Debug.LogWarning($"coroutine '{id}' not found"); 
            }
        }
        public static void StopAll() 
        { 
            _singleton.StopAllCoroutines(); 
        }

    }
}
