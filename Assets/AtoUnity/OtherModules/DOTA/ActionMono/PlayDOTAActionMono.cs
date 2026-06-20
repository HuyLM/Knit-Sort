#if ATOGAME_ENABLE
using AtoGame.Base;
#endif
using System;
using UnityEngine;
namespace AtoGame.OtherModules.DOTA
{
#if ATOGAME_ENABLE
    public class PlayDOTAActionMono : ActionMono
    {
        [SerializeField] private DoTweenAnimation anim;
        [SerializeField] private bool restart = true;
        [SerializeField] private bool immediateComplete = false;

        private Action onCompleted;

        public override void Execute(Action onCompleted = null)
        {
            this.onCompleted = onCompleted;
            anim?.Play(() => {
                if(immediateComplete == false)
                {
                    OnComplete(this.onCompleted);
                }
            }, restart);
            if(immediateComplete == true)
            {
                OnComplete(this.onCompleted);
            }
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
    public class PlayDOTAActionMono : MonoBehaviour
    {
        [SerializeField] private DoTweenAnimation anim;
        [SerializeField] private bool restart = true;
        [SerializeField] private bool immediateComplete = false;
    }
#endif
}