using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AtoGame.OtherModules.HUD
{
    public abstract class BasePopup : DOTweenFrame
    {
        [Header("[References]")]
        [SerializeField] protected TextMeshProUGUI titleText;
        [SerializeField] protected TextMeshProUGUI messageText;
        [SerializeField] protected Button closeTap;
        [SerializeField] protected Button closeButton;
        [SerializeField] protected TextMeshProUGUI closeText;

        protected Action closeAction;
        protected Action preCloseAction;

        protected virtual void Start()
        {
            if (closeButton)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }

            if (closeTap)
            {
                closeTap.onClick.AddListener(OnCloseButtonClicked);
            }
        }

        protected override void ActiveFrame()
        {
            base.ActiveFrame();
            OnClose(null);
            OnPreClose(null);
        }

        protected override void OnHiddenFrame()
        {
            base.OnHiddenFrame();
            SetTapState(true);
        }

        protected virtual void OnCloseButtonClicked()
        {
            Close();
        }

        public BasePopup OnClose(Action closeAction)
        {
            this.closeAction = closeAction;
            return this;
        }

        public BasePopup OnPreClose(Action preCloseAction)
        {
            this.preCloseAction = preCloseAction;
            return this;
        }

        protected void Close()
        {
            SetTapState(false);
            preCloseAction?.Invoke();
            Hide();
            closeAction?.Invoke();
        }

        public BasePopup SetTapState(bool interactable, bool show = true)
        {
            if (closeTap)
            {
                closeTap.gameObject.SetActive(show);
                if (show)
                {
                    closeTap.interactable = (interactable);
                }
            }
            return this;
        }

        public BasePopup SetCloseState(bool interactable, bool show = true)
        {
            if (closeButton)
            {
                closeButton.gameObject.SetActive(show);
                if (show)
                {
                    closeButton.interactable = (interactable);
                }
            }
            return this;
        }

        public BasePopup SetMessage(string message, bool show = true)
        {
            if (this.messageText)
            {
                this.messageText.gameObject.SetActive(show);
                if (show)
                {
                    this.messageText.text = message;
                }
            }
            return this;
        }

        public BasePopup SetCloseContent(string content, bool show = true)
        {
            if (this.closeText)
            {
                this.closeText?.gameObject.SetActive(show);
                if (show)
                {
                    this.closeText.text = content;
                }
            }
            return this;
        }

        public BasePopup SetTitle(string title, bool show = true)
        {
            if (this.titleText)
            {
                this.titleText.gameObject.SetActive(show);
                if (show)
                {
                    this.titleText.text = title;
                }
            }
            return this;
        }
    }
}
