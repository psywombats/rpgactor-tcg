using UnityEngine;
using UnityEditor;
using System;

namespace RpgActorTGC
{
    [CustomPropertyDrawer(typeof(StatSet))]
public class StatSetDrawer : PropertyDrawer {

    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label) 
    {
        var serialDictionary = property.FindPropertyRelative("serializedStats");
        var keys = serialDictionary.FindPropertyRelative("keys");
        var values = serialDictionary.FindPropertyRelative("values");

        EditorGUI.BeginProperty(pos, label, property);

        var fieldStyle = new GUIStyle(GUI.skin.textArea) { fixedWidth = 80 };

        pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);

        var seen = 0;
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            EditorGUI.LabelField(
                new Rect(pos.x, 24 + pos.y + seen * 18, pos.width, EditorGUIUtility.singleLineHeight),
                stat + ": ");

            var oldFloat = GetStatValue(property, stat);
            var newFloat = EditorGUI.FloatField(
                new Rect(pos.x + 82, 24 + pos.y + seen * 18, pos.width, EditorGUIUtility.singleLineHeight),
                oldFloat,
                fieldStyle);
            if (newFloat != oldFloat) {
                SetStatValue(property, stat, newFloat);
            }
            seen += 1;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
    {
        return (EditorGUIUtility.singleLineHeight + 2) * (Enum.GetValues(typeof(Stat)).Length + 1) + 6;
    }

    private float GetStatValue(SerializedProperty property, Stat stat) 
    {
        var serialDictionary = property.FindPropertyRelative("serializedStats");
        var keys = serialDictionary.FindPropertyRelative("keys");
        var values = serialDictionary.FindPropertyRelative("values");

        for (var i = 0; i < keys.arraySize; i += 1) 
        {
            if (keys.GetArrayElementAtIndex(i).stringValue == stat.ToString()) 
            {
                return values.GetArrayElementAtIndex(i).floatValue;
            }
        }
        return 0f;
    }

    private void SetStatValue(SerializedProperty property, Stat stat, float value) 
    {
        SerializedProperty serialDictionary = property.FindPropertyRelative("serializedStats");
        SerializedProperty keys = serialDictionary.FindPropertyRelative("keys");
        SerializedProperty values = serialDictionary.FindPropertyRelative("values");

        var applied = false;
        for (var i = 0; i < keys.arraySize; i += 1) 
        {
            if (keys.GetArrayElementAtIndex(i).stringValue == stat.ToString()) 
            {
                values.GetArrayElementAtIndex(i).floatValue = value;
                applied = true;
                break;
            }
        }

        if (!applied) 
        {
            keys.arraySize += 1;
            values.arraySize += 1;
            keys.InsertArrayElementAtIndex(keys.arraySize - 1);
            values.InsertArrayElementAtIndex(values.arraySize - 1);
            keys.GetArrayElementAtIndex(keys.arraySize - 1).stringValue = stat.ToString();
            values.GetArrayElementAtIndex(values.arraySize - 1).floatValue = value;
        }
        
        property.serializedObject.ApplyModifiedProperties();
    }
}
}
