using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    public class CanvasGroupDoTween: BaseDoTween
    {
        private float preAlpha;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            float endValue = dota.FloatTo;
            if (dota.IsRelative)
            {
                if (dota.FromCurrent)
                {
                    endValue = dota.CanvasGroupTarget.alpha + dota.FloatTo;
                }
                else
                {
                    endValue = dota.FloatFrom + dota.FloatTo;
                }
            }
            Tween = dota.CanvasGroupTarget.DOFade(endValue, dota.BaseOptions.Duration);
            base.CreateTween(dota, onCompleted);
        }
        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if (dota.FromCurrent == false)
            {
                dota.CanvasGroupTarget.alpha = dota.FloatFrom;
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            preAlpha = dota.CanvasGroupTarget.alpha;
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.CanvasGroupTarget.alpha = preAlpha;
        }

        public override bool CheckShowFloatValues()
        {
            return true;
        }

        public override bool CheckShowCanvasGroupTarget()
        {
            return true;
        }

        public override bool CheckShowIsRelative()
        {
            return true;
        }
    }
}
