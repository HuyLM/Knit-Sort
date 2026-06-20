using DG.Tweening;
using System;

namespace AtoGame.OtherModules.DOTA
{
    public class FloatDoTween : BaseDoTween {

        private float preValue;
        public override void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            float endValue = dota.FloatTo;
            Tween = DOVirtual.Float(dota.FloatFrom, dota.FloatTo, dota.BaseOptions.Duration, (value) => {
                dota.FloatGetSetTarget.Set(value);
            });
            base.CreateTween(dota, onCompleted);
        }

        public override void ResetState(TweenAnimation dota)
        {
            base.ResetState(dota);
            if (dota.FromCurrent == false)
            {
                dota.FloatGetSetTarget.Set(dota.FloatFrom);
            }
        }

        public override void Save(TweenAnimation dota)
        {
            base.Save(dota);
            preValue = dota.FloatGetSetTarget.Get();
        }

        public override void Load(TweenAnimation dota)
        {
            base.Load(dota);
            dota.FloatGetSetTarget.Set(preValue);
        }


        public override bool CheckShowFloatValues()
        {
            return true;
        }

        public override bool CheckShowFloatGetSetTarget()
        {
            return true;
        }
    }
}
