/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EnergyBarToolkit {

public abstract class EnergyBarUGUIBase : EnergyBarBase {

    #region Public Fields

    public List<SpriteTex> spritesBackground = new List<SpriteTex>();
    public List<SpriteTex> spritesForeground = new List<SpriteTex>();

    public Text label;

    // burn effect
    public SpriteTex effectBurnSprite;

    public bool debugMode;

    #endregion

    #region Private Fields

    [SerializeField]
    protected List<GameObject> createdChildren = new List<GameObject>(32);

    [SerializeField]
    protected List<SpriteBind> backgroundBinds = new List<SpriteBind>(16);

    [SerializeField]
    protected List<SpriteBind> foregroundBinds = new List<SpriteBind>(16);

    protected RectTransform rectTransform;

    #endregion

    #region Properties

    public override Rect DrawAreaRect {
        get { return new Rect(); }
    }

    #endregion

    #region Public Methods

    public abstract void SetNativeSize();
    
    #endregion

    #region Lifecycle Methods

    protected override void OnEnable() {
        base.OnEnable();
        rectTransform = GetComponent<RectTransform>();
    }

    protected override void Update() {
        base.Update();
        UpdateLabel();
        UpdateBackgroundForegroundColor();

        if (Application.isEditor) {
            UpdateDebugMode(debugMode);
        }
    }

    private void UpdateBackgroundForegroundColor() {
        UpdateColor(backgroundBinds, spritesBackground);
        UpdateColor(foregroundBinds, spritesForeground);
    }

    private void UpdateColor(List<SpriteBind> binds, List<SpriteTex> sprites) {
        for (int i = 0; i < binds.Count; i++) {
            var bind = binds[i];
            var image = bind.image;
            int index = bind.index;
            if (sprites.Count > index) {
                var spriteTex = sprites[index];

                image.color = ComputeColor(spriteTex.color);
            }
        }
    }

    private void UpdateLabel() {
        if (label != null) {
            label.text = LabelFormatResolve(labelFormat);
        }
    }
    
    #endregion

    #region Build Methods

    protected void BuildBackgroundImages() {
        BuildImagesFromList(spritesBackground, "bg_", ref backgroundBinds);
    }

    protected void BuildForegroundImages() {
        BuildImagesFromList(spritesForeground, "fg_", ref foregroundBinds);
    }

    private void BuildImagesFromList(List<SpriteTex> sprites, string prefix, ref List<SpriteBind> store) {
        for (int i = 0; i < sprites.Count; ++i) {
            var spriteTex = sprites[i];
            var sprite = spriteTex.sprite;
            if (sprite == null) {
                continue;
            }

            var image = CreateChild<Image>(prefix + (i + 1));
            image.sprite = sprite;
            image.SetNativeSize();
            image.type = GetImageType();
            image.color = spriteTex.color;

            store.Add(new SpriteBind(image, i));
        }
    }

    protected virtual Image.Type GetImageType() {
        return Image.Type.Simple;
    }

    protected T CreateChild<T>(string childName) where T : Component {
        return CreateChild<T>(childName, transform);
    }

    protected T CreateChild<T>(string childName, Transform parent) where T : Component {
        var child = MadTransform.CreateChild<T>(parent, "generated_" + childName);
        createdChildren.Add(child.gameObject);
        SetAsLastGenerated(child.transform);
        ApplyDebugMode(child.gameObject, debugMode);
        return child;
    }

    private void SetAsLastGenerated(Transform child) {
        var lowestNonGenerated = NonGeneratedLowestIndex();
        if (lowestNonGenerated != -1) {
            child.SetSiblingIndex(lowestNonGenerated);
        } else {
            child.SetAsLastSibling();
        }
    }

    // searches for non generated child and returns it index. Returns -1 if not found
    private int NonGeneratedLowestIndex() {
        int lowestIndex = int.MaxValue;

        for (int i = 0; i < transform.childCount; i++) {
            var child = transform.GetChild(i);
            if (!IsGenerated(child)) {
                lowestIndex = Mathf.Min(child.GetSiblingIndex(), lowestIndex);
            }
        }

        if (lowestIndex == int.MaxValue) {
            return -1;
        }

        return lowestIndex;
    }

    private bool IsGenerated(Transform child) {
        return child.name.StartsWith("generated_"); // cheap method, but will do
    }

    protected void RemoveCreatedChildren() {
        for (int i = 0; i < createdChildren.Count; ++i) {
            MadGameObject.SafeDestroy(createdChildren[i]);
        }

        // scan for generated children that were not removed
        // this is needed when user performs an undo operation
        var existingChildren = MadTransform.FindChildren<Transform>(transform, c => c.name.StartsWith("generated_"), 0);
        for (int i = 0; i < existingChildren.Count; i++) {
            var child = existingChildren[i];
            MadGameObject.SafeDestroy(child.gameObject);
        }

        createdChildren.Clear();
        backgroundBinds.Clear();
        foregroundBinds.Clear();
    }

    protected void SetSize(RectTransform rt, float w, float h) {
        SetSize(rt, new Vector2(w, h));
    }

    protected void SetSize(RectTransform rt, Vector2 size) {
        var rect = rt.rect;

        if (!Mathf.Approximately(rect.width, size.x)) {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        }

        if (!Mathf.Approximately(rect.height, size.y)) {
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
    }

    protected override bool IsVisible() {
        if (!base.IsVisible()) {
            return false;
        }

        if (rectTransform.rect.size.x <= 0 || rectTransform.rect.size.y <= 0) {
            return false;
        }

        return true;
    }

    protected void MoveLabelToTop() {
        if (label == null) {
            return;
        }

        if (label.transform.parent == transform) {
            label.transform.SetAsLastSibling();
        }
    }

    private void UpdateDebugMode(bool value) {
        for (int i = 0; i < createdChildren.Count; i++) {
            var createdChild = createdChildren[i];
            if (createdChild != null) {
                ApplyDebugMode(createdChild.gameObject, value);
            }
        }
    }

    private void ApplyDebugMode(GameObject gameObject, bool enabled) {
        gameObject.hideFlags = enabled ? HideFlags.None : HideFlags.HideInHierarchy;
    }

    #endregion

    #region Inner Types

    [Serializable]
    public class SpriteTex : AbstractTex {
        public Sprite sprite;

        public SpriteTex() {
        }

        public SpriteTex(Sprite sprite) {
            this.sprite = sprite;
        }

        public override int GetHashCode() {
            int ch = MadHashCode.FirstPrime;

            ch = MadHashCode.Add(ch, sprite != null ? sprite.GetInstanceID() : 0);
            ch = MadHashCode.Add(ch, color);

            return ch;
        }
    }

    [Serializable]
    public class SpriteBind {
        public Image image;
        public int index;

        public SpriteBind(Image image, int index) {
            this.image = image;
            this.index = index;
        }
    }

    #endregion

}

} // namespace