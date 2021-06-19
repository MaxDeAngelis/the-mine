using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class MapDataNode {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PRIVATE VARIABLES											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    GameObject _nodeType;

    Node _terrain = null;
    Vector2 _location;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		   CONSTRUCTOR   											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public MapDataNode (GameObject type, Vector2 location) {
        _nodeType = type;
        _location = location;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PUBLIC FUNCTIONS											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void setTerrain (Node terrain) {
        _terrain = terrain;
    }

    public Node getTerrain () {
        return _terrain;
    }
    public Vector2 getLocation () {
        return _location;
    }

    public GameObject getNodeType () {
        return _nodeType;
    }

    public bool isTravelable () {
        if (_terrain != null) {
            return _terrain.isTravelable ();
        }
        return _nodeType.GetComponent<Node> ().isTravelable ();
    }

    public void hide () {
        if (_terrain != null) {
            _terrain.gameObject.SetActive (false);
        }
    }
    public void show () {
        if (_terrain != null) {
            _terrain.gameObject.SetActive (true);
        }
    }
}