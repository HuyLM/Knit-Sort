using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AtoGame.Base.UI
{
    public class ToggleSwitch : MonoBehaviour
    {
        [SerializeField] ActionMono onAction;
        [SerializeField] ActionMono offAction;
        [SerializeField] Button btn;

        protected bool _curState;
        public Action<bool> OnChangeSwitch;

        protected void Awake()
        {
            onAction?.Initialize();
            offAction?.Initialize();
            btn.onClick.AddListener(OnButtonClicked);
        }

        public void ForceSetState(bool state)
        {
            _curState = state;

            if(_curState == true)
            {
                onAction?.Execute();
            }
            else
            {
                offAction.Execute();
            }
        }

        public void SetState(bool state)
        {
            _curState = state;
            UpdateState();
        }

        protected void UpdateState()
        {
            if(_curState == true)
            {
                onAction?.Execute();
            }
            else
            {
                offAction.Execute();
            }
            OnChangeSwitch?.Invoke(_curState);
        }

        private void OnButtonClicked()
        {
            SetState(!_curState);
        }
    }
}
