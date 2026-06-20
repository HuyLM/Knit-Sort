using System;
using DG.Tweening;
using AtoGame.Base.Helper;

namespace AtoGame.OtherModules.DOTA
{
    public class AlphaDoTween : BaseDoTween
    {
        private float preValue;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            float endValue = dota.FloatTo;
            if (dota.IsRelative)
            {
                if (dota.FromCurrent)
                {
                    endValue = dota.GraphicTarget.color.a + dota.FloatTo;
                }
                else
                {
                    endValue = dota.FloatFrom + dota.FloatTo;
                }
            }
            Tween = dota.GraphicTarget.DOFade(endValue, dota.BaseOptions.Duration);
            base.CreateTween(dota, onCompleted);
        }
        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if (dota.FromCurrent == false)
            {
                dota.GraphicTarget.ChangeAlpha(dota.FloatFrom);
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            preValue = dota.GraphicTarget.color.a;
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.GraphicTarget.ChangeAlpha(preValue);
        }

        public override bool CheckShowFloatValues()
        {
            return true;
        }

        public override bool CheckShowGraphicTarget()
        {
            return true;
        }

        public override bool CheckShowIsRelative()
        {
            return true;
        }
    }
}
