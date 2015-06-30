/*
* Copyright (c) Mad Pixel Machine, Unity Technologies
* http://www.madpixelmachine.com/
*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnergyBarToolkit {

public class EBTPackerPolicy : IPackerPolicy {

    protected class Entry {
        public Sprite sprite;
        public AtlasSettings settings;
        public string atlasName;
        public SpritePackingMode packingMode;
        public int anisoLevel;
        public uint paddingPower;
    }

    private const uint kDefaultPaddingPower = 2; // Good for base and two mip levels.

    public virtual int GetVersion() {
        return 8;
    }

    protected virtual string TagPrefix { get { return "[TIGHT]"; } }
    protected virtual bool AllowTightWhenTagged { get { return true; } }

    public void OnGroupAtlases(BuildTarget target, PackerJob job, int[] textureImporterInstanceIDs) {
        List<Entry> entries = new List<Entry>();

        foreach (int instanceID in textureImporterInstanceIDs) {
            TextureImporter ti = EditorUtility.InstanceIDToObject(instanceID) as TextureImporter;
            var texture = AssetDatabase.LoadAssetAtPath(ti.assetPath, typeof(Texture2D)) as Texture2D;
            int paddingPower = ComputePaddingPower(texture);
            

#if UNITY_4_6
            TextureImportInstructions ins = new TextureImportInstructions();
            ti.ReadTextureImportInstructions(ins, target);
#else // Unity 5
            ColorSpace colorSpace;
            int compression;
            TextureFormat format;
            
            ti.ReadTextureImportInstructions(target, out format, out colorSpace, out compression);
            TextureImporterSettings tis = new TextureImporterSettings();
            ti.ReadTextureSettings(tis);
#endif


            Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(ti.assetPath).Select(x => x as Sprite).Where(x => x != null).ToArray();
            foreach (Sprite sprite in sprites) {
                Entry entry = new Entry();
                entry.sprite = sprite;

#if UNITY_4_6
                entry.settings.format = ins.desiredFormat;
                entry.settings.usageMode = TextureUsageMode.Default;
                entry.settings.colorSpace = ins.colorSpace;
                entry.settings.compressionQuality = ins.compressionQuality;
#else // Unity 5
                entry.settings.format = format;
                entry.settings.colorSpace = colorSpace;
                entry.settings.compressionQuality = compression;
#endif

                entry.settings.filterMode = Enum.IsDefined(typeof(FilterMode), ti.filterMode) ? ti.filterMode : FilterMode.Bilinear;
                entry.settings.maxWidth = 2048;
                entry.settings.maxHeight = 2048;
                entry.settings.generateMipMaps = ti.mipmapEnabled;
                //if (ti.mipmapEnabled)
                entry.paddingPower = (uint)paddingPower;

                // using padding power group as a hack to group sprites with similar padding power
                entry.settings.paddingPower = PaddingPowerGroup(paddingPower);
                Debug.Log("Groupping " + paddingPower + " to " + entry.settings.paddingPower);

                entry.atlasName = ParseAtlasName(ti.spritePackingTag);
                entry.packingMode = SpritePackingMode.Rectangle;
                entry.anisoLevel = ti.anisoLevel;

                entries.Add(entry);
            }

            Resources.UnloadAsset(ti);
            Resources.UnloadAsset(texture);
        }

        // First split sprites into groups based on atlas name
        var atlasGroups =
        from e in entries
        group e by e.atlasName;
        foreach (var atlasGroup in atlasGroups) {
            int page = 0;
            // Then split those groups into smaller groups based on texture settings
            var settingsGroup =
            from t in atlasGroup
            group t by t.settings;

            foreach (var settingGroup in settingsGroup) {
                string atlasName = atlasGroup.Key;
                if (page > 0)
                    atlasName += string.Format(" (Group {0})", page);

                AtlasSettings settings = settingGroup.Key;

                settings.anisoLevel = 1;
                // Use the highest aniso level from all entries in this atlas
                if (settings.generateMipMaps)
                    foreach (Entry entry in settingGroup)
                        if (entry.anisoLevel > settings.anisoLevel)
                            settings.anisoLevel = entry.anisoLevel;

                // apply now the padding power because grouping would fail earlier
                uint biggest = BiggestPadding(settingGroup);
                settings.paddingPower = biggest;

                job.AddAtlas(atlasName, settings);

                foreach (Entry entry in settingGroup) {
                    job.AssignToAtlas(atlasName, entry.sprite, SpritePackingMode.Rectangle, SpritePackingRotation.None);
                }

                ++page;
            }
        }
    }

    // grouping padding powers close to each other
    private uint PaddingPowerGroup(int paddingPower) {
        if (paddingPower <= 4) { // up to 16
            return 0;
        } else if (paddingPower == 5 || paddingPower == 6) {
            // 32 & 64
            return 1;
        } else { // 128 and up
            return (uint)paddingPower;
        }
    }

    private uint BiggestPadding(IGrouping<AtlasSettings, Entry> entriesGroup) {
        uint biggest = kDefaultPaddingPower;
        foreach (var entry in entriesGroup) {
            biggest = (uint)Mathf.Max(biggest, entry.paddingPower);
        }
        return biggest;
    }

    private int ComputePaddingPower(Texture2D texture) {
        using (new Utils.TextureReadable(texture)) {
            Vector4 padding = Utils.ComputePadding(texture);
            float maxPadding = Mathf.Max(padding.x, padding.y, padding.w, padding.z);

            if ((int)maxPadding == 0) {
                return (int)kDefaultPaddingPower;
            }

            int pot = Mathf.NextPowerOfTwo((int)maxPadding);
            int result = (int)Mathf.Log(pot, 2) - 1;

            return Mathf.Max((int)kDefaultPaddingPower, result);
        }
    }

    protected bool IsTagPrefixed(string packingTag) {
        packingTag = packingTag.Trim();
        if (packingTag.Length < TagPrefix.Length)
            return false;
        return (packingTag.Substring(0, TagPrefix.Length) == TagPrefix);
    }

    private string ParseAtlasName(string packingTag) {
        string name = packingTag.Trim();
        if (IsTagPrefixed(name))
            name = name.Substring(TagPrefix.Length).Trim();
        return (name.Length == 0) ? "(unnamed)" : name;
    }

    private SpritePackingMode GetPackingMode(string packingTag, SpriteMeshType meshType) {
        if (meshType == SpriteMeshType.Tight)
            if (IsTagPrefixed(packingTag) == AllowTightWhenTagged)
                return SpritePackingMode.Tight;
        return SpritePackingMode.Rectangle;
    }
}

} // namespace