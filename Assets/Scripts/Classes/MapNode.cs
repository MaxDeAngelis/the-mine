using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class MapNode {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    GameObject _nodeType;
    Vector2 _location;

    bool _isActive = false;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		   CONSTRUCTOR   											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public MapNode(GameObject type, Vector2 location) {
        _nodeType = type;
        _location = location;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Vector2 getLocation() {
        return _location;
    }

    public GameObject getNodeType() {
        return _nodeType;
    }

    public void setActive(bool value) {
        _isActive = value;
    }

    public bool isActive() {
        return _isActive;
    }


    
}