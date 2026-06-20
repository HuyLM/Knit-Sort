using AtoGame.Base.UnityInspector.Editor;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Tutorial
{
    [CreateAssetMenu(fileName = "TutorialData", menuName = "Data/OtherModules/Tutorial/TutorialData")]
    public class TutorialData : ScriptableObject
    {
        [Header("Keys")]
        public int Key;//1
        public string KeyName; // ShopBuyTutorial
        public int[] NeedDoneKeys;
        public int[] NotShowWhenDoneKeys;
        [Header("Steps")]
        public int SaveAtStep;
        [ExtendScriptable] public TutorialStep[] steps;
        public bool AutoNextStep = true;
        [Header("Others")]
        public bool IsActive = true;


        protected Action onCompleted;
        protected Action onSkipCallback;

        protected int stepIndex;
        protected TutorialStep curStep;
        protected bool isShowingStep;

        public void Init()
        {
            for (int i = 0; i < steps.Length; ++i)
            {
                steps[i].Init();
            }
        }

        public bool CanShowTutorial()
        {
            if (TutorialController.Instance.CheckTutorialCompleted(Key))
            {
                TutorialController.Instance.Log($"Is Completed: {Key}");
                return false;
            }
            if (TutorialController.Instance.CheckTutorialCompleted(NotShowWhenDoneKeys))
            {
                TutorialController.Instance.Log($"tutorial {Key} can't show because NotShowWhenDoneKeys");
                return false;
            }

            if (IsActive == false)
            {
                TutorialController.Instance.Log($"tutorial {Key} is not active");
                return false;
            }

            if ((NeedDoneKeys == null || NeedDoneKeys.Length == 0) || TutorialController.Instance.CheckTutorialCompleted(NeedDoneKeys))
            {
                return true;
            }
            else
            {
                TutorialController.Instance.Log($"tutorial {Key} Need to done one of NeedDoneKeys first!");
                return false;
            }
        }

        public void Show(Action onCompleted)
        {
            // data
            isShowingStep = false;
            this.onCompleted = onCompleted;
            stepIndex = 0;
            ShowCurrentStep();
            if (SaveAtStep <= 0)
            {
                TutorialController.Instance.SaveKeys(new int[] { Key });
            }
        }

        public void EndCurrentStep()
        {
            curStep?.EndStep();
        }

        private void ShowCurrentStep()
        {
            if (isShowingStep == true)
            {
                TutorialController.Instance.Log("Can't show next step because current step is showing");
                return;
            }
            ForceShowCurrentStep();
        }

        public void ForceShowCurrentStep()
        {
            if (stepIndex < steps.Length)
            {
                isShowingStep = true;
                curStep = steps[stepIndex];
                curStep.ShowStep(OnStepCompleted);
            }
        }

        public void End()
        {
            onCompleted?.Invoke();
        }

        public void Skip()
        {
            if (SaveAtStep > stepIndex + 1)
            {
                TutorialController.Instance.SaveKeys(new int[] { Key });
            }
            if (curStep != null)
            {
                curStep.OnSkip();
            }
            onSkipCallback?.Invoke();
            End();
        }

        public void OnStepCompleted()
        {
            isShowingStep = false;
            if (SaveAtStep == stepIndex + 1)
            {
                TutorialController.Instance.SaveKeys(new int[] { Key });
            }
            stepIndex++;

            if (AutoNextStep)
            {
                ShowCurrentStep();
            }

            if (stepIndex >= steps.Length)
            {
                End();
            }
        }

        public void AssignSkipCallBack(Action onSkip)
        {
            this.onSkipCallback = onSkip;
        }

        public TutorialStep GetTutorialStep(int index)
        {
            if (index < 0 || index >= steps.Length)
            {
                TutorialController.Instance.Log("Get step out of range");
                return null;
            }
            return steps[index];
        }

        public void RemoveReference()
        {
            foreach (var s in steps)
            {
                s.RemoveReferences();
            }
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
