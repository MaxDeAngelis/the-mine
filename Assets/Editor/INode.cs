using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class INode : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	PRIVATE VARIABLES   			     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	bool _stoneEdgeAccents = false;
	bool _resourceAccents = false;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		Node _node = target as Node;

		_node.type = (NODE_TYPE)EditorGUILayout.EnumPopup("Node Type", _node.type);
		if (_node.type == NODE_TYPE.Stone) {
			_stoneEdgeAccents = EditorGUILayout.Foldout(_stoneEdgeAccents, "Stone Edge Accents");
			if (_stoneEdgeAccents) {
				_node.accentTop = (GameObject)EditorGUILayout.ObjectField ("Top Accent", _node.accentTop, typeof(GameObject), true);
				_node.accentRight = (GameObject)EditorGUILayout.ObjectField ("Right Accent", _node.accentRight, typeof(GameObject), true);
				_node.accentBottom = (GameObject)EditorGUILayout.ObjectField ("Bottom Accent", _node.accentBottom, typeof(GameObject), true);
				_node.accentLeft = (GameObject)EditorGUILayout.ObjectField ("Left Accent", _node.accentLeft, typeof(GameObject), true);
			}

			_resourceAccents = EditorGUILayout.Foldout(_resourceAccents, "Resource Accents");
			if (_resourceAccents) {
				_node.accentIron = (GameObject)EditorGUILayout.ObjectField ("Iron Accent", _node.accentIron, typeof(GameObject), true);
				_node.accentGold = (GameObject)EditorGUILayout.ObjectField ("Gold Accent", _node.accentGold, typeof(GameObject), true);
			}

			_node.resource = (RESOURCE_TYPE)EditorGUILayout.EnumPopup("Resource Type", _node.resource);
			if (_node.resource != RESOURCE_TYPE.None) {
				_node.resourceAmount = EditorGUILayout.IntField("Resource Amount", _node.resourceAmount);
			}

		}

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_node);
		}
	}
}
