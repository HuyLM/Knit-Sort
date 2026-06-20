using DG.Tweening;
using System;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    public class ShakeScaleDoTween : BaseDoTween {

        private Vector3 preValue;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            Vector3 endValue = dota.Vector3To;
            Tween = dota.TransformTarget.DOShakeScale(dota.BaseOptions.Duration, endValue, dota.IntValue_1, dota.FloatValue_1, dota.BoolValue_1, dota.ShakeRandomnessMode);
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

        public override bool CheckShowBool1()
        {
            return true;
        }

        public override string GetFloat1Lable()
        {
            return "Randomness";
        }

        public override string GetInt1Lable()
        {
            return "Vibrato";
        }

        public override string GetBool1Lable()
        {
            return "FadeOut";
        }

        public override string GetToLable()
        {
            return "Strength";
        }

        public override bool CheckShowTransformTarget()
        {
            return true;
        }

        public override bool CheckShowShakeRandomnessMode()
        {
            return true;
        }
    }
}
