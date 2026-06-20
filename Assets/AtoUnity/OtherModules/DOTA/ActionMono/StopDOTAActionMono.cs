#if ATOGAME_ENABLE
using AtoGame.Base;
#endif
using System;
using UnityEngine;
namespace AtoGame.OtherModules.DOTA
{
#if ATOGAME_ENABLE
    public class StopDOTAActionMono : ActionMono
    {
        [SerializeField] private DoTweenAnimation anim;
        [SerializeField] private bool completed = false;

        private Action onCompleted;

        public override void Execute(Action onCompleted = null)
        {
            this.onCompleted = onCompleted;
            anim?.Stop(completed);
            OnComplete(this.onCompleted);
        }

        public override void ValidateObject()
        {
            base.ValidateObject();
            if(anim == null)
            {
                Debug.Log($"{name} ValidateObject: anim is null", this);
            }
        }
    }
#else
    public class StopDOTAActionMono : MonoBehaviour
    {
        [SerializeField] private DoTweenAnimation anim;
        [SerializeField] private bool completed = false;


    }

#endif
}