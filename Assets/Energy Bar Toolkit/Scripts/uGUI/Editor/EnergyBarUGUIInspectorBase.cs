/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EnergyBarToolkit {

public class EnergyBarUGUIInspectorBase : EnergyBarInspectorBase {

    protected SerializedProperty spritesBackground;
    protected SerializedProperty spritesForeground;

    protected SerializedProperty label;

    protected SerializedProperty debugMode;

    private ReorderableList backgroundList;
    private ReorderableList foregroundList;

    private AnimBool formatHelpAnimBool = new AnimBool();

    public override void OnEnable() {
        base.OnEnable();

        spritesBackground = serializedObject.FindProperty("spritesBackground");
        spritesForeground = serializedObject.FindProperty("spritesForeground");

        label = serializedObject.FindProperty("label");

        debugMode = serializedObject.FindProperty("debugMode");

        backgroundList = CreateLayerList(spritesBackground, "Background Sprites");
        foregroundList = CreateLayerList(spritesForeground, "Foreground Sprites");

        formatHelpAnimBool.valueChanged.AddListener(Repaint);
    }

    protected ReorderableList CreateLayerList(SerializedProperty sprites, string label) {
        var list = new ReorderableList(serializedObject, sprites, true, true, true, true);
        list.drawHeaderCallback += rect => GUI.Label(rect, label);
        list.drawElementCallback += (rect, index, active, focused) => {
            rect.height = 16;
            rect.y += 2;
            FieldSprite(rect, sprites.GetArrayElementAtIndex(index), "Sprite");
        };
        list.onAddCallback += l => {
            sprites.arraySize++;
            var lastElement = sprites.GetArrayElementAtIndex(list.count - 1);
            var color = lastElement.FindPropertyRelative("color");
            color.colorValue = Color.white;
        };

        return list;
    }

    protected void FieldBackgroundSprites() {
        backgroundList.DoLayoutList();
    }

    protected void FieldForegroundSprites() {
        //FieldSprites((target as EnergyBarUGUIBase).spritesForeground);
        foregroundList.DoLayoutList();
    }

    protected void FieldSetNativeSize() {
        if (MadGUI.Button("Set Native Size")) {
            var b = (EnergyBarUGUIBase) target;
            MadUndo.RecordObject2(b.gameObject, "Set Native Size");
            b.SetNativeSize();
        }
    }

    protected void SectionDebugMode() {
        GUILayout.Label("Debug", "HeaderLabel");

        using (MadGUI.Indent()) {
            MadGUI.PropertyField(debugMode, "Debug Mode");
        }
    }

    private void FieldSprites(List<EnergyBarUGUIBase.SpriteTex> sprites) {
        var arrayList = new MadGUI.ArrayList<EnergyBarUGUIBase.SpriteTex>(sprites, tex => {
            FieldSprite(tex, "");
            return tex;
        });

        arrayList.onAdd += tex => tex.color = Color.white;

        if (arrayList.Draw()) {
            EditorUtility.SetDirty((target as EnergyBarUGUIBase).gameObject);
        }
    }

    public void FieldSprite(SerializedProperty p, string label) {
        FieldSprite(p, label, property => true);
    }

    public void FieldSprite(Rect rect, SerializedProperty p, string label) {
        FieldSprite(rect, p, label, property => true);
    }

    public void FieldSprite(SerializedProperty p, string label, MadGUI.Validator validator) {
        var sprite = p.FindPropertyRelative("sprite");
        var color = p.FindPropertyRelative("color");

        EditorGUILayout.BeginHorizontal();
        MadGUI.PropertyField(sprite, label, validator);
        EditorGUILayout.PropertyField(color, new GUIContent(""), GUILayout.Width(90));
        //MadGUI.PropertyField(color, "");
        EditorGUILayout.EndHorizontal();
    }

    public void FieldSprite(Rect rect, SerializedProperty p, string label, MadGUI.Validator validator) {
        var sprite = p.FindPropertyRelative("sprite");
        var color = p.FindPropertyRelative("color");

        //GUILayout.BeginArea(rect);
        //GUILayout.Label("Test");
//        //EditorGUILayout.BeginHorizontal();
        //MadGUI.PropertyField(sprite, label, validator);
        Rect r1, r2;
        HorizSplit(rect, 0.7f, out r1, out r2);

        EditorGUI.PropertyField(r1, sprite, new GUIContent(""));
        EditorGUI.PropertyField(r2, color, new GUIContent(""));

//        EditorGUILayout.PropertyField(color, new GUIContent(""), GUILayout.Width(90));
//        //MadGUI.PropertyField(color, "");
//        //EditorGUILayout.EndHorizontal();
        //GUILayout.EndArea();
    }

    private void HorizSplit(Rect inRect, float split, out Rect outRect1, out Rect outRect2) {
        outRect1 = new Rect(inRect.xMin, inRect.yMin, inRect.width * split, inRect.height);
        outRect2 = new Rect(outRect1.xMax, inRect.yMin, inRect.width - outRect1.width, inRect.height);
    }

    public void FieldSprite(EnergyBarUGUIBase.SpriteTex tex, string label) {
        EditorGUILayout.BeginHorizontal();
        tex.sprite = (Sprite) EditorGUILayout.ObjectField(label, tex.sprite, typeof (Sprite), false);
        tex.color = EditorGUILayout.ColorField(tex.color);
        EditorGUILayout.EndHorizontal();
    }

    protected void FieldLabel2() {
        MadGUI.PropertyField(label, "Label");

        using (MadGUI.EnabledIf((target as EnergyBarUGUIBase).label != null)) {
            using (MadGUI.Indent()) {
                MadGUI.PropertyField(labelFormat, "Format");
                formatHelpAnimBool.target = MadGUI.Foldout("Label Format Help", false);
                if (EditorGUILayout.BeginFadeGroup(formatHelpAnimBool.faded)) {
                    EditorGUILayout.HelpBox(FormatHelp, MessageType.None);
                }
                EditorGUILayout.EndFadeGroup();
            }
        }
    }

    protected void EnsureReadable(SerializedProperty spriteObject) {
        if (spriteObject.objectReferenceValue != null) {
            var sprite = spriteObject.objectReferenceValue as Sprite;
            if (!Utils.IsReadable(sprite.texture)) {
                if (EditorUtility.DisplayDialog("Set texture as readable?",
                    string.Format("Texture '{0}' must be set as readable. Do it now?", sprite.name), "OK", "Cancel")) {
                    Utils.SetReadable(sprite.texture);
                } else {
                    spriteObject.objectReferenceValue = null;
                }
            }
        }
    }
}

} // namespace
