/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnergyBarToolkit {

public class Utils {

    #region Public Fields
    #endregion

    #region Private Fields
    #endregion

    #region Public Methods

#if UNITY_EDITOR

    public static bool IsReadable(Texture texture) {
        var assetPath = AssetDatabase.GetAssetPath(texture);
        var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        return textureImporter.isReadable;
    }

    public static void SetReadable(Texture texture) {
        var assetPath = AssetDatabase.GetAssetPath(texture);
        var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        textureImporter.textureType = TextureImporterType.Advanced;
        textureImporter.isReadable = true;

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
    }

#endif

    public static Vector4 ComputePadding(Texture2D texture) {
        return ComputePadding(texture, new Rect(0, 0, texture.width, texture.height));
    }

    public static Vector4 ComputePadding(Texture2D texture, Rect rect) {
        var colors = texture.GetPixels32();
        float left = 0, right = 0, top = 0, bottom = 0;

        int stride = texture.width;
        int xMin = (int) rect.xMin;
        int yMin = (int) rect.yMin;
        int xMax = (int) (rect.xMax - 1); // xMax and yMax of rect is working a little different than expected
        int yMax = (int) (rect.yMax - 1);

        // scanning left
        for (int i = xMin; i < xMax; ++i) {
            if (HasVisibleVert(i, colors, rect, stride)) {
                left = i;
                break;
            }
        }

        // scanning right
        for (int i = xMin; i < xMax; ++i) {
            int i2 = xMax - i - 1;
            if (HasVisibleVert(i2, colors, rect, stride)) {
                right = i;
                break;
            }
        }

        // scanning bottom
        for (int i = yMin; i < yMax; ++i) {
            if (HasVisibleHoriz(i, colors, rect, stride)) {
                bottom = i;
                break;
            }
        }

        // scanning top
        for (int i = yMin; i < yMax; i++) {
            int i2 = yMax - i - 1;
            if (HasVisibleHoriz(i2, colors, rect, stride)) {
                top = i;
                break;
            }
        }

        // apply 2 pixel padding
        left = Mathf.Max(0, left - 2);
        bottom = Mathf.Max(0, bottom - 2);
        right = Mathf.Max(0, right - 2);
        top = Mathf.Max(0, top - 2);

        return new Vector4(left, bottom, right, top);
    }

    private static bool HasVisibleHoriz(int y, Color32[] colors, Rect rect, int stride) {
        return HasVisible((int) rect.xMin, y, (int) rect.xMax - 1, y, colors, stride);
    }

    private static bool HasVisibleVert(int x, Color32[] colors, Rect rect, int stride) {
        return HasVisible(x, (int) rect.yMin, x, (int) rect.yMax - 1, colors, stride);
    }

    private static bool HasVisible(int minX, int minY, int maxX, int maxY, Color32[] colors, int stride) {
        for (int y = minY; y <= maxY; ++y) {
            for (int x = minX; x <= maxX; ++x) {
                int index = Index(x, y, stride);
                if (index >= colors.Length) {
                    Debug.LogError("out of range " + index + ", size " + colors.Length);
                    return false;
                }
                var color = colors[index];
                if (color.a != 0) {
                    return true;
                }
            }
        }

        return false;
    }

    private static int Index(int x, int y, int w) {
        return y * w + x;
    }

    #endregion

    #region Unity Methods

    void Start() {
    }

    void Update() {
    }

    #endregion

    #region Private Methods
    #endregion

    #region Inner and Anonymous Classes

    public class TextureReadable : IDisposable {
        private readonly bool isReadable;

#if UNITY_EDITOR
        private readonly TextureImporter textureImporter;
#endif
        private readonly string assetPath;

        public TextureReadable(Texture texture) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                assetPath = AssetDatabase.GetAssetPath(texture);
                textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                isReadable = textureImporter.isReadable;

                if (!isReadable) {
                    textureImporter.isReadable = true;
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }
            } else {
                DisplayError(texture);
            }
#else
            DisplayError(texture);
#endif
        }

        private void DisplayError(Texture texture) {
            Debug.LogError("Texture " + texture + " must be set as readable!");
        }

        public void Dispose() {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                if (!isReadable) {
                    textureImporter.isReadable = false;
                    AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                }
            }
#endif
        }
    }

    #endregion
}

} // namespace