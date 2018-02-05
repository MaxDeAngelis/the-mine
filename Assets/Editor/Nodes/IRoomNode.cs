using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RoomNode))]
public class IRoomNode : INode {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	PRIVATE VARIABLES   			     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	bool _roomSprites = false;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		RoomNode _node = target as RoomNode;

		_node.type = NODE_TYPE.Room;

		_roomSprites = EditorGUILayout.Foldout(_roomSprites, "Room Sprites");
		if (_roomSprites) {
			_node.roomBottomLeft = (Sprite)EditorGUILayout.ObjectField ("Bottom Left", _node.roomBottomLeft, typeof(Sprite), true);
			_node.roomBottomMiddle = (Sprite)EditorGUILayout.ObjectField ("Bottom Middle", _node.roomBottomMiddle, typeof(Sprite), true);
			_node.roomBottomRight = (Sprite)EditorGUILayout.ObjectField ("Bottom Right", _node.roomBottomRight, typeof(Sprite), true);
			_node.roomTopLeft = (Sprite)EditorGUILayout.ObjectField ("Top Left", _node.roomTopLeft, typeof(Sprite), true);
			_node.roomTopMiddle = (Sprite)EditorGUILayout.ObjectField ("Top Middle", _node.roomTopMiddle, typeof(Sprite), true);
			_node.roomTopRight = (Sprite)EditorGUILayout.ObjectField ("Top Right", _node.roomTopRight, typeof(Sprite), true);
		}

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_node);
		}
	}
}
