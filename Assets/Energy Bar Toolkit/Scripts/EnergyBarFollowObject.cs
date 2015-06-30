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

/// <summary>
/// Makes the object follow another world object.
/// </summary>
[ExecuteInEditMode]
public class EnergyBarFollowObject : MonoBehaviour {

    #region Public Fields

    public GameObject followObject;
    public Vector3 offset;

    public ObjectFinder worldCamera = new ObjectFinder(typeof(Camera), "/Main Camera", "MainCamera", ObjectFinder.Method.ByTag);

    public bool lookAtCamera = true;

    #endregion

    #region Private Fields

    [SerializeField]
    [HideInInspector]
    private Camera renderCamera;

    private Camera cameraReference;

    private Canvas canvas;

    #endregion

    #region Public Methods

    public bool IsPossiblyVisible() {
        if (followObject != null && cameraReference != null) {
            Vector3 heading = followObject.transform.position - cameraReference.transform.position;
            float dot = Vector3.Dot(heading, cameraReference.transform.forward);

            return dot >= 0;
        }

        Debug.LogError("Cannot determine visibility of this bar.", this);
        return false;
    }

    #endregion

    #region Unity Methods

    void OnEnable() {
#if UNITY_EDITOR
        if (renderCamera != null) {
            worldCamera.chosenMethod = ObjectFinder.Method.ByReference;
            worldCamera.reference = renderCamera.gameObject;
            renderCamera = null;
            EditorUtility.SetDirty(this);
        }
#endif
    }

    void Start() {
    }

    void Update() {
        if (followObject != null) {
            if (!Application.isPlaying || canvas == null) {
                canvas = GetComponentInParent<Canvas>();
                if (canvas == null) {
                    Debug.LogError("This object should be placed under a canvas", this);
                }
            }

            if (!Application.isPlaying || cameraReference == null) {
                cameraReference = worldCamera.Lookup<Camera>(this);
            }
            UpdateFollowObject();

            bool visible = IsPossiblyVisible();
            var energyBarBase = GetComponent<EnergyBarBase>();

            energyBarBase.opacity = visible ? 1 : 0;

            if (cameraReference != null && canvas != null) {
                if (canvas.renderMode == RenderMode.WorldSpace) {
                    energyBarBase.transform.rotation =
                        Quaternion.LookRotation(energyBarBase.transform.position - cameraReference.transform.position);
                } else {
                    energyBarBase.transform.rotation = Quaternion.identity;
                }
            }
        }
    }

    #endregion

    #region Private Methods

    private void UpdateFollowObject() {
        switch (canvas.renderMode) {
            case RenderMode.ScreenSpaceOverlay:
                UpdateFollowObjectScreenSpaceOverlay();
                break;
            case RenderMode.ScreenSpaceCamera:
                UpdateFollowObjectScreenSpaceCamera();
                break;
            case RenderMode.WorldSpace:
                UpdateFollowObjectWorldSpace();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void UpdateFollowObjectScreenSpaceOverlay() {
        UpdateScreenSpace();
    }

    private void UpdateFollowObjectScreenSpaceCamera() {
        UpdateScreenSpace();
    }

    private void UpdateScreenSpace() {
        if (cameraReference == null) {
            Debug.LogError("Render Camera must be set for the follow script to work.", this);
            return;
        }
        var rect = canvas.pixelRect;

        var w2 = rect.width / 2;
        var h2 = rect.height / 2;

        var screenPoint = cameraReference.WorldToScreenPoint(followObject.transform.position);
        var pos = screenPoint + offset - new Vector3(w2, h2);
        pos = new Vector3(pos.x / canvas.scaleFactor, pos.y / canvas.scaleFactor);

        MadTransform.SetLocalPosition(transform, pos);
    }

    private void UpdateFollowObjectWorldSpace() {
        MadTransform.SetPosition(transform, followObject.transform.position + offset);
    }

    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

} // namespace