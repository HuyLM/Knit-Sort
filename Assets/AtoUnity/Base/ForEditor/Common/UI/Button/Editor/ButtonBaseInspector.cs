using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AtoGame.Base.UI
{
    [CustomEditor(typeof(ButtonBase)), CanEditMultipleObjects]
    public class ButtonBaseInspector : ButtonEditor
    {
        private ButtonBase button1;
        private SerializedProperty clickScaleProperty;
        private SerializedProperty tfScaleProperty;
        protected override void OnEnable()
        {
            base.OnEnable();
            button1 = target as ButtonBase;
            clickScaleProperty = serializedObject.FindProperty("clickScale");
            tfScaleProperty = serializedObject.FindProperty("tfScale");

        }
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour((ButtonBase)target), typeof(ButtonBase), false);
            GUI.enabled = true;
            GUILayout.Space(20);
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(clickScaleProperty);
            EditorGUILayout.PropertyField(tfScaleProperty);
        }
    }
}