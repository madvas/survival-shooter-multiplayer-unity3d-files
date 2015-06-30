/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/


using System.Collections.Generic;
using UnityEngine;

namespace EnergyBarToolkit {

[ExecuteInEditMode]
public class InstantiateRandom : MonoBehaviour {

    #region Public Fields

    public EnergyBarSpawnerUGUI prefab;

    public Bounds bounds;

    #endregion

    #region Private Fields
    
    private List<GameObject> instances = new List<GameObject>();

    #endregion

    #region Unity Methods

    void OnGUI() {
        if (!Application.isPlaying) {
            GUILayout.Label("Hit the Play button!");
        } else {
            if (GUILayout.Button("Instantiate Now")) {
                var instance = (EnergyBarSpawnerUGUI) Instantiate(prefab, RandomPosition(), Quaternion.identity);
                instances.Add(instance.gameObject);
            }

            if (GUILayout.Button("Destroy Any") && instances.Count > 0) {
                int index = Random.Range(0, instances.Count);
                var go = instances[index];
                Destroy(go);
                instances.RemoveAt(index);
            }
        }
    }

    private Vector3 RandomPosition() {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
    }

    #endregion
}

} // namespace