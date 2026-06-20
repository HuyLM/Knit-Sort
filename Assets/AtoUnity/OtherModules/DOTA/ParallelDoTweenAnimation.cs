using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    public class ParallelDoTweenAnimation : DoTweenAnimation {
        [SerializeField, TabGroup("Tab1", "Animation Setting")]
        private DoTweenAnimation[] dotas;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            for(int i = 0; i < dotas.Length; ++i)
            {
                dotas[i].Initialize();
            }
        }

        public override void Play(Action onCompleted, bool restart, bool isPreview = false)
        {
            base.Play(onCompleted, restart, isPreview);
            if (dotas.Length == 0)
            {

            }
            else
            {
                for (int i = 0; i < dotas.Length; ++i)
                {
                    dotaCallingCounter++;
                    dotas[i].Play(()=>{
                        CheckOnCompleted();
                    }, restart, isPreview);
                }
            }
            CheckOnCompleted();
        }

        public override void Stop(bool complete)
        {
            for (int i = 0; i < dotas.Length; ++i)
            {
                dotas[i].Stop(complete);
            }
            base.Stop(complete);
        }
    }
}
