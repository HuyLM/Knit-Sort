using DG.Tweening;
using UnityEditor;
using Sirenix.OdinInspector;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using AtoGame.Base.UI;

namespace AtoGame.OtherModules.DOTA
{
    public class TweenAnimation : DoTweenAnimation {
        [SerializeField, LabelText("Select Animation"), TabGroup("Tab1", "Animation Setting"), OnValueChanged(nameof(OnAnimationNameValueChanged))]
        private EName _animationName;

        #region Target
        [SerializeField, TabGroup("Tab1", "Animation Setting"), ShowIf(nameof(CheckShowTransformTarget)), OnValueChanged(nameof(OnTransformTargetValueChanged))]
        public Transform TransformTarget;
        [SerializeField, TabGroup("Tab1", "Animation Setting"), ShowIf(nameof(CheckShowGraphicTarget)), OnValueChanged(nameof(OnGraphicTargetValueChanged))]
        public Graphic GraphicTarget;
        [SerializeField, TabGroup("Tab1", "Animation Setting"), ShowIf(nameof(CheckShowRectTransformTarget)), OnValueChanged(nameof(OnRectTransformTargetValueChanged))]
        public RectTransform RectTransformTarget;
        [SerializeField, TabGroup("Tab1", "Animation Setting"), ShowIf(nameof(CheckShowCanvasGroupTarget)), OnValueChanged(nameof(OnCanvasGroupTargetValueChanged))]
        public CanvasGroup CanvasGroupTarget;
        [SerializeField, TabGroup("Tab1", "Animation Setting"), ShowIf(nameof(CheckShowImageTarget)), OnValueChanged(nameof(OnImageTargetValueChanged))]
        public Image ImageTarget;
        [SerializeField, TabGroup("Tab1", "Animation Setting"), ShowIf(nameof(CheckShowSpriteRendererTarget)), OnValueChanged(nameof(OnSpriteRendererTargetValueChanged))]
        public SpriteRenderer SpriteRendererTarget;
        [SerializeField, TabGroup("Tab1", "Animation Setting"), ShowIf(nameof(CheckShowFloatGetSetTarget)), OnValueChanged(nameof(OnFloatGetSetTargetValueChanged))]
        public FloatGetSet FloatGetSetTarget;
        #endregion Target

        [SerializeField, HideLabel, TabGroup("Tab1", "Animation Setting"), ShowIf(nameof(CheckShowProperties))]
        public BaseOptions BaseOptions = BaseOptions.Default;

        #region Values
        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowProperties))]
        public bool FromCurrent;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowFloatFrom)), LabelText("$" + nameof(GetFromLable))]
        public float FloatFrom;
        private bool CheckShowFloatFrom => CheckShowFloatValues() && !FromCurrent;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowFloatValues)), LabelText("$" + nameof(GetToLable))]
        public float FloatTo; 

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowUniform)), OnValueChanged(nameof(OnUniformValueChanged))]
        public bool Uniform;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowFromUniformValue)), LabelText("$" + nameof(GetFromLable)), OnValueChanged(nameof(OnFromUniformValueChanged))]
        public float FromUniformValue;
        private bool CheckShowFromUniformValue => CheckShowUniform() && !FromCurrent && Uniform;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowToUniformValue)), LabelText("$" + nameof(GetToLable)), OnValueChanged(nameof(OnToUniformValueChanged))]
        public float ToUniformValue;
        private bool CheckShowToUniformValue => CheckShowUniform() && Uniform;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, LabelText("$" + nameof(GetFromLable)), ShowIf(nameof(CheckShowVector2From))]
        public Vector2 Vector2From;
        private bool CheckShowVector2From => CheckShowVector2Values() && !FromCurrent && !Uniform;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowVector2To)), LabelText("$" + nameof(GetToLable))]
        public Vector2 Vector2To;
        private bool CheckShowVector2To => CheckShowVector2Values() && !Uniform;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, LabelText("$" + nameof(GetFromLable)), ShowIf(nameof(CheckShowVector3From))]
        public Vector3 Vector3From;
        public bool CheckShowVector3From => CheckShowVector3Values() && !FromCurrent && !Uniform;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowVector3To)), LabelText("$" + nameof(GetToLable))]
        public Vector3 Vector3To;
        private bool CheckShowVector3To => CheckShowVector3Values() && !Uniform;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowColorFrom)), LabelText("$" + nameof(GetFromLable))]
        public Color ColorFrom = Color.white;
        private bool CheckShowColorFrom => CheckShowColorValues() && !FromCurrent;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowColorValues)), LabelText("$" + nameof(GetToLable))]
        public Color ColorTo = Color.black;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowIsRelative))]
        public bool IsRelative;
        #endregion Values

        #region Other Values
        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowShakeRandomnessMode))]
        public ShakeRandomnessMode ShakeRandomnessMode;

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowRotateMode))]
        public RotateMode RotateMode;// LocalRotation , Rotate

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowFloat1)), LabelText("$" + nameof(GetFloat1Lable))]
        public float FloatValue_1; // PunchAnchorPosition, PunchPosition, PunchRotation, PunchScale, ShakePosition, ShakeRotation, ShakeScale

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowInt1)), LabelText("$" + nameof(GetInt1Lable))]
        public int IntValue_1; // PunchAnchorPosition, PunchPosition, PunchRotation, PunchScale, ShakePosition, ShakeRotation, ShakeScale

        [FoldoutGroup("Tab1/Animation Setting/Custom Options"), SerializeField, ShowIf(nameof(CheckShowBool1)), LabelText("$" + nameof(GetBool1Lable))]
        public bool BoolValue_1; // ShakePosition, ShakeRotation, ShakeScale  
        #endregion Other Values

        [SerializeField, LabelText("On Tween Start"), TabGroup("Tab1", "Callback Setting"), ShowIf(nameof(callbackType), ECallbackType.OnTweenStart), Space]
        protected DoTweenAnimation tweenStartDota;
        [SerializeField, LabelText("On Tween Complete"), TabGroup("Tab1", "Callback Setting"), ShowIf(nameof(callbackType), ECallbackType.OnTweenComplete), Space]
        protected DoTweenAnimation tweenCompleteDota;
        [SerializeField, LabelText("On Tween Start"), TabGroup("Tab1", "Callback Setting"), ShowIf(nameof(callbackType), ECallbackType.OnTweenStart), Space]
        private UnityEvent tweenStartCallback;
        [SerializeField, LabelText("On Tween Complete"), TabGroup("Tab1", "Callback Setting"), ShowIf(nameof(callbackType), ECallbackType.OnTweenComplete), Space]
        private UnityEvent tweenCompleteCallback;
        

        private BaseDoTween _baseDoTween;
        private Component _target;

        public Tween Tween
        {
            get => _baseDoTween == null ? null : _baseDoTween.Tween;
        }

        private void OnValidate()
        {
            CreateDoTween();
            if(_target != null)
            {
                TransformTarget = _target.transform;
                GraphicTarget = _target.GetComponent<Graphic>();
                RectTransformTarget = _target.GetComponent<RectTransform>();
                CanvasGroupTarget = _target.GetComponent<CanvasGroup>();
                ImageTarget = _target.GetComponent<Image>();
                SpriteRendererTarget = _target.GetComponent<SpriteRenderer>();
                FloatGetSetTarget = _target.GetComponent<FloatGetSet>();
            }
        }

        #region Check Show Properties
        private bool CheckShowProperties()
        {
            return _baseDoTween != null;
        }

        private bool CheckShowFloatValues()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowFloatValues();
        }

        private bool CheckShowUniform()
        {
            return CheckShowVector2Values() || CheckShowVector3Values();
        }

        private bool CheckShowIsRelative()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowIsRelative();
        }

        private bool CheckShowVector2Values()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowVector2Values();
        }

        private bool CheckShowColorValues()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowColorValues();
        }

        private bool CheckShowVector3Values()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowVector3Values();
        }

        private bool CheckShowRotateMode()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowRotateMode();
        } 
        
        private bool CheckShowShakeRandomnessMode()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowShakeRandomnessMode();
        }

        private bool CheckShowFloat1()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowFloat1();
        }

        private bool CheckShowInt1()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowInt1();
        }

        private bool CheckShowBool1()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowBool1();
        }

        private bool CheckShowTransformTarget()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowTransformTarget();
        }

        private bool CheckShowGraphicTarget()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowGraphicTarget();
        }

        private bool CheckShowRectTransformTarget()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowRectTransformTarget();
        }

        private bool CheckShowCanvasGroupTarget()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowCanvasGroupTarget();
        }

        private bool CheckShowImageTarget()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowImageTarget();
        }

        private bool CheckShowSpriteRendererTarget()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowSpriteRendererTarget();
        }

        private bool CheckShowFloatGetSetTarget()
        {
            return CheckShowProperties() && _baseDoTween.CheckShowFloatGetSetTarget();
        }

        #endregion Check Show Properties

        #region Lable Text
        private string GetFromLable()
        {
            return _baseDoTween.GetFromLable();
        }

        private string GetToLable()
        {
            return _baseDoTween.GetToLable();
        }

        private string GetFloat1Lable()
        {
            if(CheckShowFloat1())
            {
                return _baseDoTween.GetFloat1Lable();
            }
            return string.Empty;
        }

        private string GetInt1Lable()
        {
            if (CheckShowInt1())
            {
                return _baseDoTween.GetInt1Lable();
            }
            return string.Empty;
        }

        private string GetBool1Lable()
        {
            if (CheckShowBool1())
            {
                return _baseDoTween.GetBool1Lable();
            }
            return string.Empty;
        }
        #endregion Lable Text

        #region On Value Changed 
        private void OnAnimationNameValueChanged()
        {
            CreateDoTween();
        }
        private void OnUniformValueChanged()
        {
            if (Uniform)
            {
                if(CheckShowVector2Values())
                {
                    FromUniformValue = Vector2From.x;
                    ToUniformValue = Vector2To.x;
                }
                if (CheckShowVector3Values())
                {
                    FromUniformValue = Vector3From.x;
                    ToUniformValue = Vector3To.x;
                }
            }
            else
            {
                Vector2From = Vector2.one * FromUniformValue;
                Vector2To = Vector2.one * ToUniformValue;

                Vector3From = Vector3.one * FromUniformValue;
                Vector3To = Vector3.one * ToUniformValue;
            }
            
        }
        private void OnFromUniformValueChanged()
        {
            Vector2From = Vector2.one * FromUniformValue;
            Vector3From = Vector3.one * FromUniformValue;
        }
        private void OnToUniformValueChanged()
        {
            Vector2To = Vector2.one * ToUniformValue;
            Vector3To = Vector3.one * ToUniformValue;
        }

        private void OnTransformTargetValueChanged()
        {
            _target = TransformTarget;
        }
        private void OnGraphicTargetValueChanged()
        {
            _target = GraphicTarget;
        }
        private void OnRectTransformTargetValueChanged()
        {
            _target = RectTransformTarget;
        }
        private void OnCanvasGroupTargetValueChanged()
        {
            _target = CanvasGroupTarget;
        }
        private void OnImageTargetValueChanged()
        {
            _target = ImageTarget;
        }
        private void OnSpriteRendererTargetValueChanged()
        {
            _target = SpriteRendererTarget;
        }
        private void OnFloatGetSetTargetValueChanged()
        {
            _target = FloatGetSetTarget;
        }

        #endregion On Value Changed 

        #region Play Editor

#if UNITY_EDITOR
        private static Action<TweenAnimation, Tween> prepareTweenForPreviewFunc;
        public static void AddPrepareTweenForPreviewFunction(Action<TweenAnimation, Tween> previewFunc)
        {
            prepareTweenForPreviewFunc = previewFunc;
        }

#endif
        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();
            CreateDoTween();
        }

        public override void Play(Action onCompleted, bool restart, bool isPreview = false)
        {
         
            base.Play(onCompleted, restart, isPreview);
            if (Application.isPlaying)
            {
                if (isInitialized == false)
                {
                    return;
                }
                Play(restart);
            }
            else
            {

                if (_baseDoTween != null)
                {
                    _baseDoTween.PlayPreview(this, onCompleted);
#if UNITY_EDITOR
                    if (prepareTweenForPreviewFunc != null)
                    {
                        prepareTweenForPreviewFunc.Invoke(this, _baseDoTween.Tween);
                    }
#endif
                }
            }
        }


        private void Play(bool restart)
        {
            if (restart)
            {
                ResetState();
            }
            if (_baseDoTween != null)
            {
                _baseDoTween.Play(this, () => {
                    CheckOnCompleted();
                });
            }
        }

        public void ResetState()
        {
            if (_baseDoTween != null)
            {
                _baseDoTween.Stop(this);
                _baseDoTween.ResetState(this);
            }
        }

        public override void Stop(bool complete)
        {
            if (_baseDoTween != null)
            {
                if (Application.isPlaying)
                {
                    _baseDoTween.Stop(this, complete);
                }
                else
                {
                    _baseDoTween.StopPreview(this);
                }
            }

            tweenStartDota?.Stop(complete);
            tweenCompleteDota?.Stop(complete);
            base.Stop(complete);
        }

        private void CreateDoTween()
        {
            DoTweenFactory.AnimationName = _animationName;
            _baseDoTween = DoTweenFactory.CreateDoTween();
        }

        public void OnTweenStartEvent()
        {
            tweenStartCallback?.Invoke();
            if (tweenStartDota != null)
            {
                dotaCallingCounter++;
                tweenStartDota.Play(() => {
                    CheckOnCompleted();
                }, true);
            }
        }

        public void OnTweenCompleteEvent()
        {
            tweenCompleteCallback?.Invoke();
            if (tweenCompleteDota != null)
            {
                dotaCallingCounter++;
                tweenCompleteDota.Play(() => {
                    CheckOnCompleted();
                }, true);
            }
        }
    }
}
