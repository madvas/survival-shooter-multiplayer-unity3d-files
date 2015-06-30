/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System.Collections.Generic;
using UnityEngine;

namespace EnergyBarToolkit {

[SelectionBase]
[ExecuteInEditMode]
[RequireComponent(typeof(EnergyBar))]
public class RepeatedRendererUGUI : EnergyBarUGUIBase {

    #region Public Fields

    public SpriteTex spriteIcon = new SpriteTex();
    public SpriteTex spriteSlot = new SpriteTex();

    public int repeatCount = 5;
    public Vector2 repeatPositionDelta = new Vector2(32, 0);

    public GrowType growType;
    public GrowDirection growDirection;

    #endregion

    #region Private Fields

    [SerializeField]
    private int lastRebuildHash;

    private bool dirty;

    [SerializeField]
    private List<Image2> slotImages = new List<Image2>(32);

    [SerializeField]
    private List<Image2> iconImages = new List<Image2>(32);

    [SerializeField]
    private List<Vector2> originPositions = new List<Vector2>(32);
        
    [SerializeField]
    private Vector2 sizeOrigin;

    [SerializeField]
    private Vector2 scaleRatio = Vector2.one;

    #endregion

    #region Properties

    private Color IconTintTransparent {
        get { return new Color(spriteIcon.color.r, spriteIcon.color.g, spriteIcon.color.g, 0); }
    }

    #endregion

    #region Public Methods

    public override void SetNativeSize() {
        if (spriteIcon == null) {
            // try to create the bar now
            Rebuild();
            if (spriteIcon == null) {
                Debug.LogWarning("Cannot resize bar that has not been created yet");
                return;
            }
        }

        var nativeSize = ComputeNativeSize();
        SetSize(rectTransform, nativeSize);
    }

    private Vector2 ComputeNativeSize() {
        if (spriteIcon.sprite == null) {
            return Vector2.zero;
        }

        int iw = Mathf.RoundToInt(spriteIcon.sprite.rect.width);
        int ih = Mathf.RoundToInt(spriteIcon.sprite.rect.height);

        int w = (int) (iw + Mathf.Abs(repeatPositionDelta.x * (repeatCount - 1)));
        int h = (int) (ih + Mathf.Abs(repeatPositionDelta.y * (repeatCount - 1)));

        return new Vector2(w, h);
    }

    /// <summary>
    /// Gets the generated icon image. Note that if you want to modify the image, you have to do it
    /// after this component Update() function. To do this, please adjust script execution order:
    /// http://docs.unity3d.com/Manual/class-ScriptExecution.html
    /// </summary>
    /// <param name="index">Icon index</param>
    /// <returns>Icon image.</returns>
    public Image2 GetIconImage(int index) {
        return iconImages[index];
    }

    /// <summary>
    /// Gets the generated slot image. Note that if you want to modify the image, you have to do it
    /// after this component Update() function. To do this, please adjust script execution order:
    /// http://docs.unity3d.com/Manual/class-ScriptExecution.html
    /// </summary>
    /// <param name="index">Slot index</param>
    /// <returns>Slot image.</returns>
    public Image2 GetSlotImage(int index) {
        return slotImages[index];
    }

    /// <summary>
    /// Gets total icon count.
    /// </summary>
    /// <returns>Total icon count.</returns>
    public int GetIconCount() {
        return repeatCount;
    }

    /// <summary>
    /// Gets the number of icons painted at full visibility (full value).
    /// </summary>
    /// <returns>Full visiblity icon count.</returns>
    public int GetFullyVisibleIconCount() {
        float displayIconCountF = ValueF2 * repeatCount;
        return (int)Mathf.Floor(displayIconCountF);     // icons painted at full visibility
    }

    /// <summary>
    /// Gets the number of icons painted including the last one that can be not fully visible.
    /// </summary>
    /// <returns>Get the number of painted icons.</returns>
    public int GetVisibleIconCount() {
        float displayIconCountF = ValueF2 * repeatCount;
        return (int)Mathf.Ceil(displayIconCountF);
    }

    #endregion

    #region Unity Methods

    protected override void Update() {
        base.Update();

        UpdateRebuild();
        UpdateBar();
        UpdateVisible();
        UpdateSize();
    }

    void OnValidate() {
        repeatCount = Mathf.Max(1, repeatCount);
    }

    #endregion

    #region Update Methods

    void UpdateBar() {
        if (iconImages.Count == 0) {
            return;
        }

        float displayIconCountF = ValueF2 * repeatCount;
        int visibleIconCount = (int)Mathf.Floor(displayIconCountF);     // icons painted at full visibility
        float middleIconValue = displayIconCountF - visibleIconCount;

        for (int i = 0; i < repeatCount; ++i) {
            var icon = iconImages[i];

            if (slotImages.Count > 0) {
                var slot = slotImages[i];
                UpdateSlot(slot);
            }

            if (i < visibleIconCount) {
                // this is visible sprite
                SetIconVisible(icon);
            } else if (i > visibleIconCount) {
                // this is invisible sprite
                SetIconInvisible(icon);
            } else {
                // this is partly-visible sprite
                switch (growType) {
                    case GrowType.None:
                        if (Mathf.Approximately(middleIconValue, 0)) {
                            SetIconInvisible(icon);
                        } else {
                            SetIconVisible(icon);
                        }
                        break;
                    case GrowType.Fade:
                        SetIconVisible(icon);
                        icon.color = Color.Lerp(IconTintTransparent, spriteIcon.color, middleIconValue);
                        break;
                    case GrowType.Grow:
                        SetIconVisible(icon, middleIconValue);
                        break;
                    case GrowType.Fill:
                        SetIconVisible(icon);
                        icon.growDirection = growDirection;
                        icon.fillValue = middleIconValue;
                        break;

                    default:
                        Debug.Log("Unknown grow type: " + growType);
                        break;
                }
            }

        }
    }

    void UpdateSize() {
        if (spriteIcon.sprite == null) {
            return;
        }

        var nativeSize = ComputeNativeSize();
        var currentSize = rectTransform.rect.size;
        scaleRatio = new Vector2(currentSize.x / nativeSize.x, currentSize.y / nativeSize.y);

        var spriteRect = spriteIcon.sprite.rect;

        for (int i = 0; i < iconImages.Count; i++) {
            var originPosition = originPositions[i];
            var iconImage = iconImages[i];

            UpdateSpriteSize(originPosition, iconImage, spriteRect);
        }

        for (int i = 0; i < slotImages.Count; i++) {
            var originPosition = originPositions[i];
            var slotImage = slotImages[i];

            UpdateSpriteSize(originPosition, slotImage, spriteRect);
        }
    }

    private void UpdateSpriteSize(Vector2 originPosition, Image2 image, Rect spriteRect) {
        float posX = originPosition.x * scaleRatio.x;
        float posY = originPosition.y * scaleRatio.y;

        MadTransform.SetLocalPosition(image.rectTransform, new Vector3(posX, posY));
        SetSize(image.rectTransform, new Vector2(spriteRect.width * scaleRatio.x, spriteRect.height * scaleRatio.y));
    }

    void SetIconVisible(Image2 image, float scale = 1) {
        image.color = ComputeColor(spriteIcon.color);
        image.fillValue = 1;
        MadTransform.SetLocalScale(image.transform, new Vector3(scale, scale, scale));
        image.enabled = true;
    }

    void UpdateSlot(Image2 image) {
        if (image != null) {
            image.color = ComputeColor(spriteSlot.color);
        }
    }

    void SetIconInvisible(Image2 image) {
        image.enabled = false;
    }

    void UpdateVisible() {
        bool visible = IsVisible();

        if (!visible) {
            // make all sprites invisible
            // no need to make the oposite, sprites are visible by default
            for (int i = 0; i < iconImages.Count; ++i) {
                var iconImage = iconImages[i];
                iconImage.enabled = false;
            }

            for (int i = 0; i < slotImages.Count; i++) {
                var slotImage = slotImages[i];
                slotImage.enabled = false;
            }
        }
    }

    #endregion


    #region Rebuild Methods

    private void UpdateRebuild() {
        if (RebuildNeeded()) {
            Rebuild();
        }
    }

    private bool RebuildNeeded() {
        if (iconImages.Count == 0 && spriteIcon != null && repeatCount != 0) {
            return true;
        }

        if (iconImages.Count > 0 && iconImages[0] == null) {
            // this can happen when user executes a undo operation
            return true;
        }

        int ch = MadHashCode.FirstPrime;
        ch = MadHashCode.Add(ch, spriteIcon);
        ch = MadHashCode.Add(ch, spriteSlot);
        ch = MadHashCode.Add(ch, repeatCount);
        ch = MadHashCode.Add(ch, repeatPositionDelta);
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
        RemoveCreatedChildren();

        iconImages.Clear();
        slotImages.Clear();
        originPositions.Clear();

        BuildIconsAndSlots();
        MoveLabelToTop();
    }

    private void BuildIconsAndSlots() {
        Vector2 min = Vector2.zero;
        Vector2 max = Vector2.zero;
        bool hasMinMax = false;

        for (int i = 0; i < repeatCount; ++i) {
            // slot
            if (spriteSlot.sprite != null) {
                var slot = CreateChild<Image2>(string.Format("slot_{0:D2}", i + 1));
                slot.sprite = spriteSlot.sprite;
                slot.color = spriteSlot.color;

                slot.transform.localPosition = repeatPositionDelta * i;
                //slot.transform.localPosition += LocalIconOffset

                slotImages.Add(slot);

                Expand(ref min, ref max, ref hasMinMax, slot.rectTransform);
            }

            // icon
            if (spriteIcon.sprite != null) {
                var icon = CreateChild<Image2>(string.Format("icon_{0:D2}", i + 1));
                icon.sprite = spriteIcon.sprite;
                icon.color = spriteIcon.color;

                icon.transform.localPosition = repeatPositionDelta * i;

                iconImages.Add(icon);

                Expand(ref min, ref max, ref hasMinMax, icon.rectTransform);
            }
        }

        // set the size
        sizeOrigin = ComputeNativeSize();
        SetNativeSize();

        // compute offset
        Vector2 offset = new Vector2(
            0.5f * sizeOrigin.x - (rectTransform.pivot.x * sizeOrigin.x),
            0.5f * sizeOrigin.y - (rectTransform.pivot.y * sizeOrigin.y));
        Vector2 startPos = -repeatPositionDelta * (repeatCount - 1) * 0.5f;

        // reposition again
        for (int i = 0; i < slotImages.Count; i++) {
            var slotImage = slotImages[i];
            slotImage.rectTransform.localPosition = repeatPositionDelta * i + (startPos + offset);
        }

        for (int i = 0; i < iconImages.Count; i++) {
            var iconImage = iconImages[i];
            iconImage.rectTransform.localPosition = repeatPositionDelta * i + (startPos + offset);
            originPositions.Add(iconImage.rectTransform.localPosition);
        }

        if (scaleRatio != Vector2.one) {
            var targetSize = rectTransform.rect.size;
            targetSize.x *= scaleRatio.x;
            targetSize.y *= scaleRatio.y;

            SetSize(rectTransform, targetSize);
            UpdateSize();
        }
    }

    private void Expand(ref Vector2 min, ref Vector2 max, ref bool hasMinMax, RectTransform tr) {
        var rect = tr.rect;
        float xMin = rect.xMin + tr.localPosition.x;
        float xMax = rect.xMax + tr.localPosition.x;
        float yMin = rect.yMin + tr.localPosition.y;
        float yMax = rect.yMax + tr.localPosition.y;

        if (!hasMinMax) {
            min.x = xMin;
            min.y = yMin;
            max.x = xMax;
            max.y = yMax;
            hasMinMax = true;
        } else {
            if (xMin < min.x) {
                min.x = xMin;
            }

            if (yMin < min.y) {
                min.y = yMin;
            }

            if (xMax > max.x) {
                max.x = xMax;
            }

            if (yMax > max.y) {
                max.y = yMax;
            }
        }
    }

    #endregion

    #region Inner Types

    public enum GrowType {
        None,
        Grow,
        Fade,
        Fill
    }

    #endregion

}

} // namespace