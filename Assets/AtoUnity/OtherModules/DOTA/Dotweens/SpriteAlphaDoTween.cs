using AtoGame.Base.Helper;
using DG.Tweening;
using System;

namespace AtoGame.OtherModules.DOTA
{
    public class SpriteAlphaDoTween : BaseDoTween {

        private float preValue;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            float endValue = dota.FloatTo;
            if (dota.IsRelative)
            {
                if (dota.FromCurrent)
                {
                    endValue = dota.SpriteRendererTarget.color.a + dota.FloatTo;
                }
                else
                {
                    endValue = dota.FloatFrom + dota.FloatTo;
                }
            }
            Tween = dota.SpriteRendererTarget.DOFade(endValue, dota.BaseOptions.Duration);
            base.CreateTween(dota, onCompleted);
        }
        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if (dota.FromCurrent == false)
            {
                dota.SpriteRendererTarget.ChangeAlpha(dota.FloatFrom);
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            preValue = dota.SpriteRendererTarget.color.a;
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.SpriteRendererTarget.ChangeAlpha(preValue);
        }

        public override bool CheckShowFloatValues()
        {
            return true;
        }

        public override bool CheckShowSpriteRendererTarget()
        {
            return true;
        }

        public override bool CheckShowIsRelative()
        {
            return true;
        }
    }
}
