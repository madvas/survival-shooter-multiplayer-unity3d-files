/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System.Collections;
using UnityEngine;

namespace EnergyBarToolkit {

/// <summary>
/// This is only a example script that is ugin Repeated Renderer UGUI API to get and animate last
/// visible icon in sequence.
/// </summary>
public class RepeatedUGUILastIconAnimator : MonoBehaviour {
    
    #region Public Fields

    public float animationTime = 2;

    public float scaleFrom = 1;
    public float scaleTo = 2;

    public float alphaFrom = 1;
    public float alphaTo = 0;

    private MadiTween.EaseType scaleEaseType = MadiTween.EaseType.easeOutCubic;

    #endregion

    #region Private Fields

    private RepeatedRendererUGUI barRenderer;

    public MadiTween.EaseType easeType {
        get { return scaleEaseType; }
        set { scaleEaseType = value; }
    }

    #endregion

    #region Unity Methods

    void Start() {
        barRenderer = GetComponent<RepeatedRendererUGUI>();

        // Doing the animation in coroutine have two advantages:
        // 1 - You don't have to set script execution order, because Coroutines are executed after Update() functions
        // 2 - Coroutines are usually easy to read
        StartCoroutine(Anim());
    }

    #endregion

    #region Private Methods

    public IEnumerator Anim() {
        while (enabled) { // infinite animation coroutine
            var visibleCount = barRenderer.GetVisibleIconCount();
            if (visibleCount > 0) {
                var image = barRenderer.GetIconImage(visibleCount - 1);

                // make a copy
                var clone = (Image2) Instantiate(image);

                // changing the name, because "generated_*" icons are treated in a special way and this may lead to errors
                clone.name = "anim icon";

                clone.transform.SetParent(image.transform.parent, false);
                clone.transform.position = image.transform.position;

                clone.transform.SetSiblingIndex(image.transform.GetSiblingIndex());

                // do the animation
                float startTime = Time.time;
                float endTime = Time.time + animationTime;

                var easingFunction = MadiTween.GetEasingFunction(scaleEaseType);
                while (Time.time < endTime) {
                    float f = (Time.time - startTime) / animationTime;
                    var scale = easingFunction.Invoke(scaleFrom, scaleTo, f);
                    clone.transform.localScale = new Vector3(scale, scale, scale);

                    var alpha = easingFunction.Invoke(alphaFrom, alphaTo, f);
                    clone.color = new Color(clone.color.r, clone.color.g, clone.color.b, alpha);

                    yield return null; // next frame
                }

                // remove
                Destroy(clone.gameObject);
            } else {
                // if there's no last icon, just wait
                yield return new WaitForSeconds(animationTime);
            }
        }
    }

    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

} // namespace