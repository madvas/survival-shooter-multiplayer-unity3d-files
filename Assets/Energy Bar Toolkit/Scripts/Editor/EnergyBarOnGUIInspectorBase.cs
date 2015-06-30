/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System.Collections.Generic;
using UnityEngine;

namespace EnergyBarToolkit {

public class EnergyBarOnGUIInspectorBase : EnergyBarInspectorBase {

    protected List<Texture2D> BackgroundTextures() {
        return TexturesOf((target as EnergyBarOnGUIBase).texturesBackground);
    }

    protected List<Texture2D> ForegroundTextures() {
        return TexturesOf((target as EnergyBarOnGUIBase).texturesForeground);
    }

}

} // namespace