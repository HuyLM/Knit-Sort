using DG.Tweening;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    [CustomEditor(typeof(DoTweenAnimation)), CanEditMultipleObjects]
    public abstract class DoTweenAnimationInspector : OdinEditor {
        private static List<TweenAnimation> playingDOTAList = new List<TweenAnimation>();
        private bool isPlaying;
        protected DoTweenAnimation dotweenAnimation;

        protected override void OnEnable()
        {
            base.OnEnable();
            isPlaying = false;
            dotweenAnimation = target as DoTweenAnimation;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Stop();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            if(isPlaying)
            {
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if(GUILayout.Button("Stop", GUILayout.Height(50)))
                {
                    Stop();
                }
                GUI.backgroundColor = oldColor;
            }
            else
            {
                if(GUILayout.Button("Play", GUILayout.Height(50)))
                {
                    Play();
                }
            }
            GUILayout.EndHorizontal();
            base.OnInspectorGUI();
        }

        public void PrepareTweenForPreview(TweenAnimation dota, Tween tween)
        {
            playingDOTAList.Insert(0, dota);
            DG.DOTweenEditor.DOTweenEditorPreview.PrepareTweenForPreview(tween, false);
        }

        private void Play()
        {
            playingDOTAList.Clear();
            isPlaying = true;
            DG.DOTweenEditor.DOTweenEditorPreview.Start();
            TweenAnimation.AddPrepareTweenForPreviewFunction(PrepareTweenForPreview);
            dotweenAnimation.PlayPreview();
        }


        private void Stop()
        {
            if (isPlaying)
            {
                isPlaying = false;
                for (int i = playingDOTAList.Count - 1; i >= 0; i--)
                {
                    playingDOTAList[i].Stop();
                }
                DG.DOTweenEditor.DOTweenEditorPreview.Stop();
            }
        }
    }
}
