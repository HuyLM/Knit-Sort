using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    public class PunchScaleDoTween : BaseDoTween {

        private Vector3 preValue;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            Vector3 endValue = dota.Vector3To;
            Tween = dota.TransformTarget.DOPunchScale(endValue, dota.BaseOptions.Duration, dota.IntValue_1, dota.FloatValue_1);
            base.CreateTween(dota, onCompleted);
        }
        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if (dota.FromCurrent == false)
            {
                dota.TransformTarget.localScale = (dota.Vector3From);
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            preValue = dota.TransformTarget.localScale;
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.TransformTarget.localScale = (preValue);
        }

        public override bool CheckShowVector3Values()
        {
            return true;
        }

        public override bool CheckShowFloat1()
        {
            return true;
        }

        public override bool CheckShowInt1()
        {
            return true;
        }

        public override string GetFloat1Lable()
        {
            return "Elasticity";
        }

        public override string GetInt1Lable()
        {
            return "Vibrato";
        }
        public override string GetToLable()
        {
            return "Punch";
        }

        public override bool CheckShowTransformTarget()
        {
            return true;
        }
    }
}
