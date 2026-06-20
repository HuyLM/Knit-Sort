using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AtoGame.Base.UI
{
    public abstract class SelectableDisplayer<TModel> : Displayer<TModel>
    {
        [SerializeField] private Button btnSelect;
        private Action<SelectableDisplayer<TModel>> onSelect;

        protected virtual void Start()
        {
            if(btnSelect != null)
            {
                btnSelect.onClick.AddListener(OnSelectButtonClicked);
            }
        }

        public SelectableDisplayer<TModel> SetOnSelected(Action<SelectableDisplayer<TModel>> onSelect)
        {
            this.onSelect = onSelect;
            return this;
        }

        protected virtual void OnSelectButtonClicked()
        {
            onSelect?.Invoke(this);
        }

        public SelectableDisplayer<TModel> SetStateSelectButton(bool interaction, bool show)
        {
            if (btnSelect)
            {
                btnSelect.gameObject.SetActive(true);
                if (show)
                {
                    btnSelect.interactable = interaction;
                }
            }
            return this;
        }
    }
}
