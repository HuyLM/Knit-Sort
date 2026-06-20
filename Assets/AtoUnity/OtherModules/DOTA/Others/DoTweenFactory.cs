

namespace AtoGame.OtherModules.DOTA
{
    public static class DoTweenFactory 
    {
        public static EName AnimationName;

        public static BaseDoTween CreateDoTween()
        {
            switch (AnimationName)
            {
                case EName.Delay:
                    return CreateObjectAnimation<DelayDoTween>();
                case EName.Alpha:
                    return CreateObjectAnimation<AlphaDoTween>();
                case EName.AnchorPosition:
                    return CreateObjectAnimation<AnchorPositionDoTween>();
                case EName.Color:
                    return CreateObjectAnimation<ColorDoTween>();
                case EName.CanvasGroup:
                    return CreateObjectAnimation<CanvasGroupDoTween>();
                case EName.FillAmount:
                    return CreateObjectAnimation<FillAmountDoTween>();
                case EName.LocalPosition:
                    return CreateObjectAnimation<LocalPositionDoTween>();
                case EName.Position:
                    return CreateObjectAnimation<PositionDoTween>();
                case EName.LocalRotation:
                    return CreateObjectAnimation<LocalRotationDoTween>();
                case EName.Rotate:
                    return CreateObjectAnimation<RotationDoTween>();
                case EName.LocalScale:
                    return CreateObjectAnimation<LocalScaleDoTween>();
                case EName.PunchAnchorPosition:
                    return CreateObjectAnimation<PunchAnchorPositionDoTween>();
                case EName.PunchPosition:
                    return CreateObjectAnimation<PunchPositionDoTween>();
                case EName.PunchRotation:
                    return CreateObjectAnimation<PunchRotationDoTween>();
                case EName.PunchScale:
                    return CreateObjectAnimation<PunchScaleDoTween>();
                case EName.ShakePosition:
                    return CreateObjectAnimation<ShakePositionDoTween>();
                case EName.ShakeRotation:
                    return CreateObjectAnimation<ShakeRotationDoTween>();
                case EName.ShakeScale:
                    return CreateObjectAnimation<ShakeScaleDoTween>();
                case EName.SizeDelta:
                    return CreateObjectAnimation<SizeDeltaDoTween>();
                case EName.SpriteAlpha:
                    return CreateObjectAnimation<SpriteAlphaDoTween>();
                case EName.Float:
                    return CreateObjectAnimation<FloatDoTween>();
            }
            return null;
        }


            private static BaseDoTween CreateObjectAnimation<T>() where T : BaseDoTween, new()
        {
            BaseDoTween animation = (BaseDoTween)new T();
            return animation;
        }

    }
}
