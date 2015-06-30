/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using System.Collections;

namespace EnergyBarToolkit {

[RequireComponent(typeof(RectTransform))]
[CustomEditor(typeof(FilledRendererUGUI))]
public class FillerRendererUGUIInspector : EnergyBarUGUIInspectorBase {

    private SerializedProperty spriteBar;

    private SerializedProperty spriteBarColorType;
    private SerializedProperty spriteBarColor;
    private SerializedProperty spriteBarGradient;
    private SerializedProperty growDirection;

    private SerializedProperty radialOffset;
    private SerializedProperty radialLength;

    private SerializedProperty barImageScale;
    private SerializedProperty barImageOffset;

    // TODO: This can be moved to base class
    private SerializedProperty effectBurn;
    private SerializedProperty effectBurnSprite;
    private SerializedProperty effectBurnDirection;

    private SerializedProperty effectBlink;
    private SerializedProperty effectBlinkValue;
    private SerializedProperty effectBlinkRatePerSecond;
    private SerializedProperty effectBlinkColor;
    private SerializedProperty effectBlinkOperator;

    private SerializedProperty effectTiled;
    private SerializedProperty effectTiledSprite;
    private SerializedProperty effectTiledTiling;
    private SerializedProperty effectTiledStartOffset;
    private SerializedProperty effectTiledOffsetChangeSpeed;
    private SerializedProperty effectTiledTint;

    private SerializedProperty effectFollow;
    private SerializedProperty effectFollowObject;
    private SerializedProperty effectFollowOffset;
    private SerializedProperty effectFollowScaleX;
    private SerializedProperty effectFollowScaleY;
    private SerializedProperty effectFollowScaleZ;
    private SerializedProperty effectFollowRotation;
    private SerializedProperty effectFollowColor;

    private readonly AnimBool tiledEffectAnimBool = new AnimBool();
    private readonly AnimBool blinkEffectAnimBool = new AnimBool();
    private readonly AnimBool burnEffectAnimBool = new AnimBool();
    private readonly AnimBool followEffectAnimBool = new AnimBool();

    private FilledRendererUGUI renderer;

    public override void OnEnable() {
        base.OnEnable();

        spriteBar = serializedObject.FindProperty("spriteBar");

        spriteBarColorType = serializedObject.FindProperty("spriteBarColorType");
        spriteBarColor = serializedObject.FindProperty("spriteBarColor");
        spriteBarGradient = serializedObject.FindProperty("spriteBarGradient");
        growDirection = serializedObject.FindProperty("growDirection");

        radialOffset = serializedObject.FindProperty("radialOffset");
        radialLength = serializedObject.FindProperty("radialLength");

        barImageScale = serializedObject.FindProperty("barImageScale");
        barImageOffset = serializedObject.FindProperty("barImageOffset");

        effectBurn = serializedObject.FindProperty("effectBurn");
        effectBurnSprite = serializedObject.FindProperty("effectBurnSprite");
        effectBurnDirection = serializedObject.FindProperty("effectBurnDirection");

        effectBlink = serializedObject.FindProperty("effectBlink");
        effectBlinkValue = serializedObject.FindProperty("effectBlinkValue");
        effectBlinkRatePerSecond = serializedObject.FindProperty("effectBlinkRatePerSecond");
        effectBlinkColor = serializedObject.FindProperty("effectBlinkColor");
        effectBlinkOperator = serializedObject.FindProperty("effectBlinkOperator");

        effectTiled = serializedObject.FindProperty("effectTiled");
        effectTiledSprite = serializedObject.FindProperty("effectTiledSprite");
        effectTiledTiling = serializedObject.FindProperty("effectTiledTiling");
        effectTiledStartOffset = serializedObject.FindProperty("effectTiledStartOffset");
        effectTiledOffsetChangeSpeed = serializedObject.FindProperty("effectTiledOffsetChangeSpeed");
        effectTiledTint = serializedObject.FindProperty("effectTiledTint");

        effectFollow = serializedObject.FindProperty("effectFollow");
        effectFollowObject = serializedObject.FindProperty("effectFollowObject");
        effectFollowOffset = serializedObject.FindProperty("effectFollowOffset");
        effectFollowScaleX = serializedObject.FindProperty("effectFollowScaleX");
        effectFollowScaleY = serializedObject.FindProperty("effectFollowScaleY");
        effectFollowScaleZ = serializedObject.FindProperty("effectFollowScaleZ");
        effectFollowRotation = serializedObject.FindProperty("effectFollowRotation");
        effectFollowColor = serializedObject.FindProperty("effectFollowColor");

        tiledEffectAnimBool.valueChanged.AddListener(Repaint);
        blinkEffectAnimBool.valueChanged.AddListener(Repaint);
        burnEffectAnimBool.valueChanged.AddListener(Repaint);
        followEffectAnimBool.valueChanged.AddListener(Repaint);

        tiledEffectAnimBool.value = effectTiled.boolValue;
        blinkEffectAnimBool.value = effectBlink.boolValue;
        burnEffectAnimBool.value = effectBurn.boolValue;
        followEffectAnimBool.value = effectFollow.boolValue;

        renderer = (FilledRendererUGUI) target;
    }

    public override void OnInspectorGUI() {
        serializedObject.UpdateIfDirtyOrScript();

        EditorGUI.BeginChangeCheck();

        SectionTextures();
        SectionAppearance();

        EditorGUILayout.Space();

        SectionEffects();
        SectionDebugMode();

        if (EditorGUI.EndChangeCheck()) {
            Undo.RegisterFullObjectHierarchyUndo(renderer.gameObject, "Changed Bar");
        }

        if (serializedObject.ApplyModifiedProperties()) {
            renderer.UpdateRebuild();
        }
    }

    private void SectionEffects() {
        GUILayout.Label("Effects", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldSmoothEffect();

            burnEffectAnimBool.target = effectBurn.boolValue;
            MadGUI.PropertyField(effectBurn, "Burn");
            if (EditorGUILayout.BeginFadeGroup(burnEffectAnimBool.faded)) {
                MadGUI.Indent(() => {
                    FieldSprite(effectBurnSprite, "Bar Texture");
                    MadGUI.PropertyFieldEnumPopup(effectBurnDirection, "Direction");
                    if (effectBurnDirection.enumValueIndex != (int) EnergyBarBase.BurnDirection.OnlyWhenDecreasing &&
                        !effectSmoothChange.boolValue) {
                        if (
                            MadGUI.WarningFix(
                                "Burning when increasing will be visible only if the Smooth effect is enabled.",
                                "Enable Smooth Effect")) {
                            effectSmoothChange.boolValue = true;
                        }
                    }
                    MadGUI.PropertyField(effectSmoothChangeSpeed, "Speed");
                });
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();

            blinkEffectAnimBool.target = effectBlink.boolValue;
            MadGUI.PropertyField(effectBlink, "Blink");
            if (EditorGUILayout.BeginFadeGroup(blinkEffectAnimBool.faded)) {
                MadGUI.Indent(() => {
                    MadGUI.PropertyFieldEnumPopup(effectBlinkOperator, "Operator");
                    MadGUI.PropertyField(effectBlinkValue, "Value");
                    MadGUI.PropertyField(effectBlinkRatePerSecond, "Rate (per second)");
                    MadGUI.PropertyField(effectBlinkColor, "Color");
                });
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();

            tiledEffectAnimBool.target = effectTiled.boolValue;
            MadGUI.PropertyField(effectTiled, "Tiled");
            if (EditorGUILayout.BeginFadeGroup(tiledEffectAnimBool.faded)) {
                MadGUI.Indent(() => {
                    EditorGUILayout.BeginHorizontal();
                    MadGUI.PropertyField(effectTiledSprite, "Sprite", MadGUI.ObjectIsSet);
                    EditorGUILayout.PropertyField(effectTiledTint, new GUIContent(""), GUILayout.Width(90));
                    EditorGUILayout.EndHorizontal();

                    EnsureSpriteRepeated(effectTiledSprite);

                    MadGUI.PropertyFieldVector2(effectTiledTiling, "Tiling");
                    MadGUI.PropertyFieldVector2(effectTiledStartOffset, "Start Offset");
                    MadGUI.PropertyFieldVector2(effectTiledOffsetChangeSpeed, "Speed");
                });
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFadeGroup();

            followEffectAnimBool.target = effectFollow.boolValue;
            MadGUI.PropertyField(effectFollow, "Follow");
            if (EditorGUILayout.BeginFadeGroup(followEffectAnimBool.faded)) {
                using (MadGUI.Indent()) {
                    if (!GrowDirectionSupportedByFollowEffect()) {
                        MadGUI.Error("This effect cannot be used with selected grow direction. "
                                     + "Please read manual for more information.");
                    }

                    MadGUI.PropertyField(effectFollowObject, "GameObject");
                    MadGUI.PropertyField(effectFollowOffset, "Position Offset");

                    EditorGUILayout.Space();

                    MadGUI.PropertyField(effectFollowColor, "Color");
                    MadGUI.PropertyField(effectFollowRotation, "Rotation");
                    if (MadGUI.Foldout("Scale", false)) {
                        MadGUI.Indent(() => {
                            MadGUI.PropertyField(effectFollowScaleX, "X");
                            MadGUI.PropertyField(effectFollowScaleY, "Y");
                            MadGUI.PropertyField(effectFollowScaleZ, "Z");
                        });
                    }
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndFadeGroup();
        }
    }

    private bool GrowDirectionSupportedByFollowEffect() {
        EnergyBarBase.GrowDirection gd = (EnergyBarBase.GrowDirection)growDirection.enumValueIndex;
        return renderer.GrowDirectionSupportedByFollowEffect(gd);
    }

    private void EnsureSpriteRepeated(SerializedProperty spriteSP) {
        var sprite = (Sprite) spriteSP.objectReferenceValue;
        if (sprite == null) {
            return;
        }

        if (sprite.packed) {
            MadGUI.Error("This sprite shouldn't be packed! Please select it and remove the Packing Tag.");
            return;
        }

        if (sprite.rect.width != sprite.texture.width || sprite.rect.height != sprite.texture.height) {
            MadGUI.Error("This sprite shouldn't be packed! Please use the single texture instead.");
            return;
        }

        var texture = sprite.texture;

        var assetPath = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer.wrapMode != TextureWrapMode.Repeat) {
            if (MadGUI.ErrorFix("Sprite wrap mode must be set to Repeat", "Fix Now")) {
                importer.textureType = TextureImporterType.Advanced;
                importer.wrapMode = TextureWrapMode.Repeat;
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }
        }

    }

    private void SectionTextures() {
        GUILayout.Label("Textures", "HeaderLabel");
        using (MadGUI.Indent()) {
            FieldBackgroundSprites();

            EditorGUILayout.Space();

            MadGUI.PropertyField(spriteBar, "Bar", MadGUI.ObjectIsSet);

#if !UNITY_5
            EnsureReadable(spriteBar);
#endif

            EditorGUILayout.Space();

            FieldForegroundSprites();
        }
    }

    private void SectionAppearance() {
        GUILayout.Label("Appearance", "HeaderLabel");

        using (MadGUI.Indent()) {
            FieldSetNativeSize();
            EditorGUILayout.Space();

            var dir = (EnergyBarBase.GrowDirection)growDirection.enumValueIndex;
            if (dir == EnergyBarBase.GrowDirection.ColorChange) {
                GUI.enabled = false;
            }
            MadGUI.PropertyField(spriteBarColorType, "Bar Color Type");
            EditorGUI.indentLevel++;
            switch ((EnergyBarBase.ColorType)spriteBarColorType.enumValueIndex) {
                case EnergyBarBase.ColorType.Solid:
                    MadGUI.PropertyField(spriteBarColor, "Bar Color");
                    break;

                case EnergyBarBase.ColorType.Gradient:
                    MadGUI.PropertyField(spriteBarGradient, "Bar Gradient");
                    break;
            }

            EditorGUI.indentLevel--;

            GUI.enabled = true;

            MadGUI.PropertyFieldEnumPopup(growDirection, "Grow Direction");

            using (MadGUI.Indent()) {
                if (dir == EnergyBarRenderer.GrowDirection.RadialCW || dir == EnergyBarRenderer.GrowDirection.RadialCCW) {
                    MadGUI.Indent(() => {
                        EditorGUILayout.Slider(radialOffset, -1, 1, "Offset");
                        EditorGUILayout.Slider(radialLength, 0, 1, "Length");
                    });
                } else if (dir == EnergyBarRenderer.GrowDirection.ColorChange) {
                    EditorGUILayout.PropertyField(spriteBarGradient, new GUIContent("Bar Gradient"));
                }
            }

            EditorGUILayout.Space();

            MadGUI.PropertyField(barImageScale, "Bar Scale");
            MadGUI.PropertyField(barImageOffset, "Bar Offset");

            EditorGUILayout.Space();

            FieldLabel2();

            GUI.enabled = true;
        }
    }
}

} // namespace