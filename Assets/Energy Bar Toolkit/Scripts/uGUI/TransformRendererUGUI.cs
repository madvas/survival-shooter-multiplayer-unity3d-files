/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System.Collections.Generic;
using EnergyBarToolkit;
using UnityEngine;
using UnityEngine.UI;

namespace EnergyBarToolkit {

[RequireComponent(typeof(EnergyBar))]
[ExecuteInEditMode]
public class TransformRendererUGUI : EnergyBarUGUIBase {

    #region Public Fields

    public SpriteTex spriteObject = new SpriteTex();

    public Vector2 spriteObjectPivot = new Vector2(0.5f, 0.5f);

    public bool transformTranslate;
    public bool transformRotate;
    public bool transformScale;

    public TranslateFunction translateFunction = new TranslateFunction();
    public RotateFunction rotateFunction = new RotateFunction();
    public ScaleFunction scaleFunction = new ScaleFunction();

    #endregion

    #region Private Fields

    // this field is increased when changes done by maintainer requires
    // this bar object to be rebuilt on update
    [SerializeField]
    private const int BuildVersion = 1;

    [SerializeField]
    private int lastRebuildHash;

    [SerializeField]
    private bool dirty;

    [SerializeField]
    private Image imageObject;

    [SerializeField]
    private Transform imageObjectContainer;

    #endregion

    #region Public Methods

    public override void SetNativeSize() {
        Vector2? computeNativeSize = ComputeNativeSize();
        if (computeNativeSize.HasValue) {
            SetSize(rectTransform, computeNativeSize.Value);
        }
    }

    private Vector2? ComputeNativeSize() {
        var sprite = FirstBackgroundOrForegroundSprite();
        if (sprite == null) {
            // try to create the bar now
            Rebuild();
            if (sprite == null) {
                Debug.LogWarning("Cannot resize bar that has not been created yet");
                return null;
            }
        }

        return new Vector2(sprite.rect.width, sprite.rect.height);
    }

    private Sprite FirstBackgroundOrForegroundSprite() {
        for (int i = 0; i < spritesBackground.Count; i++) {
            var spriteTex = spritesBackground[i];
            if (spriteTex != null && spriteTex.sprite != null) {
                return spriteTex.sprite;
            }
        }

        for (int i = 0; i < spritesForeground.Count; i++) {
            var spriteTex = spritesForeground[i];
            if (spriteTex != null && spriteTex.sprite != null) {
                return spriteTex.sprite;
            }
        }

        return null;
    }

    #endregion

    #region Unity Methods

    protected override void Update() {
        base.Update();

        UpdateRebuild();

        if (IsValid()) {
            UpdateColor();
            UpdateAnchor();
            UpdateSize();
            UpdateTransform();
        }
    }

    private void UpdateAnchor() {
        imageObject.rectTransform.pivot = spriteObjectPivot;
    }

    #endregion

    #region Update Methods

    private void UpdateSize() {
        var thisRectTransform = GetComponent<RectTransform>();

        for (int i = 0; i < createdChildren.Count; ++i) {
            var child = createdChildren[i];
            if (child.gameObject == imageObject.gameObject) {
                continue;
            }

            var otherRectTransform = child.GetComponent<RectTransform>();
            if (otherRectTransform != null) {
                SetSize(otherRectTransform, thisRectTransform.rect.size);
            }
        }

        // update container scale
        Vector2 nativeSize;
        if (TryGetNativeSize(out nativeSize)) {
            Vector2 ratio = new Vector2(
                thisRectTransform.rect.width / nativeSize.x,
                thisRectTransform.rect.height / nativeSize.y);
            MadTransform.SetLocalScale(imageObjectContainer.transform, ratio);
        }
    }

    private bool TryGetNativeSize(out Vector2 nativeSize) {
        for (int i = 0; i < createdChildren.Count; ++i) {
            var child = createdChildren[i];
            if (child.gameObject == imageObject.gameObject) {
                continue;
            }

            var image = child.GetComponent<Image>();
            if (image != null) {
                nativeSize = image.sprite.rect.size;
                return true;
            }
        }

        nativeSize = Vector2.zero;
        return false;
    }

    private void UpdateColor() {
        if (imageObject != null) {
            imageObject.color = ComputeColor(spriteObject.color);
        }
    }

    private void UpdateTransform() {
        if (transformTranslate) {
            Vector2 translate = translateFunction.Value(ValueF2);

            Vector2 size;
            if (TryGetNativeSize(out size)) {
                imageObject.transform.localPosition = new Vector2(
                translate.x * size.x,
                translate.y * size.y);
            }
        }

        if (transformRotate) {
            Quaternion rotate = rotateFunction.Value(ValueF2);
            var localRotation = Quaternion.identity * Quaternion.Inverse(rotate);

            // ReSharper disable once RedundantCheckBeforeAssignment
            if (imageObject.transform.localRotation != localRotation) {
                imageObject.transform.localRotation = localRotation;
            }
        }

        if (transformScale) {
            Vector3 scale = scaleFunction.Value(ValueF2);
            MadTransform.SetLocalScale(imageObject.transform, scale);
        }
    }

    #endregion

    #region Rebuild Methods

    private void UpdateRebuild() {
        if (!IsValid()) {
            RemoveCreatedChildren();
        }

        if (RebuildNeeded()) {
            Rebuild();
        }
    }

    private bool IsValid() {
        return spriteObject.sprite != null;
    }

    private bool RebuildNeeded() {
        int ch = MadHashCode.FirstPrime;
        ch = MadHashCode.Add(ch, BuildVersion);
        ch = MadHashCode.Add(ch, spriteObject != null ? spriteObject.GetHashCode() : 0);
        ch = MadHashCode.AddList(ch, spritesBackground);
        ch = MadHashCode.AddList(ch, spritesForeground);
        ch = MadHashCode.Add(ch, spriteObjectPivot);
        ch = MadHashCode.Add(ch, label);
        ch = MadHashCode.Add(ch, effectBurn);
        ch = MadHashCode.Add(ch, effectBurnSprite);
        ch = MadHashCode.Add(ch, rectTransform.pivot);

        if (ch != lastRebuildHash || dirty) {
            lastRebuildHash = ch;
            dirty = false;
            return true;
        } else {
            return false;
        }
    }

    private void Rebuild() {
        if (!IsValid()) {
            return;
        }

        RemoveCreatedChildren();

        BuildBackgroundImages();
        BuildObject();
        BuildForegroundImages();

        UpdateSize();

        MoveLabelToTop();
    }

    private void BuildObject() {
        imageObjectContainer = CreateChild<Transform>("container");

        imageObject = CreateChild<Image>("bar", imageObjectContainer);
        imageObject.sprite = spriteObject.sprite;
        imageObject.SetNativeSize();
    }

    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

} // namespace