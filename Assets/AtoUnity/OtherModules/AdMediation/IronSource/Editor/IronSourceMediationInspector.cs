using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AtoGame.Mediation
{
#if ATO_IRONSOURCE_MEDIATION_ENABLE
    [CustomEditor(typeof(IronSourceMediation))]
    public class IronSourceMediationInspector : Editor
    {
        // Cached scriptable object editor
        private Editor editor = null;
        bool isExpanded = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AdIronSourceSettings.Instance.ToString();

            // Draw foldout arrow
            isExpanded = EditorGUILayout.Foldout(isExpanded, "Settings");

            if(isExpanded)
            {
                EditorGUI.indentLevel++;
                if (!editor)
                    Editor.CreateCachedEditor(AdIronSourceSettings.Instance, null, ref editor);
                editor.OnInspectorGUI();
                EditorGUI.indentLevel--;
            }
        }
    }
#endif
}
