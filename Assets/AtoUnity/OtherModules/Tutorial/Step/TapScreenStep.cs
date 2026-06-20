using UnityEngine;

namespace AtoGame.OtherModules.Tutorial
{
    [CreateAssetMenu(fileName = "TapScreenStep", menuName = "Data/OtherModules/Tutorial/Step/TapScreen")]
    public class TapScreenStep : TutorialStep
    {

        private GameObject target;
        public override void AssignTarget(GameObject target)
        {
            if (target == null)
            {
                TutorialController.Instance.Log($"Target assgin is null");
                this.target = null;
                return;
            }
            this.target = target;
        }

        public override void RemoveReferences()
        {
            target = null;
        }

        protected override void OnShow()
        {
            base.OnShow();
            Show();
        }

        private void Show()
        {
            tutorialUI.SetOnTapScreenButton(OnTapContinueClicked);
            if (target != null)
            {
                tutorialUI.HighlightObject(target);
                IHighlightComponent[] highlightComponents = target.GetComponentsInChildren<IHighlightComponent>(includeInactive: true);
                if (highlightComponents != null)
                {
                    for (int i = 0; i < highlightComponents.Length; ++i)
                    {
                        highlightComponents[i].Show();
                    }
                }
            }
        }

        protected override void OnShowTextBoxCompleted()
        {
            base.OnShowTextBoxCompleted();
            tutorialUI.SetShowTapScreenButton(true);
        }

        public override void EndStep()
        {
            Hide();
            base.EndStep();
        }

        private void Hide()
        {
            tutorialUI.SetShowTapScreenButton(false);
            tutorialUI.SetOnTapScreenButton(null);
            if (target != null)
            {
                tutorialUI.LowlightObject(target);
                IHighlightComponent[] highlightComponents = target.GetComponentsInChildren<IHighlightComponent>(includeInactive: true);
                if (highlightComponents != null)
                {
                    for (int i = 0; i < highlightComponents.Length; ++i)
                    {
                        highlightComponents[i].Hide();
                    }
                }
            }
        }


        private void OnTapContinueClicked()
        {
            EndStep();
        }
    }
}
