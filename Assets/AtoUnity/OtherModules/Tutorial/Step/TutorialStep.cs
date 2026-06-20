using AtoGame.Base;
using AtoGame.Base.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Tutorial
{
    public abstract class TutorialStep : ScriptableObject
    {
        [Header("Configs")]
        public float DelayShow = 0.5f;
        public bool CanSkip;
        [Header("Text Box")]
        public bool IsShowTextBox = true;
        public string DescriptionKey;
        public bool UseCustomTextBoxPosition;
        public Vector2 TextBoxPosition;
        public Vector2 MinAnchor = new Vector2(0.5f, 0.5f);
        public Vector2 MaxAnchor = new Vector2(0.5f, 0.5f);
        public bool IsHideImmediate;
        [Header("Background")]
        public bool IsShowBg = true;
        public Color BgColor;

        protected Color previousColor;
        protected Action onStepCompleted;
        protected Action onStartCallback; // extend
        protected Action onEndCallback;// for extend

        //cache
        protected ITutorialUI tutorialUI;
        protected WaitForSecondsRealtime wfsDelayShow;

        public virtual void Init()
        {
            tutorialUI = TutorialController.Instance.UI;
            if(DelayShow > 0)
            {
                wfsDelayShow = new WaitForSecondsRealtime(DelayShow);
            }
        }

        public void ShowStep(Action onStepCompleted)
        {
            this.onStepCompleted = onStepCompleted;
            onStartCallback?.Invoke();
            if (DelayShow > 0)
            {
                CoroutineHelper.Start(IDelay());
            }
            else
            {
                OnShow();
            }
        }

        private IEnumerator IDelay()
        {
            tutorialUI.IgnoreInput(true);
            yield return wfsDelayShow;
            tutorialUI.IgnoreInput(false);
            OnShow();
        }

        protected virtual void OnShow()
        {
            tutorialUI.SetShowBG(IsShowBg);
            if (IsShowBg)
            {
                previousColor = tutorialUI.GetBgImage().color;
                tutorialUI.SetBgColor(BgColor);
            }
            if (IsShowTextBox)
            {
                tutorialUI.SetDescriptionText(tutorialUI.GetTranslate(DescriptionKey));
                if(UseCustomTextBoxPosition)
                {
                    tutorialUI.SetDescriptionAutoPosition();
                }
                else
                {
                    tutorialUI.SetDescriptionCustomPosition(MinAnchor, MaxAnchor, TextBoxPosition);
                }

                tutorialUI.ShowDescription();
                tutorialUI.IgnoreInput(true);
                tutorialUI.SetOnShowTextBoxCompleted(OnShowTextBoxCompleted);
            }

            if (CanSkip)
            {
                tutorialUI.SetShowSkipButton(true);
            }
        }

        protected virtual void OnShowTextBoxCompleted()
        {
            tutorialUI.IgnoreInput(false);
        }

        public virtual void EndStep()
        {
            if (IsShowBg)
            {
                tutorialUI.SetBgColor(previousColor);
            }
            tutorialUI.SetShowBG(false);
            if (IsShowTextBox)
            {
                tutorialUI.HideDescription(IsHideImmediate);
                tutorialUI.SetOnShowTextBoxCompleted(null);
            }
            if (CanSkip)
            {
                tutorialUI.SetShowSkipButton(false);
            }
            onStepCompleted?.Invoke();
            onEndCallback?.Invoke();
        }
        public abstract void AssignTarget(GameObject target);
        public abstract void RemoveReferences();

        public void AssignCallback(Action onStart, Action onEnd)
        {
            this.onStartCallback = onStart;
            this.onEndCallback = onEnd;
        }

        public void OnSkip()
        {
            onStepCompleted = null;
            EndStep();
        }

        [NaughtyAttributes.Button("Focus In Project")]
        private void FocusInProject()
        {
#if UNITY_EDITOR
            UnityEditor.Selection.activeObject = this;
            UnityEditor.EditorGUIUtility.PingObject(this);
#endif
        }
    }
}
