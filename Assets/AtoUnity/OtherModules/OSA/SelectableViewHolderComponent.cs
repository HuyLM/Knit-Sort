#if OSA_ENABLE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AtoGame.OtherModules.OptimizedScrollView
{
    public abstract class SelectableViewHolderComponent<T> : ViewHolderComponent<T> where T : IScrollViewItemModel
    {
        [SerializeField]
        protected Button btnSelect;

        private Action<SelectableViewHolderComponent<T>> onSelect;

        protected virtual void Start()
        {
            if(btnSelect != null)
            {
                btnSelect.onClick.AddListener(OnSelectButtonClicked);
            }
        }

        public SelectableViewHolderComponent<T> SetOnSelected(Action<SelectableViewHolderComponent<T>> onSelect)
        {
            this.onSelect = onSelect;
            return this;
        }

        protected virtual void OnSelectButtonClicked()
        {
            onSelect?.Invoke(this);
        }

        public SelectableViewHolderComponent<T> SetStateSelectButton(bool interaction, bool show)
        {
            if(btnSelect)
            {
                btnSelect.gameObject.SetActive(true);
                if(show)
                {
                    btnSelect.interactable = interaction;
                }
            }
            return this;
        }
    }
}
#endif