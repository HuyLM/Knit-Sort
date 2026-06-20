using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    public abstract class FloatGetSet : MonoBehaviour {
        public abstract void Set(float value);
        public abstract float Get();
    }
}
