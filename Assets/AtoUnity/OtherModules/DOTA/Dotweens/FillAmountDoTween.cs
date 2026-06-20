using DG.Tweening;
using System;

namespace AtoGame.OtherModules.DOTA
{
    public class FillAmountDoTween : BaseDoTween {

        private float preFillAmount;

        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            float endValue = dota.FloatTo;
            if (dota.IsRelative)
            {
                if (dota.FromCurrent)
                {
                    endValue = dota.ImageTarget.fillAmount + dota.FloatTo;
                }
                else
                {
                    endValue = dota.FloatFrom + dota.FloatTo;
                }
            }
            Tween = dota.ImageTarget.DOFillAmount(endValue, dota.BaseOptions.Duration);
            base.CreateTween(dota, onCompleted);
        }
        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if (dota.FromCurrent == false)
            {
                dota.ImageTarget.fillAmount = (dota.FloatFrom);
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            preFillAmount = dota.ImageTarget.fillAmount;
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.ImageTarget.fillAmount = preFillAmount;
        }

        public override bool CheckShowIsRelative()
        {
            return true;
        }

        public override bool CheckShowFloatValues()
        {
            return true;
        }

        public override bool CheckShowImageTarget()
        {
            return true;
        }
    }
}
