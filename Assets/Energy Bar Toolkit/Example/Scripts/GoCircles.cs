/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using UnityEngine;

namespace EnergyBarToolkit {

public class GoCircles : MonoBehaviour {

    #region Public Fields

    public float distance = 3;
    public float speed = 100;

    #endregion

    #region Private Fields

    private Vector3 startPosition;

    private float angle;

    #endregion

    #region Public Methods
    #endregion

    #region Unity Methods

    void Start() {
        startPosition = transform.position;
    }

    void Update() {
        float x = Mathf.Cos(Mathf.Deg2Rad * angle) * distance;
        float z = Mathf.Sin(Mathf.Deg2Rad * angle) * distance;

        transform.position = startPosition + new Vector3(x, 0, z);

        angle += Time.deltaTime * speed;
    }

    #endregion

    #region Private Methods
    #endregion

    #region Inner and Anonymous Classes
    #endregion
}

} // namespace