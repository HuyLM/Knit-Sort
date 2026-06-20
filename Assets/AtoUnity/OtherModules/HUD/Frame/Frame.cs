using AtoGame.Base.Helper;
using NaughtyAttributes;
using System;
using UnityEngine;

namespace AtoGame.OtherModules.HUD
{
    [DisallowMultipleComponent]
    public abstract class Frame : MonoBehaviour
    {
        [Foldout("Frame Events")]
        [SerializeField] private FrameEvent onShowing;
        [Foldout("Frame Events")]
        [SerializeField] private FrameEvent onHidding;
        [Foldout("Frame Events")]
        [SerializeField] private FrameEvent onPausing;
        [Foldout("Frame Events")]
        [SerializeField] private FrameEvent onResuming;

        protected FrameState state = FrameState.NotInitialized;
        protected HUD hud;

        protected Action onCompleted;
        protected bool instant;
        protected Action _onHidden;
        protected Action _onPaused;

        public HUD Hud { get => hud; private set => hud = value; }
        public bool Initialized { get => state != FrameState.NotInitialized; }

        public bool IsShowing
        {
            get => state == FrameState.Showing;
        }

        public bool IsHidding
        {
            get => state == FrameState.Hidding;
        }
        public bool IsPausing
        {
            get => state == FrameState.Pausing;
        }

        public abstract string GetName();

        public virtual bool CanOnTop()
        {
            return true;
        }


        public void SetState(FrameState state)
        {
            this.state = state;
        }

        public Frame Initialize(HUD hud)
        {
            if (!Initialized)
            {
                this.Hud = hud;
                OnInitialize(hud);
            }
            return this;
        }

        protected virtual void OnInitialize(HUD hud) 
        {
            state = FrameState.Hidding;
        }

        public Frame ShowByHUD(Action onCompleted = null, bool instant = false) // note: ShowByHUD -> Awake -> OnEnable -> ActiveFrame -> Start -> OnShowedFrame
        {
            if(IsHidding)
            {
                this.onCompleted = onCompleted;
                this.instant = instant;
                SetState(FrameState.Showing);
                onShowing?.Invoke(this);
                ActiveFrame();
                SetOnHidden(null);
                SetOnPaused(null);
            }
            return this;
        }

        protected virtual void ActiveFrame()
        {
            gameObject.SetActive(true);
            this.DelayFrame(2, OnShowedFrame);
        }

        protected virtual void OnShowedFrame()
        {
            this.onCompleted?.Invoke();
        }

        public Frame HideByHUD(Action onCompleted = null, bool instant = false) // note: HideByHUD -> DeactiveFrame -> OnDisable -> OnHiddenFrame
        {
            if(Initialized && !IsHidding)
            {
                this.onCompleted = onCompleted;
                this.instant = instant;
                SetState(FrameState.Hidding);
                onHidding?.Invoke(this);
                DeactiveFrame();
            }
            return this;
        }

        protected virtual void DeactiveFrame()
        {
            OnHiddenFrame();
            //this.DelayFrame(2, OnHiddenFrame);
        }

        protected virtual void OnHiddenFrame()
        {
            gameObject.SetActive(false);
            this.onCompleted?.Invoke();
            _onHidden?.Invoke();
        }

        public Frame PauseByHUD(Action onCompleted = null, bool instant = false)
        {
            if(IsShowing)
            {
                this.onCompleted = onCompleted;
                this.instant = instant;
                SetState(FrameState.Pausing);
                onPausing?.Invoke(this);
                PauseFrame();
            }
            return this;
        }

        protected virtual void PauseFrame()
        {
            this.DelayFrame(2, OnPausedFrame);
        }

        protected virtual void OnPausedFrame()
        {
            this.onCompleted?.Invoke();
            _onPaused?.Invoke();
        }

        public Frame ResumeByHUD(Action onCompleted = null, bool instant = false)
        {
            if (IsPausing == true)
            {
                this.onCompleted = onCompleted;
                this.instant = instant;
                SetState(FrameState.Showing);
                onResuming?.Invoke(this);
                ResumeFrame();
            }
            return this;
        }

        protected virtual void ResumeFrame()
        {
            this.DelayFrame(2, OnResumedFrame);
        }

        protected virtual void OnResumedFrame()
        {
            this.onCompleted?.Invoke();
        }

        public virtual bool Back()
        {
            return Hud.BackToPrevious(null);
        }

        public void Hide(bool instant = false)
        {
            hud.Hide(this, instant: instant);
        }

        public void Show()
        {
            hud.Show(this);
        }

        public void Pause()
        {
            hud.Pause(this);
        }

        public void Resume()
        {
            hud.Resume(this);
        }

        public Frame SetOnHidden(Action onHidden)
        {
            _onHidden = onHidden;
            return this;
        }

        public Frame SetOnPaused(Action onPaused)
        {
            _onPaused = onPaused;
            return this;
        }

        public enum FrameState
        {
            NotInitialized,
            Showing,
            Hidding,
            Pausing
        }
    }
}
