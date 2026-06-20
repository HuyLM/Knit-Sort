using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    public class RotationDoTween : BaseDoTween {

        private Vector3 preValue;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            Vector3 endValue = dota.Vector3To;
            if (dota.IsRelative)
            {
                if (dota.FromCurrent)
                {
                    endValue = dota.TransformTarget.rotation.eulerAngles + dota.Vector3To;
                }
                else
                {
                    endValue = dota.Vector3From + dota.Vector3To;
                }
            }
            Tween = dota.TransformTarget.DORotate(endValue, dota.BaseOptions.Duration, dota.RotateMode);
            base.CreateTween(dota, onCompleted);
        }
        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if (dota.FromCurrent == false)
            {
                dota.TransformTarget.rotation = Quaternion.Euler(dota.Vector3From);
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            preValue = dota.TransformTarget.rotation.eulerAngles;
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.TransformTarget.rotation = Quaternion.Euler(preValue);
        }


        public override bool CheckShowVector3Values()
        {
            return true;
        }

        public override bool CheckShowRotateMode()
        {
            return true;
        }

        public override bool CheckShowTransformTarget()
        {
            return true;
        }

        public override bool CheckShowIsRelative()
        {
            return true;
        }
    }
}
