using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(JobButton))]
public class IJobButton : Editor {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	     	PRIVATE VARIABLES   			     					     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public override void OnInspectorGUI() {
		GUI.changed = false;

		// Get a reference to the extended class
		JobButton _button = target as JobButton;
		_button.jobType = (JOB_TYPE)EditorGUILayout.EnumPopup("Job Type", _button.jobType);

		if (_button.jobType == JOB_TYPE.Build) {
			_button.buildType = (BUILD_TYPE)EditorGUILayout.EnumPopup("Build Type", _button.buildType);
			if (_button.buildType == BUILD_TYPE.Decor) {
				_button.decorType = (DECOR_TYPE)EditorGUILayout.EnumPopup("Decor Type", _button.decorType);
			} else if (_button.buildType == BUILD_TYPE.Furniture) {
				_button.furnitureType = (FURNITURE_TYPE)EditorGUILayout.EnumPopup("Furniture Type", _button.furnitureType);
			} else if (_button.buildType == BUILD_TYPE.Item) {
				_button.itemType = (ITEM_TYPE)EditorGUILayout.EnumPopup("Item Type", _button.itemType);
			} else if (_button.buildType == BUILD_TYPE.Node) {
				_button.nodeType = (NODE_TYPE)EditorGUILayout.EnumPopup("Node Type", _button.nodeType);
			}
		} else if (_button.jobType == JOB_TYPE.Debug) {
			_button.debugType = (DEBUG_TYPE)EditorGUILayout.EnumPopup("Debug Type", _button.debugType);
		}

		// If changed then you need to set dirty
		if (GUI.changed) {
			EditorUtility.SetDirty(_button);
		}
	}
}
