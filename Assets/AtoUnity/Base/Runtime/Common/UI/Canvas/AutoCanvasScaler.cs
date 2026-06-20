using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using AtoGame.Base.Helper;

namespace AtoGame.Base.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class AutoCanvasScaler : MonoBehaviour
    {
        [SerializeField] private Camera cameraRef;
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField, Range(0f, 10f)] private float updateRate = 1f;
        [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(1.33f, 0f, 2f, 1f);
        [SerializeField] private float delay;

        private WaitForSeconds waitUpdate;
        private float currentAspect;

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            canvasScaler = GetComponent<CanvasScaler>();
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        }

        private void Awake()
        {
            if (updateRate > 0)
            {
                waitUpdate = new WaitForSeconds(updateRate);
            }
            else
            {
                waitUpdate = null;
            }
        }

        private void OnEnable()
        {
            StartCoroutine(AutoUpdate());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        [ContextMenu("CalculatorScale")]
        [Button("CalculatorScale")]
        private void CalculatorScale()
        {
            Camera camera = cameraRef;
            if(camera == null)
            {
                camera = Camera.main;
            }

            if (camera == null) return;
            if (canvasScaler == null) return;
            if (camera.aspect == currentAspect) return;

            currentAspect = camera.aspect;
            canvasScaler.matchWidthOrHeight = curve.Evaluate(currentAspect);
        }

        private IEnumerator AutoUpdate()
        {
            if(delay > 0)
            {
                yield return Yielder.Wait(delay);
            }
            while (true)
            {
                CalculatorScale();
                if (waitUpdate == null)
                {
                    yield break;
                }
                yield return waitUpdate;
            }
        }
    }
}