
using UnityEditor;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    [CustomPropertyDrawer(typeof(ItemFieldAttribute))]
    public class ItemFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isInteger = property.propertyType == SerializedPropertyType.Integer;

            if (isInteger)
            {
                label = EditorGUI.BeginProperty(position, label, property);
                int indent = EditorGUI.indentLevel;
                Rect contentPosition = EditorGUI.PrefixLabel(position, label);
                EditorGUI.indentLevel = 0;

                EditorGUI.BeginChangeCheck();

                int selectedValue = property.intValue;

                ItemFieldAttribute itemFieldattribute = (ItemFieldAttribute)attribute;
                string[] nameCollectors = itemFieldattribute.NameCollectors;
                int numberItem = 0;

                if(nameCollectors == null)
                {
                    numberItem = ItemDatabaseEditor.GetCount();
                }
                else
                {
                    for (int i = 0; i < nameCollectors.Length; ++i)
                    {
                        foreach (var itemType in ItemDatabaseEditor.GetAllItem(nameCollectors[i]))
                        {
                            numberItem++;
                        }
                    }
                }

               

                GUIContent[] contents = new GUIContent[numberItem + 1];
                contents[0] = new GUIContent("None", "None");
                int[] optionsValue = new int[numberItem + 1];
                optionsValue[0] = ItemDatabase.NoneId;

                int index = 1;
                if (nameCollectors == null)
                {
                    foreach (var itemType in ItemDatabaseEditor.GetAllItem())
                    {
                        string type = itemType.NameType;
                        string name = $"{itemType.Item.DisplayName} (ID: {itemType.Item.Id})";
                        contents[index] = new GUIContent(type + name);
                        optionsValue[index] = itemType.Item.Id;
                        index++;
                    }
                }
                else
                {
                    for (int i = 0; i < nameCollectors.Length; ++i)
                    {
                        foreach (var itemType in ItemDatabaseEditor.GetAllItem(nameCollectors[i]))
                        {
                            string type = string.Empty;
                            if (itemFieldattribute.RemoveCollectorName)
                            {
                                type = itemType.NameType;
                                int startIndex = type.IndexOf(nameCollectors[i] + "/");
                                type = type.Remove(startIndex, nameCollectors[i].Length + 1);
                            }
                            else
                            {
                                type = itemType.NameType;
                            }
                            string name = $"{itemType.Item.DisplayName} (ID: {itemType.Item.Id})";
                            contents[index] = new GUIContent(type + name);
                            optionsValue[index] = itemType.Item.Id;
                            index++;
                        }
                    }

                }
                Rect popupRect = contentPosition;
                popupRect.Set(popupRect.x, popupRect.y, popupRect.width * 0.8f, popupRect.height);
                selectedValue = EditorGUI.IntPopup(popupRect, selectedValue, contents, optionsValue);

                Rect fieldRect = contentPosition;
                fieldRect.Set(fieldRect.x + fieldRect.width * 0.8f, fieldRect.y, fieldRect.width * 0.2f, fieldRect.height);
                selectedValue = EditorGUI.DelayedIntField(fieldRect, selectedValue);

                //EditorGUI.IntField(position, selectedValue);

                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = selectedValue;
                }
                EditorGUI.EndProperty();
                EditorGUI.indentLevel = indent;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}