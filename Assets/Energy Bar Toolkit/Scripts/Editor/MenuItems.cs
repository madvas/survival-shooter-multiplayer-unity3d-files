/*
* Energy Bar Toolkit by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace EnergyBarToolkit {

public class MenuItems : ScriptableObject {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    static Transform ActiveParentOrPanel() {
        Transform parentTransform = null;
        
        var transforms = Selection.transforms;
        if (transforms.Length > 0) {
            var firstTransform = transforms[0];
            if (MadTransform.FindParent<MadPanel>(firstTransform) != null) {
                parentTransform = firstTransform;
            }
        }
        
        if (parentTransform == null) {
            var panel = MadPanel.UniqueOrNull();
            if (panel != null) {
                parentTransform = panel.transform;
            }
        }
        
        return parentTransform;
    }
    
    static T Create<T>(string name) where T : Component {
        var parent = Selection.activeTransform;
        var component = MadTransform.CreateChild<T>(parent, name);
        Selection.activeObject = component.gameObject;
        return component;
    }

    [MenuItem("GameObject/UI/Energy Bar Toolkit/Filled Bar", false, 0)]
    [MenuItem("Tools/Energy Bar Toolkit/uGUI/Create Filled Bar", false, 0)]
    static void CreateFilledUGUI() {
        UGUIBarsBuilder.CreateFilled();
    }

    [MenuItem("GameObject/UI/Energy Bar Toolkit/Sliced Bar", false, 1)]
    [MenuItem("Tools/Energy Bar Toolkit/uGUI/Create Sliced Bar", false, 1)]
    static void CreateSlicedUGUI() {
        UGUIBarsBuilder.CreateSliced();
    }

    [MenuItem("GameObject/UI/Energy Bar Toolkit/Repeated Bar", false, 2)]
    [MenuItem("Tools/Energy Bar Toolkit/uGUI/Create Repeated Bar", false, 2)]
    static void CreateRepeatedUGUI() {
        UGUIBarsBuilder.CreateRepeated();
    }

    [MenuItem("GameObject/UI/Energy Bar Toolkit/Sequence Bar", false, 3)]
    [MenuItem("Tools/Energy Bar Toolkit/uGUI/Create Sequence Bar", false, 3)]
    static void CreateSequenceUGUI() {
        UGUIBarsBuilder.CreateSequence();
    }

    [MenuItem("GameObject/UI/Energy Bar Toolkit/Transform Bar", false, 4)]
    [MenuItem("Tools/Energy Bar Toolkit/uGUI/Create Transform Bar", false, 4)]
    static void CreateTransformUGUI() {
        UGUIBarsBuilder.CreateTransform();
    }

    [MenuItem("Tools/Energy Bar Toolkit/uGUI/Add Components/Follow Object", false, 50)]
    static void AttachFollowObject() {
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<EnergyBarUGUIBase>()) {
            Selection.activeGameObject.AddComponent<EnergyBarFollowObject>();
        } else {
            EditorUtility.DisplayDialog("Select uGUI Bar First", "You have to select uGUI bar first!", "OK");
        }
    }

    [MenuItem("Tools/Energy Bar Toolkit/uGUI/Add Components/Spawner", false, 51)]
    static void AttachSpawner() {
        Selection.activeGameObject.AddComponent<EnergyBarSpawnerUGUI>();
    }

    [MenuItem("Tools/Energy Bar Toolkit/Online Resources/Online Manual", false, 101)]
    static void OnlineManual() {
        Application.OpenURL(
            "http://energybartoolkit.madpixelmachine.com/documentation.html");
    }

    [MenuItem("Tools/Energy Bar Toolkit/Online Resources/Examples", false, 102)]
    static void Examples() {
        Application.OpenURL(
            "http://energybartoolkit.madpixelmachine.com/demo.html");
    }

    [MenuItem("Tools/Energy Bar Toolkit/Online Resources/Change Log", false, 103)]
    static void ReleaseNotes() {
        Application.OpenURL(
            "http://energybartoolkit.madpixelmachine.com/doc/latest/changelog.html");
    }

    [MenuItem("Tools/Energy Bar Toolkit/Online Resources/MadPixelMachine", false, 104)]
    static void MadPixelMachine() {
        Application.OpenURL(
            "http://madpixelmachine.com");
    }

    [MenuItem("Tools/Energy Bar Toolkit/Online Resources/Support", false, 150)]
    static void Support() {
        Application.OpenURL(
            "http://energybartoolkit.madpixelmachine.com/doc/latest/support.html");
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/Initialize", false, 1000)]
    static void InitTool() {
        MadInitTool.ShowWindow();
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/Create Atlas", false, 1050)]
    static void CreateAtlas() {
        MadAtlasBuilder.CreateAtlas();
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/Create Font", false, 1051)]
    static void CreateFont() {
        MadFontBuilder.CreateFont();
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/UI/Sprite", false, 1100)]
    static void CreateSprite() {
        var sprite = MadTransform.CreateChild<MadSprite>(ActiveParentOrPanel(), "sprite");
        Selection.activeGameObject = sprite.gameObject;
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/UI/Text", false, 1101)]
    static void CreateText() {
        var text = MadTransform.CreateChild<MadText>(ActiveParentOrPanel(), "text");
        Selection.activeGameObject = text.gameObject;
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/UI/Anchor", false, 1102)]
    static void CreateAnchor() {
        var anchor = MadTransform.CreateChild<MadAnchor>(ActiveParentOrPanel(), "Anchor");
        Selection.activeGameObject = anchor.gameObject;
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/Filled", false, 1200)]
    static void CreateMeshFillRenderer() {
        FilledRenderer3DBuilder.Create();
    }
    
    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/Repeated", false, 1201)]
    static void CreateMeshRepeatRenderer() {
        RepeatRenderer3DBuilder.Create();
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/Sequence", false, 1202)]
    static void CreateMeshSequenceRenderer() {
        SequenceRenderer3DBuilder.Create();
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/Mesh Bars/Transform", false, 1203)]
    static void CreateMeshTransformRenderer() {
        TransformRenderer3DBuilder.Create();
    }

    [MenuItem("Tools/Energy Bar Toolkit/Old/OnGUI Bars/Fill Renderer", false, 1210)]
    static void CreateFillRendererOnGUI() {
        Create<EnergyBarRenderer>("fill renderer");
    }
    
    [MenuItem("Tools/Energy Bar Toolkit/Old/OnGUI Bars/Repeat Renderer", false, 1211)]
    static void CreateRepeatRendererOnGUI() {
        Create<EnergyBarRepeatRenderer>("repeat renderer");
    }
    
    [MenuItem("Tools/Energy Bar Toolkit/Old/OnGUI Bars/Sequence Renderer", false, 1212)]
    static void CreateSequenceRendererOnGUI() {
        Create<EnergyBarSequenceRenderer>("sequence renderer");
    }
    
    [MenuItem("Tools/Energy Bar Toolkit/Old/OnGUI Bars/Transform Renderer", false, 1213)]
    static void CreateTransformRendererOnGUI() {
        Create<EnergyBarTransformRenderer>("transform renderer");
    }
 
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace