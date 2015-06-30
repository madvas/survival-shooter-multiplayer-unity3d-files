/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using EnergyBarToolkit;
using UnityEditor;
using UnityEngine;
using PropertyDrawer = UnityEditor.PropertyDrawer;

[CustomPropertyDrawer(typeof(ObjectFinder))]
public class ObjectFinderEditor : PropertyDrawer {

    private float lineHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        GUI.Label(GetLineRect(position, 0), label);

        EditorGUI.indentLevel++;
        EditorGUI.PropertyField(GetLineRect(position, 1), property.FindPropertyRelative("chosenMethod"), new GUIContent("Lookup Method"));

        EditorGUI.indentLevel++;
        var method = GetChosenMethod(property);
        switch (method) {
            case ObjectFinder.Method.ByPath:
                EditorGUI.PropertyField(
                    GetLineRect(position, 2),
                    property.FindPropertyRelative("path"),
                    new GUIContent("Path"));
                break;
            case ObjectFinder.Method.ByTag:
                EditorGUI.PropertyField(
                    GetLineRect(position, 2),
                    property.FindPropertyRelative("tag"),
                    new GUIContent("Tag"));
                break;
            case ObjectFinder.Method.ByType:
                break;
            case ObjectFinder.Method.ByReference:
                EditorGUI.PropertyField(
                    GetLineRect(position, 2),
                    property.FindPropertyRelative("reference"),
                    new GUIContent("Camera"));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        EditorGUI.indentLevel -= 2;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        lineHeight = base.GetPropertyHeight(property, label);
        return lineHeight * GetLineCount(property);
    }

    private Rect GetLineRect(Rect rect, int index) {
        float space = lineHeight;
        return new Rect(rect.min.x, rect.min.y + index * space, rect.width, lineHeight);
    }

    private int GetLineCount(SerializedProperty property) {
        var method = GetChosenMethod(property);
        switch (method) {
            case ObjectFinder.Method.ByPath:
                return 3;
            case ObjectFinder.Method.ByTag:
                return 3;
            case ObjectFinder.Method.ByType:
                return 2;
            case ObjectFinder.Method.ByReference:
                return 3;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private ObjectFinder.Method GetChosenMethod(SerializedProperty property) {
        var index = property.FindPropertyRelative("chosenMethod").enumValueIndex;
        Array values = Enum.GetValues(typeof(ObjectFinder.Method));
        return (ObjectFinder.Method) values.GetValue(index);
    }
}