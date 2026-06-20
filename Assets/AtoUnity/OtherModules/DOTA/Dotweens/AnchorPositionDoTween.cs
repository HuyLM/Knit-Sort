using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AtoGame.OtherModules.DOTA
{
    public class AnchorPositionDoTween : BaseDoTween {

        private Vector2 prePosition;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            Vector2 endValue = dota.Vector2To;
            if(dota.IsRelative)
            {
                if (dota.FromCurrent)
                {
                    endValue = dota.RectTransformTarget.anchoredPosition + dota.Vector2To;
                }
                else
                {
                    endValue = dota.Vector2From + dota.Vector2To;
                }
            }
            Tween = dota.RectTransformTarget.DOAnchorPos(endValue, dota.BaseOptions.Duration);
            base.CreateTween(dota, onCompleted);
        }

        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if(dota.FromCurrent == false)
            {
                dota.RectTransformTarget.anchoredPosition = dota.Vector2From;
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            prePosition = dota.RectTransformTarget.anchoredPosition;
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.RectTransformTarget.anchoredPosition = prePosition;
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
