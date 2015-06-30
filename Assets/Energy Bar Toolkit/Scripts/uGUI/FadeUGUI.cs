/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEngine;

[ExecuteInEditMode]
public class FadeUGUI : MonoBehaviour {

    #region Public Fields

    public float invisibleDistance = 100;
    public float fadeDistance = 70;

    public Transform target;
    private EnergyBarBase energyBarBase;

    #endregion

    #region Private Fields
    #endregion

    #region Public Methods
    #endregion

    #region Unity Methods

    void OnEnable() {
        energyBarBase = GetComponent<EnergyBarBase>();
    }

    void Update() {
        if (target == null) {
            return;
        }

        float distance = (Camera.main.transform.position - target.position).magnitude;

        if (distance < fadeDistance) {
            SetAlpha(1);
        } else if (distance > invisibleDistance) {
            SetAlpha(0);
        } else {
            SetAlpha((distance - fadeDistance) / (invisibleDistance - fadeDistance));
        }
    }

    private void SetAlpha(float alpha) {
        energyBarBase.tint = new Color(1, 1, 1, alpha);
    }

    #endregion

    #region Private Methods
    #endregion

    #region Inner and Anonymous Classes
    #endregion
}