using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AtoGame.OtherModules.DOTA
{
    [CustomEditor(typeof(TweenAnimation)), CanEditMultipleObjects]
    public class TweenAnimationInspector : DoTweenAnimationInspector {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
