using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class MapNode {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PRIVATE VARIABLES											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    GameObject _nodeType;

    GameObject _terrain = null;
    Vector2 _location;

    bool _isActive = false;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		   CONSTRUCTOR   											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public MapNode (GameObject type, Vector2 location) {
        _nodeType = type;
        _location = location;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PUBLIC FUNCTIONS											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void setTerrain (GameObject terrain) {
        _terrain = terrain;
    }

    public GameObject getTerrain () {
        return _terrain;
    }
    public Vector2 getLocation () {
        return _location;
    }

    public GameObject getNodeType () {
        return _nodeType;
    }

    public void setActive (bool value) {
        _isActive = value;
    }

    public bool isActive () {
        return _isActive;
    }

}