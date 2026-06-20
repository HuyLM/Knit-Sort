
using System;
using UnityEngine;
namespace AtoGame.Base.Helper
{
    [Serializable]
    public struct RangeIntValue
    {
        public int startValue;
        public int endValue;

        public RangeIntValue(int start, int end)
        {
            this.startValue = start;
            this.endValue = end;
        }

        public int GetRandomValue()
        {
            return RandomHelper.RandomInRange(this);
        }
    }

    [Serializable]
    public struct RangeFloatValue
    {
        public float startValue;
        public float endValue;

        public RangeFloatValue(float start, float end)
        {
            this.startValue = start;
            this.endValue = end;
        }

        public float GetRandomValue()
        {
            return RandomHelper.RandomInRange(this);
        }

        public float GetRatioValue(float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            return startValue + (endValue - startValue) * ratio;
        }
    }
}
