/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnergyBarToolkit {

public class Image2 : MaskableGraphic {

    public Sprite sprite;
    [SerializeField] private Sprite cachedSprite;
    [SerializeField] private Vector4 cachedPadding;

    // shared padding cache
    private static Dictionary<int, Vector4> cachedPaddings = new Dictionary<int, Vector4>();

    public EnergyBarBase.GrowDirection growDirection;

    public float fillValue {
        get { return _fillValue; }
        set { _fillValue = value; SetVerticesDirty(); }
    }

    [SerializeField] private float _fillValue = 1;

    public float radialFillOffset;
    public float radialFillLength = 1;

    public Vector2 uvOffset = Vector2.zero;
    public Vector2 uvTiling = Vector2.one;

    // for unity free
    public bool readable = false;

    public override Texture mainTexture {
        get {
            if (sprite == null) {
                return s_WhiteTexture;
            }

            return sprite.texture;
        }
    }

    protected override void OnFillVBO(List<UIVertex> vbo) {
        if (growDirection != EnergyBarBase.GrowDirection.RadialCW &&
            growDirection != EnergyBarBase.GrowDirection.RadialCCW) {
            FillRegular(vbo);
        } else {
            FillQuad(vbo);
        }
        
    }

    private void FillRegular(List<UIVertex> vbo) {
        if (sprite == null) {
            return;
        }

        var drawingDimensions = GetDrawingDimensions(false);

        float left = drawingDimensions.x;
        float right = drawingDimensions.z;
        float bottom = drawingDimensions.y;
        float top = drawingDimensions.w;

//        if (uvOffset != Vector2.zero) {
//            leftF += uvOffset.x;
//            rightF += uvOffset.x;
//            bottomF += uvOffset.y;
//            topF += uvOffset.y;
//        }
//
//        if (uvScale != Vector2.one) {
//            rightF = leftF + (rightF - leftF) * uvScale.x;
//            topF = bottomF + (topF - bottomF) * uvScale.y;
//        }

        Vector2 corner1 = Vector2.zero;
        Vector2 corner2 = Vector2.zero;

        if (growDirection == EnergyBarBase.GrowDirection.ExpandHorizontal) {
            corner1.x = Mathf.Lerp(0.5f, 0f, fillValue);
            corner2.x = Mathf.Lerp(0.5f, 1f, fillValue);
        } else {

            if (growDirection == EnergyBarBase.GrowDirection.RightToLeft) {
                corner1.x = Mathf.Lerp(1f, 0f, fillValue);
            } else {
                corner1.x = 0f;
            }

            if (growDirection == EnergyBarBase.GrowDirection.LeftToRight) {
                corner2.x = Mathf.Lerp(0f, 1f, fillValue);
            } else {
                corner2.x = 1f;
            }
        }

        if (growDirection == EnergyBarBase.GrowDirection.ExpandVertical) {
            corner2.y = Mathf.Lerp(0.5f, 1f, fillValue);
            corner1.y = Mathf.Lerp(0.5f, 0f, fillValue);
        } else {

            if (growDirection == EnergyBarBase.GrowDirection.BottomToTop) {
                corner2.y = Mathf.Lerp(0f, 1f, fillValue);
            } else {
                corner2.y = 1f;
            }

            if (growDirection == EnergyBarBase.GrowDirection.TopToBottom) {
                corner1.y = Mathf.Lerp(1f, 0f, fillValue);
            } else {
                corner1.y = 0f;
            }
        }

        Vector4 outerUv = DataUtility.GetOuterUV(sprite);
        outerUv = FixUV(outerUv);
//        Debug.Log(outerUv);

//        Debug.Log("sprite.rect = " + sprite.rect);
//        Debug.Log("sprite.textureRect = " + sprite.textureRect);

//        Vector2 corner1uv = new Vector2(sprite.rect.min.x / sprite.textureRect.width, sprite.rect.min.y / sprite.textureRect.height);
//        Vector2 corner2uv = new Vector2(sprite.rect.max.x / sprite.textureRect.width, sprite.rect.max.y / sprite.textureRect.height);
        float uvw = outerUv.z - outerUv.x;
        float uvh = outerUv.w - outerUv.y;
        Vector2 corner1uv = new Vector2(outerUv.x + uvw * corner1.x, outerUv.y + uvh * corner1.y);
        Vector2 corner2uv = new Vector2(outerUv.z - uvw * (1 - corner2.x), outerUv.w - uvh * (1 - corner2.y));

        if (uvOffset != Vector2.zero) {
            corner1uv.x += uvOffset.x;
            corner2uv.x += uvOffset.x;
            corner1uv.y += uvOffset.y;
            corner2uv.y += uvOffset.y;
        }

        if (uvTiling != Vector2.one) {
            corner2uv.x = corner1uv.x + (corner2uv.x - corner1uv.x) * uvTiling.x;
            corner2uv.y = corner1uv.y + (corner2uv.y - corner1uv.y) * uvTiling.y;
        }

//        Debug.Log("top: " + top);
//        Debug.Log("bottom: " + bottom);

        corner1.x *= right - left;
        corner1.y *= top - bottom;
        corner2.x *= right - left;
        corner2.y *= top - bottom;

        corner1.x += left;
        corner1.y += bottom;
        corner2.x += left;
        corner2.y += bottom;

        vbo.Clear();

        UIVertex vert = UIVertex.simpleVert;

        vert.position = new Vector2(corner1.x, corner1.y);
        vert.uv0 = new Vector2(corner1uv.x, corner1uv.y);
        vert.color = color;
        vbo.Add(vert);

        vert.position = new Vector2(corner1.x, corner2.y);
        vert.uv0 = new Vector2(corner1uv.x, corner2uv.y);
        vert.color = color;
        vbo.Add(vert);

        vert.position = new Vector2(corner2.x, corner2.y);
        vert.uv0 = new Vector2(corner2uv.x, corner2uv.y);
        vert.color = color;
        vbo.Add(vert);

        vert.position = new Vector2(corner2.x, corner1.y);
        vert.uv0 = new Vector2(corner2uv.x, corner1uv.y);
        vert.color = color;
        vbo.Add(vert);
    }

    private Vector4 FixUV(Vector4 outerUv) {
        if (Application.HasProLicense() || Application.unityVersion.StartsWith("5.")) {
            return outerUv;
        }

        if (sprite == null) {
            return outerUv;
        }

        var padding = GetCachedPadding();

        var w = sprite.rect.width;
        var h = sprite.rect.height;

        outerUv.x += padding.x / w;
        outerUv.y += padding.y / h;
        outerUv.z -= padding.z / w;
        outerUv.w -= padding.w / h;

        return outerUv;
    }

    private void FillQuad(List<UIVertex> vbo) {
        bool invert = growDirection == EnergyBarBase.GrowDirection.RadialCCW;

        var topLeftQuad = new Quad(invert);
        var topRightQuad = new Quad(invert);
        var bottomRightQuad = new Quad(invert);
        var bottomLeftQuad = new Quad(invert);

        topLeftQuad.anchor = Quad.Point.BottomRight;
        topRightQuad.anchor = Quad.Point.BottomLeft;
        bottomRightQuad.anchor = Quad.Point.TopLeft;
        bottomLeftQuad.anchor = Quad.Point.TopRight;

        var topLeftQuad2 = new Quad(topLeftQuad);
        var topRightQuad2 = new Quad(topRightQuad);
        var bottomRightQuad2 = new Quad(bottomRightQuad);
        var bottomLeftQuad2 = new Quad(bottomLeftQuad);

        // creating 8 quads instead of 8 because when using offset it may display one additional quad
        // and the simplest way is to create 8 quads and wrap around
        Quad[] ordered = new Quad[8];

        if (!invert) {
            ordered[0] = topRightQuad;
            ordered[1] = bottomRightQuad;
            ordered[2] = bottomLeftQuad;
            ordered[3] = topLeftQuad;
            ordered[4] = topRightQuad2;
            ordered[5] = bottomRightQuad2;
            ordered[6] = bottomLeftQuad2;
            ordered[7] = topLeftQuad2;
        } else {
            ordered[7] = topRightQuad2;
            ordered[6] = bottomRightQuad2;
            ordered[5] = bottomLeftQuad2;
            ordered[4] = topLeftQuad2;
            ordered[3] = topRightQuad;
            ordered[2] = bottomRightQuad;
            ordered[1] = bottomLeftQuad;
            ordered[0] = topLeftQuad;
        }

        float rOffset = radialFillOffset % 1;
        if (rOffset < 0) {
            rOffset += 1;
        }

        float fillValue = Mathf.Clamp01(this.fillValue) * radialFillLength;
        float fillStart = rOffset * 4;
        float fillEnd = (rOffset + fillValue) * 4;

        for (int i = Mathf.FloorToInt(fillStart); i < Mathf.CeilToInt(fillEnd); ++i) {
            Quad quad = ordered[i % 8];

            if (i < fillStart) {
                quad.offset = fillStart - i;
            } else {
                quad.offset = 0;
            }

            if (fillEnd > i + 1) {
                quad.progress = 1 - quad.offset;
            } else {
                quad.progress = fillEnd - i - quad.offset;
            }
        }

        float sx = 1;
        float sy = 1;
        float sx2 = sx / 2;
        float sy2 = sy / 2;

        // collect points anv uvs
        MadList<Vector2> points = new MadList<Vector2>(4);
        MadList<Vector2> uvs = new MadList<Vector2>(4);

        vbo.Clear();

        Vector4 outerUv = DataUtility.GetOuterUV(sprite);
        outerUv = FixUV(outerUv);

        topRightQuad.Points(sx2, sy, sx, sy2, points);
        PreparePointsAndUvs(points, uvs, outerUv);
        AddToVbo(vbo, points, uvs);

        topRightQuad2.Points(sx2, sy, sx, sy2, points);
        PreparePointsAndUvs(points, uvs, outerUv);
        AddToVbo(vbo, points, uvs);

        bottomRightQuad.Points(sx2, sy2, sx, 0, points);
        PreparePointsAndUvs(points, uvs, outerUv);
        AddToVbo(vbo, points, uvs);

        bottomRightQuad2.Points(sx2, sy2, sx, 0, points);
        PreparePointsAndUvs(points, uvs, outerUv);
        AddToVbo(vbo, points, uvs);


        bottomLeftQuad.Points(0, sy2, sx2, 0, points);
        PreparePointsAndUvs(points, uvs, outerUv);
        AddToVbo(vbo, points, uvs);

        bottomLeftQuad2.Points(0, sy2, sx2, 0, points);
        PreparePointsAndUvs(points, uvs, outerUv);
        AddToVbo(vbo, points, uvs);

        topLeftQuad.Points(0, sy, sx2, sy2, points);
        PreparePointsAndUvs(points, uvs, outerUv);
        AddToVbo(vbo, points, uvs);
        topLeftQuad2.Points(0, sy, sx2, sy2, points);
        PreparePointsAndUvs(points, uvs, outerUv);
        AddToVbo(vbo, points, uvs);
    }

    private void PreparePointsAndUvs(MadList<Vector2> points, MadList<Vector2> uvs, Vector4 outerUv) {
        uvs.Clear();

        var drawingDimensions = GetDrawingDimensions(false);

        float left = drawingDimensions.x;
        float right = drawingDimensions.z;
        float bottom = drawingDimensions.y;
        float top = drawingDimensions.w;

        float width = right - left;
        float height = top - bottom;

        float uvw = outerUv.z - outerUv.x;
        float uvh = outerUv.w - outerUv.y;

        for (int i = 0; i < points.Count; i++) {
            var v = points[i];

            var uv = new Vector2(outerUv.x + uvw * v.x, outerUv.y + uvh * v.y);

            if (uvOffset != Vector2.zero) {
                uv.x += uvOffset.x;
                uv.y += uvOffset.y;
            }

            v.x *= width;
            v.y *= height;

            v.x += left;
            v.y += bottom;
            points[i] = v;
            uvs.Add(uv);
        }
    }

    private void AddToVbo(List<UIVertex> vbo, MadList<Vector2> points, MadList<Vector2> uvs) {
        UIVertex vert = UIVertex.simpleVert;
        for (int i = 0; i < points.Count; ++i) {
            var point = points[i];
            var uv = uvs[i];

            vert.position = point;
            vert.uv0 = new Vector2(uv.x, uv.y);
            vert.color = color;
            vbo.Add(vert);
        }
    }

    public override void SetNativeSize() {
        if (sprite == null) {
            return;
        }

        int num1 = Mathf.RoundToInt(sprite.rect.width);
        int num2 = Mathf.RoundToInt(sprite.rect.height);
        rectTransform.anchorMax = rectTransform.anchorMin;
        rectTransform.sizeDelta = new Vector2(num1, num2);

        SetAllDirty();
    }

    public Vector4 GetDrawingDimensions(bool shouldPreserveAspect) {
        //Vector4 vector4_1 = !(sprite == null) ? DataUtility.GetPadding(sprite) : Vector4.zero;
        //Debug.Log(vector4_1 + " " + GetPadding(sprite));

        Vector4 padding;
        padding = GetCachedPadding();

        Vector2 vector2 = !(sprite == null) ? new Vector2(sprite.rect.width, sprite.rect.height) : Vector2.zero;
        Rect pixelAdjustedRect = GetPixelAdjustedRect();
        int num1 = Mathf.RoundToInt(vector2.x);
        int num2 = Mathf.RoundToInt(vector2.y);
        Vector4 vector4_2 = new Vector4(padding.x / num1, padding.y / num2, (num1 - padding.z) / num1, (num2 - padding.w) / num2);
        if (shouldPreserveAspect && vector2.sqrMagnitude > 0.0) {
            float num3 = vector2.x / vector2.y;
            float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
            if ((double)num3 > num4) {
                float height = pixelAdjustedRect.height;
                pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
                pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * rectTransform.pivot.y;
            } else {
                float width = pixelAdjustedRect.width;
                pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
                pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * rectTransform.pivot.x;
            }
        }
        vector4_2 = new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector4_2.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector4_2.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector4_2.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector4_2.w);
        return vector4_2;
    }

    private Vector4 GetCachedPadding() {
        Vector4 padding;
        if (sprite != cachedSprite) {
            padding = GetPadding(sprite);
            cachedSprite = sprite;
            cachedPadding = padding;
        } else {
            padding = cachedPadding;
        }
        return padding;
    }

    private Vector4 GetPadding(Sprite sprite) {
        if (sprite == null) {
            return Vector4.zero;
        }

#if !EBT_FORCE_UNITY_FREE
        if (Application.HasProLicense() || Application.unityVersion.StartsWith("5.")) {
            return DataUtility.GetPadding(sprite);
        }
#endif

        var instanceId = sprite.texture.GetInstanceID();

        if (cachedPaddings.ContainsKey(instanceId)) {
            return cachedPaddings[instanceId];
        } else {
            var padding = GetPaddingManual(sprite);
            cachedPaddings[instanceId] = padding;
            return padding;
        }
    }

    private Vector4 GetPaddingManual(Sprite sprite) {
        if (!readable) {
            return Vector4.zero;
        }

        return Utils.ComputePadding(sprite.texture, sprite.rect);
    }

    class Quad {
        public Point anchor;
        public float offset;
        public float progress;
        public bool invert;

        public Quad(bool invert) {
            this.invert = invert;
        }

        public Quad(Quad other) {
            anchor = other.anchor;
            offset = other.offset;
            progress = other.progress;
            invert = other.invert;
        }

        public MadList<Vector2> Points(float left, float top, float right, float bottom, MadList<Vector2> list) {
            list.Clear();

            if (progress == 0) {
                return list;
            } else if (progress == 1) {
                list.Add(new Vector2(left, bottom));
                list.Add(new Vector2(left, top));
                list.Add(new Vector2(right, top));
                list.Add(new Vector2(right, bottom));
            } else {
                float progressY = Y(progress + offset);
                float hy = left + (right - left) * progressY;
                float vy = bottom + (top - bottom) * progressY;

                float progressOffset = offset + progress;

                float offsetY = Y(offset);
                float ohy = left + (right - left) * offsetY;
                float ovy = bottom + (top - bottom) * offsetY;

                switch (anchor) {
                    case Point.BottomLeft:
                        if (!invert) {
                            if (progressOffset < 0.5f) {
                                list.Add(new Vector2(left, bottom));
                                list.Add(new Vector2(ohy, top));
                                list.Add(new Vector2(hy, top));
                                list.Add(new Vector2(hy, top));
                            } else {
                                if (offset < 0.5f) {
                                    list.Add(new Vector2(left, bottom));
                                    list.Add(new Vector2(ohy, top));
                                    list.Add(new Vector2(right, top));
                                    list.Add(new Vector2(right, vy));
                                } else {
                                    list.Add(new Vector2(left, bottom));
                                    list.Add(new Vector2(right, ovy));
                                    list.Add(new Vector2(right, vy));
                                    list.Add(new Vector2(right, vy));
                                }

                            }
                        } else {
                            if (progressOffset < 0.5f) {
                                list.Add(new Vector2(left, bottom));
                                list.Add(new Vector2(right, ovy));
                                list.Add(new Vector2(right, vy));
                                list.Add(new Vector2(right, vy));
                            } else {
                                if (offset < 0.5f) {
                                    list.Add(new Vector2(left, bottom));
                                    list.Add(new Vector2(right, ovy));
                                    list.Add(new Vector2(right, top));
                                    list.Add(new Vector2(hy, top));
                                } else {
                                    list.Add(new Vector2(left, bottom));
                                    list.Add(new Vector2(ohy, top));
                                    list.Add(new Vector2(hy, top));
                                    list.Add(new Vector2(hy, top));
                                }
                            }
                        }
                        break;

                    case Point.TopLeft:
                        if (!invert) {
                            if (progressOffset < 0.5f) {
                                list.Add(new Vector2(left, top));
                                list.Add(new Vector2(right, top - ovy));
                                list.Add(new Vector2(right, top - vy));
                                list.Add(new Vector2(right, top - vy));
                            } else {
                                if (offset < 0.5f) {
                                    list.Add(new Vector2(left, top));
                                    list.Add(new Vector2(right, top - ovy));
                                    list.Add(new Vector2(right, bottom));
                                    list.Add(new Vector2(hy, bottom));
                                } else {
                                    list.Add(new Vector2(left, top));
                                    list.Add(new Vector2(ohy, bottom));
                                    list.Add(new Vector2(hy, bottom));
                                    list.Add(new Vector2(hy, bottom));
                                }

                            }
                        } else {
                            if (progressOffset < 0.5f) {
                                list.Add(new Vector2(left, top));
                                list.Add(new Vector2(ohy, bottom));
                                list.Add(new Vector2(hy, bottom));
                                list.Add(new Vector2(hy, bottom));
                            } else {
                                if (offset < 0.5f) {
                                    list.Add(new Vector2(left, top));
                                    list.Add(new Vector2(ohy, bottom));
                                    list.Add(new Vector2(right, bottom));
                                    list.Add(new Vector2(right, top - vy));
                                } else {
                                    list.Add(new Vector2(left, top));
                                    list.Add(new Vector2(right, top - ovy));
                                    list.Add(new Vector2(right, top - vy));
                                    list.Add(new Vector2(right, top - vy));
                                }
                            }
                        }
                        break;

                    case Point.TopRight:
                        if (!invert) {
                            if (progressOffset < 0.5f) {
                                list.Add(new Vector2(right, top));
                                list.Add(new Vector2(right - ohy, bottom));
                                list.Add(new Vector2(right - hy, bottom));
                                list.Add(new Vector2(right - hy, bottom));
                            } else {
                                if (offset < 0.5f) {
                                    list.Add(new Vector2(right, top));
                                    list.Add(new Vector2(right - ohy, bottom));
                                    list.Add(new Vector2(left, bottom));
                                    list.Add(new Vector2(left, top - vy));
                                } else {
                                    list.Add(new Vector2(right, top));
                                    list.Add(new Vector2(left, top - ovy));
                                    list.Add(new Vector2(left, top - vy));
                                    list.Add(new Vector2(left, top - vy));
                                }
                            }
                        } else {
                            if (progressOffset < 0.5f) {
                                list.Add(new Vector2(right, top));
                                list.Add(new Vector2(left, top - ovy));
                                list.Add(new Vector2(left, top - vy));
                                list.Add(new Vector2(left, top - vy));
                            } else {
                                if (offset < 0.5f) {
                                    list.Add(new Vector2(right, top));
                                    list.Add(new Vector2(left, top - ovy));
                                    list.Add(new Vector2(left, bottom));
                                    list.Add(new Vector2(right - hy, bottom));
                                } else {
                                    list.Add(new Vector2(right, top));
                                    list.Add(new Vector2(right - ohy, bottom));
                                    list.Add(new Vector2(right - hy, bottom));
                                    list.Add(new Vector2(right - hy, bottom));
                                }
                            }
                        }
                        break;

                    case Point.BottomRight:
                        if (!invert) {
                            if (progressOffset < 0.5f) {
                                list.Add(new Vector2(right, bottom));
                                list.Add(new Vector2(left, ovy));
                                list.Add(new Vector2(left, vy));
                                list.Add(new Vector2(left, vy));
                            } else {
                                if (offset < 0.5f) {
                                    list.Add(new Vector2(right, bottom));
                                    list.Add(new Vector2(left, ovy));
                                    list.Add(new Vector2(left, top));
                                    list.Add(new Vector2(right - hy, top));
                                } else {
                                    list.Add(new Vector2(right, bottom));
                                    list.Add(new Vector2(right - ohy, top));
                                    list.Add(new Vector2(right - hy, top));
                                    list.Add(new Vector2(right - hy, top));
                                }
                            }
                        } else {
                            if (progressOffset < 0.5f) {
                                list.Add(new Vector2(right, bottom));
                                list.Add(new Vector2(right - ohy, top));
                                list.Add(new Vector2(right - hy, top));
                                list.Add(new Vector2(right - hy, top));
                            } else {
                                if (offset < 0.5f) {
                                    list.Add(new Vector2(right, bottom));
                                    list.Add(new Vector2(right - ohy, top));
                                    list.Add(new Vector2(left, top));
                                    list.Add(new Vector2(left, vy));
                                } else {
                                    list.Add(new Vector2(right, bottom));
                                    list.Add(new Vector2(left, ovy));
                                    list.Add(new Vector2(left, vy));
                                    list.Add(new Vector2(left, vy));
                                }
                            }
                        }
                        break;

                    default:
                        Debug.LogError("Unknown anchor: " + anchor);
                        break;
                }
            }

            return list;
        }

        float Y(float val) {
            float x = 1;

            float p = (val < 0.5f) ? val : 1 - val;
            float angle = p * 90 * Mathf.Deg2Rad;

            float y = Mathf.Tan(angle) * x;
            return y;
        }

        public enum Point {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft,
        }
    }
}


}

