/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnergyBarToolkit {

[ExecuteInEditMode]
[RequireComponent(typeof(EnergyBar))]
public class SequenceRendererUGUI : EnergyBarUGUIBase {

    #region Public Fields

    public RenderMethod renderMethod = RenderMethod.Grid;

    public SpriteTex gridSprite = new SpriteTex();
    public int gridWidth = 3;
    public int gridHeight = 3;

    public bool frameCountAuto = true;
    public int frameCount;

    public List<SpriteTex> sequenceSprites = new List<SpriteTex>();
    public Color sequenceTint = Color.white;

    #endregion

    #region Private Fields

    [SerializeField]
    private int lastRebuildHash;
    private bool dirty;

    [SerializeField]
    private Image2 barImage;

    #endregion

    #region Public Methods

    public override void SetNativeSize() {
        switch (renderMethod) {
            case RenderMethod.Grid:
                SetNativeSizeGrid();
                break;
            case RenderMethod.Sequence:
                SetNativeSizeSequence();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetNativeSizeGrid() {
        if (gridSprite.sprite == null) {

            // try to create the bar now
            Rebuild();
            if (gridSprite.sprite == null) {
                Debug.LogWarning("Cannot resize bar that has not been created yet");
                return;
            }
        }

        var rect = gridSprite.sprite.rect;
        float w = rect.width / gridWidth;
        float h = rect.height / gridHeight;

        SetSize(rectTransform, w, h);
    }

    private void SetNativeSizeSequence() {
        if (sequenceSprites.Count == 0) {

            // try to create the bar now
            Rebuild();
            if (sequenceSprites.Count == 0) {
                Debug.LogWarning("Cannot resize bar that has not been created yet");
                return;
            }
        }

        var sprite = sequenceSprites[0];
        if (sprite == null || sprite.sprite == null) {
            return;
        }

        var rect = sprite.sprite.rect;
        SetSize(rectTransform, rect.width, rect.height);
    }

    #endregion

    #region Unity Methods

    protected override void Update() {
        base.Update();

        if (RebuildNeeded()) {
            Rebuild();
        }

        UpdateNonIntrusive();
    }

    #endregion

    #region Update Methods

    private void UpdateNonIntrusive() {
        UpdateSize();
        UpdateValue();
//        UpdateBlinkEffect();
//        UpdateBurnEffect();
        UpdateColor();
    }

    private void UpdateSize() {
        var thisRectTransform = GetComponent<RectTransform>();
        for (int i = 0; i < createdChildren.Count; ++i) {
            var child = createdChildren[i];
            var otherRectTransform = child.GetComponent<RectTransform>();

            SetSize(otherRectTransform, thisRectTransform.rect.size);
        }
    }

    private void UpdateValue() {
        switch (renderMethod) {
            case RenderMethod.Grid:
                UpdateValueGrid();
                break;
            case RenderMethod.Sequence:
                UpdateValueSequence();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateValueGrid() {
        float val = ValueF2;
        var frameIndex = GetFrameIndex(val);

        barImage.uvTiling = GetTilling();
        barImage.uvOffset = GetOffset(frameIndex);
        barImage.sprite = gridSprite.sprite;
        barImage.color = gridSprite.color;

        barImage.SetAllDirty();
    }

    private void UpdateColor() {
        switch (renderMethod) {
            case RenderMethod.Grid:
                barImage.color = ComputeColor(gridSprite.color);
                break;
            case RenderMethod.Sequence:
                barImage.color = ComputeColor(sequenceTint);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private Vector2 GetOffset(int frameIndex) {
        var tilling = GetTilling();
        int y = frameIndex / gridWidth;
        int x = frameIndex - (y * gridWidth);

        return new Vector2(tilling.x * x, 1 - tilling.y * y - tilling.y);
    }

    private Vector2 GetTilling() {
        return new Vector2(1f / gridWidth, 1f / gridHeight);
    }

    private void UpdateValueSequence() {
        if (GetFrameCount() == 0) {
            return;
        }

        float val = ValueF2;
        var frameIndex = GetFrameIndex(val);

        var sprite = sequenceSprites[frameIndex];
        barImage.sprite = sprite.sprite;
        barImage.color = sprite.color;

        barImage.uvOffset = Vector2.zero;
        barImage.uvTiling = Vector2.one;

        barImage.SetAllDirty();
    }

    private int GetFrameIndex(float progress) {
        var count = GetFrameCount();
        int frameIndex = Mathf.FloorToInt((count - 1) * progress);
        return frameIndex;
    }

    private int GetFrameCount() {
        switch (renderMethod) {
            case RenderMethod.Grid:
                if (frameCountAuto) {
                    return gridWidth * gridHeight;
                }

                return frameCount;

            case RenderMethod.Sequence:
                return sequenceSprites.Count;

            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    #endregion

    #region Rebuild Methods

    private bool RebuildNeeded() {
        int ch = MadHashCode.FirstPrime;
        ch = MadHashCode.AddList(ch, spritesBackground);
        ch = MadHashCode.AddList(ch, spritesForeground);

        if (ch != lastRebuildHash || dirty) {
            lastRebuildHash = ch;
            dirty = false;
            return true;
        } else {
            return false;
        }
    }

    private void Rebuild() {
        RemoveCreatedChildren();
        BuildBackgroundImages();
        BuildBar();
        BuildForegroundImages();
        UpdateSize();
        MoveLabelToTop();
    }

    private void BuildBar() {
        barImage = CreateChild<Image2>("bar");
    }

    #endregion

    #region Inner and Anonymous Classes

    public enum RenderMethod {
        Grid,
        Sequence,
    }

    #endregion
}

} // namespace