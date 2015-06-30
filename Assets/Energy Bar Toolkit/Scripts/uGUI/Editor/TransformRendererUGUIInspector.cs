/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using EnergyBarToolkit;
using UnityEditor;
using UnityEngine;

namespace EnergyBarToolkit {

[CustomEditor(typeof(TransformRendererUGUI))]
public class TransformRendererUGUIInspector : EnergyBarUGUIInspectorBase {

    #region Fields

    private SerializedProperty spriteObject;
    private SerializedProperty spriteObjectPivot;

    private SerializedProperty transformTranslate;
    private SerializedProperty transformRotate;
    private SerializedProperty transformScale;

    private SerializedProperty translateFunction;
    private SerializedProperty rotateFunction;
    private SerializedProperty scaleFunction;

    private SerializedProperty translateFunctionStart;
    private SerializedProperty translateFunctionEnd;
    private SerializedProperty translateAnimation;
    private SerializedProperty rotateFunctionStart;
    private SerializedProperty rotateFunctionEnd;
    private SerializedProperty rotateAnimation;
    private SerializedProperty scaleFunctionStart;
    private SerializedProperty scaleFunctionEnd;
    private SerializedProperty scaleAnimation;

    #endregion

    #region Methods

    public override void OnEnable() {
        base.OnEnable();

        spriteObject = serializedObject.FindProperty("spriteObject");
        spriteObjectPivot = serializedObject.FindProperty("spriteObjectPivot");

        transformTranslate = serializedObject.FindProperty("transformTranslate");
        transformRotate = serializedObject.FindProperty("transformRotate");
        transformScale = serializedObject.FindProperty("transformScale");

        translateFunction = serializedObject.FindProperty("translateFunction");
        rotateFunction = serializedObject.FindProperty("rotateFunction");
        scaleFunction = serializedObject.FindProperty("scaleFunction");

        translateFunctionStart = translateFunction.FindPropertyRelative("startPosition");
        translateFunctionEnd = translateFunction.FindPropertyRelative("endPosition");
        translateAnimation = translateFunction.FindPropertyRelative("animationCurve");

        rotateFunctionStart = rotateFunction.FindPropertyRelative("startAngle");
        rotateFunctionEnd = rotateFunction.FindPropertyRelative("endAngle");
        rotateAnimation = rotateFunction.FindPropertyRelative("animationCurve");

        scaleFunctionStart = scaleFunction.FindPropertyRelative("startScale");
        scaleFunctionEnd = scaleFunction.FindPropertyRelative("endScale");
        scaleAnimation = scaleFunction.FindPropertyRelative("animationCurve");
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();

        SectionTextures();
        SectionAppearance();

        EditorGUILayout.Space();

        SectionObjectTransform();

        EditorGUILayout.Space();

        SectionEffects();
        SectionDebugMode();

        serializedObject.ApplyModifiedProperties();
    }

    private void SectionTextures() {
        GUILayout.Label("Textures", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldBackgroundSprites();

            EditorGUILayout.Space();

            FieldSprite(spriteObject, "Object Sprite", MadGUI.ObjectIsSet);
            MadGUI.PropertyFieldVector2(spriteObjectPivot, "Object Pivot");

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

            GUI.enabled = true;
        }
    }

    private void SectionObjectTransform() {
        GUILayout.Label("Object Transform", "HeaderLabel");

        using (MadGUI.Indent()) {
            MadGUI.PropertyFieldToggleGroup2(transformTranslate, "Translate", () => {
                MadGUI.Indent(() => {
                    MadGUI.PropertyFieldVector2(translateFunctionStart, "Start Point");
                    MadGUI.PropertyFieldVector2(translateFunctionEnd, "End Point");
                    MadGUI.PropertyField(translateAnimation, "Curve");
                });
            });

            MadGUI.PropertyFieldToggleGroup2(transformRotate, "Rotate", () => {
                MadGUI.Indent(() => {
                    MadGUI.PropertyField(rotateFunctionStart, "Start Angle");
                    MadGUI.PropertyField(rotateFunctionEnd, "End Angle");
                    MadGUI.PropertyField(rotateAnimation, "Curve");
                });
            });

            MadGUI.PropertyFieldToggleGroup2(transformScale, "Scale", () => {
                MadGUI.Indent(() => {
                    MadGUI.PropertyFieldVector2(scaleFunctionStart, "Start Scale");
                    MadGUI.PropertyFieldVector2(scaleFunctionEnd, "End Scale");
                    MadGUI.PropertyField(scaleAnimation, "Curve");
                });
            });
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