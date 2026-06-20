using System;
using UnityEngine;

namespace AtoGame.Base.UI
{
    public class TabControl : MonoBehaviour
    {
        [SerializeField] protected TabButton[] tabButtons;
        protected Action<int, int, bool> onTabChanged;
        protected int curTabIndex;

        public int CurTabIndex { get => curTabIndex; }

        protected virtual void Start()
        {
            for (int i = 0; i < tabButtons.Length; ++i)
            {
                tabButtons[i].SetOnTabClicked(OnTabButtonClicked);
            }
        }

        public virtual void Init()
        {
            for (int i = 0; i < tabButtons.Length; ++i)
            {
                tabButtons[i].SetActiveTab(true);
                tabButtons[i].Init();
            }
        }

        protected virtual void OnTabButtonClicked(int index)
        {
            SelectTab(index);
        }

        protected void SelectTab(int index)
        {
            ChangeTab(index, false);
        }

        public void ForceSelectTab(int index)
        {
            ChangeTab(index, true);
        }

        protected virtual void ChangeTab(int index, bool forceChange)
        {
            for (int i = 0; i < tabButtons.Length; ++i)
            {
                if (index == tabButtons[i].TabIndex) // new tab button
                {
                    tabButtons[i].SetActiveTab(false);
                }
                else if (curTabIndex == tabButtons[i].TabIndex) // old tab button
                {
                    tabButtons[i].SetActiveTab(true);
                }
            }
            onTabChanged?.Invoke(curTabIndex, index, forceChange);
            curTabIndex = index;
        }

        public void AddOnTabChanged(Action<int, int, bool> onTabChanged)
        {
            this.onTabChanged += onTabChanged;
        }

    }
}
