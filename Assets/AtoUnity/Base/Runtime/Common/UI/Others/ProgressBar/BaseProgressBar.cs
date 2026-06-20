﻿
using AtoGame.Base.Helper;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace AtoGame.Base.UI
{
    public class BaseProgressBar : MonoBehaviour
    {
        [SerializeField] protected Image imgCurrentValueLerp;
        [SerializeField] protected Image imgCurrentValueReal;
        [SerializeField] protected RangeFloatValue updateSecondSpeedRange;
        [SerializeField] protected bool useSetWidth;

        protected float maxWidth;
        protected float distance;
        protected bool isCompleted;
        bool isLoaded;
        protected Action onCompleted;
        private bool isUseLerp;

        protected virtual void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if(isLoaded)
            {
                return;
            }
            isUseLerp = imgCurrentValueLerp != null;

            if (!isLoaded)
            {
                if (useSetWidth)
                {
                    maxWidth = imgCurrentValueReal.rectTransform.rect.width;
                }
                isLoaded = true;
            }
        }

        private void FillBar(Image img, float fillAmount)
        {
            if(img == null)
            {
                return;
            }
            if (!isLoaded)
            {
                if (useSetWidth)
                {
                    maxWidth = imgCurrentValueReal.rectTransform.rect.width;
                }
                isLoaded = true;
            }

            if (useSetWidth)
            {
                img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fillAmount * maxWidth);
            }
            else
            {
                img.fillAmount = fillAmount;
            }
        }

        public void FillBar(float startPct, float endPct)
        {
            ForceFillBar(startPct);
            StartFillBar(endPct);
        }

        public virtual void StartFillBar(float pct)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            if (isUseLerp)
            {
                StopAllCoroutines();
                StartCoroutine(ChangingBar(pct));
            }

            FillBar(imgCurrentValueReal, pct);
        }

        protected virtual IEnumerator ChangingBar(float pct)
        {
            isCompleted = false;
            WaitForSeconds deltaTime = new WaitForSeconds(UnityEngine.Time.fixedDeltaTime);
            yield return deltaTime;
            float preChange = 0; 
            if(useSetWidth)
            {
                preChange = imgCurrentValueLerp.rectTransform.rect.width / maxWidth;
            }
            else
            {
                preChange = imgCurrentValueLerp.fillAmount;
            }
            distance = Mathf.Abs(pct - preChange);
            float elapsed = 0f;
            float updateSpeedSecond = updateSecondSpeedRange.GetRatioValue(distance);
            while (elapsed < distance)
            {
                elapsed += updateSpeedSecond * UnityEngine.Time.fixedDeltaTime;
                float fillAmount = Mathf.Lerp(preChange, pct, elapsed / distance);
                FillBar(imgCurrentValueLerp, fillAmount);
                yield return deltaTime;
            }
            FillBar(imgCurrentValueLerp, pct);
            LerpCompleted();
        }

        protected virtual void LerpCompleted()
        {
            isCompleted = true;
            if (onCompleted != null)
            {
                Action onAction = onCompleted;
                onCompleted = null;
                onAction.Invoke();
            }
        }

        public virtual void ForceFillBar(float pct)
        {
            pct = Mathf.Clamp(pct, 0, 1);
            FillBar(imgCurrentValueLerp, pct);
            FillBar(imgCurrentValueReal, pct);

        }

        public void AddOnComplete(Action onComplete)
        {
            this.onCompleted = onComplete;
        }

        public void RemoveOnComplete()
        {
            this.onCompleted = null;
        }
    }
}