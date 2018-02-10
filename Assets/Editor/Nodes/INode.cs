﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class INode : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	PRIVATE VARIABLES   			     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		Node _node = target as Node;
		_node.type = (NODE_TYPE)EditorGUILayout.EnumPopup("Node Type", _node.type);

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_node);
		}
	}
}