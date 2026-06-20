using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Base
{
    /// <summary>
    /// Base MonoBehavior with interface IPoolable
    /// </summary>
    public class Poolable : MonoBehaviour, IPoolable
    {
        public virtual void OnRecycleCallback()
        {
        }
        public virtual void OnSpawnCallback()
        {
        }
    }
}
