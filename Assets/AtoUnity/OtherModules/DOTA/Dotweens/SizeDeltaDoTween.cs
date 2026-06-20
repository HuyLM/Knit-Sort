using DG.Tweening;
using System;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    public class SizeDeltaDoTween : BaseDoTween {

        private Vector2 preValue;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            Vector2 endValue = dota.Vector2To;
            if (dota.IsRelative)
            {
                if (dota.FromCurrent)
                {
                    endValue = dota.RectTransformTarget.sizeDelta + dota.Vector2To;
                }
                else
                {
                    endValue = dota.Vector2From + dota.Vector2To;
                }
            }
            Tween = dota.RectTransformTarget.DOSizeDelta(endValue, dota.BaseOptions.Duration);
            base.CreateTween(dota, onCompleted);
        }
        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if (dota.FromCurrent == false)
            {
                dota.RectTransformTarget.sizeDelta = (dota.Vector2From);
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            preValue = dota.RectTransformTarget.sizeDelta;
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.RectTransformTarget.sizeDelta = (preValue);
        }

        public override bool CheckShowVector2Values()
        {
            return true;
        }

        public override bool CheckShowRectTransformTarget()
        {
            return true;
        }
        public override bool CheckShowIsRelative()
        {
            return true;
        }
    }
}
