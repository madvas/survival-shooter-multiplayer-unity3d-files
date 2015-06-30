/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;
using UnityEngine;

namespace EnergyBarToolkit {

[CustomEditor(typeof(SequenceRendererUGUI))]
public class SequenceRendererUGUIInspector : EnergyBarUGUIInspectorBase {

    #region Fields

    private SerializedProperty renderMethod;
    private SerializedProperty gridSprite;
    private SerializedProperty gridWidth;
    private SerializedProperty gridHeight;

    private SerializedProperty frameCountAuto;
    private SerializedProperty frameCount;

    private SerializedProperty sequenceSprites;
    private SerializedProperty sequenceTint;

    private SequenceRendererUGUI renderer;

    private AnimBool showFrameCount = new AnimBool();
    private ReorderableList sequenceSpritesList;

    #endregion

    #region Unity Methods

    public override void OnEnable() {
        base.OnEnable();

        renderMethod = serializedObject.FindProperty("renderMethod");
        gridSprite = serializedObject.FindProperty("gridSprite");
        gridWidth = serializedObject.FindProperty("gridWidth");
        gridHeight = serializedObject.FindProperty("gridHeight");

        frameCountAuto = serializedObject.FindProperty("frameCountAuto");
        frameCount = serializedObject.FindProperty("frameCount");

        sequenceSprites = serializedObject.FindProperty("sequenceSprites");
        sequenceTint = serializedObject.FindProperty("sequenceTint");

        renderer = (SequenceRendererUGUI) target;

        showFrameCount.value = !frameCountAuto.boolValue;
        showFrameCount.valueChanged.AddListener(Repaint);

        sequenceSpritesList = CreateLayerList(sequenceSprites, "Sequence Sprites");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();

        SectionTextures();
        SectionAppearance();
        SectionEffects();
        SectionDebugMode();

        serializedObject.ApplyModifiedProperties();
    }

    #endregion

    #region Private Methods

    private void SectionTextures() {
        GUILayout.Label("Textures", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldBackgroundSprites();

            EditorGUILayout.Space();

            MadGUI.PropertyFieldEnumPopup(renderMethod, "Render Method");
            switch (renderer.renderMethod) {
                case SequenceRendererUGUI.RenderMethod.Grid:
                    FieldSprite(gridSprite, "Sprite", MadGUI.ObjectIsSet);
                    using (MadGUI.Indent()) {
                        MadGUI.PropertyField(gridWidth, "Grid Width");
                        MadGUI.PropertyField(gridHeight, "Grid Height");
                        MadGUI.PropertyField(frameCountAuto, "Frame Count Auto");
                        showFrameCount.target = !frameCountAuto.boolValue;

                        if (EditorGUILayout.BeginFadeGroup(showFrameCount.faded)) {
                            if (renderer.frameCountAuto) {
                                frameCount.intValue = gridWidth.intValue * gridHeight.intValue;
                            }

                            MadGUI.PropertyField(frameCount, "Frame Count");
                        }
                        EditorGUILayout.EndFadeGroup();
                    }
                    break;
                case SequenceRendererUGUI.RenderMethod.Sequence:
                    sequenceSpritesList.DoLayoutList();
                    MadGUI.PropertyField(sequenceTint, "Tint");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUILayout.Space();

            //serializedObject.UpdateIfDirtyOrScript();
           // MadGUI.PropertyField(spriteBar, "Bar");
            //serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();

            FieldForegroundSprites();
        }
    }

    private void SectionAppearance() {
        GUILayout.Label("Appearance", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldSetNativeSize();
            EditorGUILayout.Space();

            FieldLabel2();
        }
    }

    private void SectionEffects() {
        GUILayout.Label("Effects", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldSmoothEffect();
        }
    }

    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

} // namespace