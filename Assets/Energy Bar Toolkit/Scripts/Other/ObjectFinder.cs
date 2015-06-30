/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnergyBarToolkit {

[Serializable]
public class ObjectFinder {

    #region Public Fields

    public Method chosenMethod = Method.ByType;

    public string path;
    public string tag;
    public GameObject reference;

    public string typeString;
    public string assembly;

    #endregion

    #region Private Fields

    #endregion

    #region Public Methods

    public ObjectFinder(Type type, string defaultPath = "", string defaultTag = "", Method defaultMethod = Method.ByType) {
        path = defaultPath;
        tag = defaultTag;
        chosenMethod = defaultMethod;

        typeString = type.ToString();
        assembly = type.Assembly.FullName;
    }

    public T Lookup<T>(Object parent) where T : Component {

        switch (chosenMethod) {
            case Method.ByPath: {
                var go = GameObject.Find(path);
                if (go != null) {
                    return GetComponent<T>(go);
                } else {
                    Debug.LogError("Cannot find object on path " + path, parent);
                    return null;
                }
            }
            case Method.ByTag: {
                var go = GameObject.FindWithTag(tag);
                if (go != null) {
                    return GetComponent<T>(go);
                } else {
                    Debug.LogError("Cannot find object by tag " + tag, parent);
                    return null;
                }
            }
            case Method.ByType: {
                Type type = GetType();
                var component = Object.FindObjectOfType(type);
                if (component == null) {
                    Debug.LogError("Cannot find object of type " + type, parent);
                }

                return component as T;
            }
            case Method.ByReference:
                return GetComponent<T>(reference);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private T GetComponent<T>(GameObject go) where T : Component {
        var component = go.GetComponent<T>();
        if (component == null) {
            Debug.LogError("Cannot find component " + typeString + " on " + go, go);
        }

        return component;
    }

    private new Type GetType() {
        return Type.GetType(typeString + ", " + assembly);
    }

    #endregion

    #region Inner and Anonymous Classes

    [Flags]
    public enum Method {
        ByPath = 1,
        ByTag = 2,
        ByType = 4,
        ByReference = 8,
    }

    #endregion
}

} // namespace