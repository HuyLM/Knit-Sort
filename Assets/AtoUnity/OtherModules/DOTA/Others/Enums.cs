using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
   public enum EName {
        None = 0, 
        Delay,
        Alpha,
        AnchorPosition,
        Color,
        CanvasGroup,
        FillAmount,
        LocalPosition,
        Position,
        LocalRotation,
        Rotate,
        LocalScale,
        PunchAnchorPosition,
        PunchPosition,
        PunchRotation,
        PunchScale,
        ShakePosition,
        ShakeRotation,
        ShakeScale,
        SizeDelta,
        SpriteAlpha,
        Float

    }

    public enum EPlayType { ManualPlay, AutoPlay, AutoInitialize }

    public enum ECallbackType { OnStart, OnTweenStart, OnTweenComplete, OnComplete }

    [System.Serializable]
    public struct BaseOptions {
        [FoldoutGroup("Base Options", true)] public bool StopOnDisable ;
        [FoldoutGroup("Base Options")] public bool IsSpeedBase;
        [FoldoutGroup("Base Options")] public float Delay;
        [FoldoutGroup("Base Options")] public float Duration;
        [FoldoutGroup("Base Options")] public bool IgnoreTimeScale;
        [FoldoutGroup("Base Options")] public int LoopNumber;
        [FoldoutGroup("Base Options"), ShowIf("@LoopNumber != 0 && LoopNumber != 1")] public LoopType LoopType;
        [FoldoutGroup("Base Options")] public Ease Ease;
        [FoldoutGroup("Base Options"), ShowIf("Ease", Ease.INTERNAL_Custom)] public AnimationCurve Curve;

        public BaseOptions(bool stopOnDisable, bool isSpeedBase, float delay, float duration, bool ignoreTimeScale, int loopNumber, LoopType loopType, Ease ease, AnimationCurve curve)
        {
            StopOnDisable = stopOnDisable;
            IsSpeedBase = isSpeedBase;
            Delay = delay;
            Duration = duration;
            IgnoreTimeScale = ignoreTimeScale;
            LoopNumber = loopNumber;
            LoopType = loopType;
            Ease = ease;
            Curve = curve;
        }

        public static BaseOptions Default
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new BaseOptions(true, false, 0, 0.5f, false, 1, LoopType.Yoyo, Ease.Linear, AnimationCurve.Linear(0, 0 , 1, 1));
            }
        }
    }

    public abstract class BaseDoTween
    {
        public Tween Tween { get; protected set; }


        public virtual bool CheckShowProperties()
        {
            return false;
        }

        public virtual bool CheckShowIsRelative()
        {
            return false;
        }

        public virtual bool CheckShowFloatValues()
        {
            return false;
        }

        public virtual bool CheckShowVector2Values()
        {
            return false;
        }

        public virtual bool CheckShowColorValues()
        {
            return false;
        }

        public virtual bool CheckShowVector3Values()
        {
            return false;
        }

        public virtual bool CheckShowRotateMode()
        {
            return false;
        }

        public virtual bool CheckShowShakeRandomnessMode()
        {
            return false;
        }

        public virtual bool CheckShowFloat1()
        {
            return false;
        }

        public virtual bool CheckShowInt1()
        {
            return false;
        }

        public virtual bool CheckShowBool1()
        {
            return false;
        }

        public virtual string GetFromLable()
        {
            return "From";
        }

        public virtual string GetToLable()
        {
            return "To";
        }

        public virtual string GetFloat1Lable()
        {
            return string.Empty;
        }

        public virtual string GetInt1Lable()
        {
            return string.Empty;
        }

        public virtual string GetBool1Lable()
        {
            return string.Empty;
        }

        public virtual bool CheckShowTransformTarget()
        {
            return false;
        }

        public virtual bool CheckShowGraphicTarget()
        {
            return false;
        }

        public virtual bool CheckShowRectTransformTarget()
        {
            return false;
        }

        public virtual bool CheckShowCanvasGroupTarget()
        {
            return false;
        }

        public virtual bool CheckShowImageTarget()
        {
            return false;
        }

        public virtual bool CheckShowSpriteRendererTarget()
        {
            return false;
        }

        public virtual bool CheckShowFloatGetSetTarget()
        {
            return false;
        }

        public virtual void CreateTween(TweenAnimation dota, Action onCompleted)
        {
            if (Tween != null)
            {
                Tween.SetSpeedBased(dota.BaseOptions.IsSpeedBase).SetUpdate(dota.BaseOptions.IgnoreTimeScale)
                            .SetDelay(dota.BaseOptions.Delay).OnComplete(() =>
                            {
                                dota.OnTweenCompleteEvent();
                                onCompleted?.Invoke();
                            })
                            .OnStart(() => {
                                dota.OnTweenStartEvent();
                            });
                if (dota.BaseOptions.LoopNumber != 0 && dota.BaseOptions.LoopNumber != 1)
                {
                    Tween.SetLoops(dota.BaseOptions.LoopNumber, dota.BaseOptions.LoopType);
                }
                if (dota.BaseOptions.Ease == Ease.INTERNAL_Custom)
                {
                    Tween.SetEase(dota.BaseOptions.Curve);
                }
                else
                {
                    Tween.SetEase(dota.BaseOptions.Ease);
                }
            }
        }

        public virtual void Play(TweenAnimation dota, System.Action onCompleted)
        {
            CreateTween(dota, onCompleted);
        }

        public virtual void Stop(TweenAnimation dota, bool complete = false)
        {
            Tween?.Kill(complete);
            if (complete && dota.BaseOptions.LoopNumber < 0)
            {
                ToEndValue(dota);
            }
        }

        public virtual void ResetState(TweenAnimation dota)
        {

        }

        public virtual void ToEndValue(TweenAnimation dota)
        {

        }

        public virtual void Save(TweenAnimation dota)
        {
        }

        public virtual void Load(TweenAnimation dota)
        {
        }

        public virtual void PlayPreview(TweenAnimation dota, Action onCompleted)
        {
            Save(dota);
            ResetState(dota);
            CreateTween(dota, onCompleted);
        }

        public virtual void StopPreview(TweenAnimation dota)
        {
            Tween?.Rewind();
            Tween?.Kill(false);
            Load(dota);
        }
    }

}
