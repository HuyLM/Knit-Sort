using UnityEditor;
using UnityEngine;
using static AtoGame.Base.UI.ButtonExplorer;

namespace AtoGame.Base.UI
{
    [CustomEditor(typeof(ButtonExplorer)), CanEditMultipleObjects]
    public class ButtonExplorerInspector : ButtonBaseInspector
    {
        protected ButtonExplorer buttonExplorer;
        private SerializedProperty mainBgProperty;
        private SerializedProperty clickSoundEnableProperty;
        private SerializedProperty clickSoundEffectProperty;
        private SerializedProperty disableTypeProperty;
        private SerializedProperty disableMaskProperty;
        private SerializedProperty enableColorProperty;
        private SerializedProperty disableColorProperty;
        private SerializedProperty enableMatProperty;
        private SerializedProperty disableMatProperty;
        private SerializedProperty enableSpriteProperty;
        private SerializedProperty disableSpriteProperty;

        private SerializedProperty onNormalStateProperty;
        private SerializedProperty onHighlishtedStateProperty;
        private SerializedProperty onPressedStateProperty;
        private SerializedProperty onSelectedStateProperty;
        private SerializedProperty onDisableStateProperty;




        protected override void OnEnable()
        {
            base.OnEnable();
            buttonExplorer = target as ButtonExplorer;
            mainBgProperty = serializedObject.FindProperty("mainBg");

            clickSoundEnableProperty = serializedObject.FindProperty("clickSoundEnable");
            clickSoundEffectProperty = serializedObject.FindProperty("clickSoundEffect");

            disableTypeProperty = serializedObject.FindProperty("disableType");

            disableMaskProperty = serializedObject.FindProperty("disableMask");

            enableColorProperty = serializedObject.FindProperty("enableColor");
            disableColorProperty = serializedObject.FindProperty("disableColor");

            enableMatProperty = serializedObject.FindProperty("enableMat");
            disableMatProperty = serializedObject.FindProperty("disableMat");

            enableSpriteProperty = serializedObject.FindProperty("enableSprite");
            disableSpriteProperty = serializedObject.FindProperty("disableSprite");

            onNormalStateProperty = serializedObject.FindProperty("onNormalState");
            onHighlishtedStateProperty = serializedObject.FindProperty("onHighlishtedState");
            onPressedStateProperty = serializedObject.FindProperty("onPressedState");
            onSelectedStateProperty = serializedObject.FindProperty("onSelectedState");
            onDisableStateProperty = serializedObject.FindProperty("onDisableState");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(mainBgProperty);

            EditorGUILayout.PropertyField(clickSoundEnableProperty);
            if (clickSoundEnableProperty.boolValue)
            {
                EditorGUILayout.PropertyField(clickSoundEffectProperty);
            }
            EditorGUILayout.PropertyField(disableTypeProperty);
            DisableType type = buttonExplorer.MyDisableType;
            if (type == DisableType.NONE)
            {

            }
            else if (type == DisableType.MASK)
            {
                EditorGUILayout.PropertyField(disableMaskProperty);
            }
            else if (type == DisableType.COLOR)
            {
                EditorGUILayout.PropertyField(enableColorProperty);
                EditorGUILayout.PropertyField(disableColorProperty);
            }
            else if (type == DisableType.MATERIAL)
            {
                EditorGUILayout.PropertyField(enableMatProperty);
                EditorGUILayout.PropertyField(disableMatProperty);
            }
            else if (type == DisableType.SPRITE)
            {
                EditorGUILayout.PropertyField(enableSpriteProperty);
                EditorGUILayout.PropertyField(disableSpriteProperty);
            }

            EditorGUILayout.PropertyField(onNormalStateProperty);
            EditorGUILayout.PropertyField(onHighlishtedStateProperty);
            EditorGUILayout.PropertyField(onPressedStateProperty);
            EditorGUILayout.PropertyField(onSelectedStateProperty);
            EditorGUILayout.PropertyField(onDisableStateProperty);

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
