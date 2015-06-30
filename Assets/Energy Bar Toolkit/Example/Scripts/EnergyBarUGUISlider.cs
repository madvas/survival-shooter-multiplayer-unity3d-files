using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EnergyBarToolkit {

public class EnergyBarUGUISlider : MonoBehaviour {

    private EnergyBar[] allBars;

	// Use this for initialization
    void Start () {
        allBars = FindObjectsOfType<EnergyBar>();
	}

    public void OnValueChanged() {
        var slider = GetComponent<Slider>();
        var val = slider.normalizedValue;

        for (int i = 0; i < allBars.Length; i++) {
            var bar = allBars[i];
            bar.ValueF = val;
        }
    }
}


} // namespace