using UnityEngine;
using System.Collections;

public class JobButton : MonoBehaviour {
	public JOB_TYPE type;
	public BUILD_SUB_TYPE buildSubType;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void select() {
		JobManager.Instance.isCommandSelected = true;
		JobManager.Instance.setCommandType(type);
		JobManager.Instance.setBuildSubType(buildSubType);
	}

	/// <summary>
	/// Called on mouse enter to show where cursor is
	/// </summary>
	public void hover() {
		Vector3 scale = gameObject.transform.localScale;
		scale += new Vector3(0.2f, 0.2f, 0.2f);
		gameObject.transform.localScale = scale;
	}

	/// <summary>
	/// Called on mouse enter to show where cursor is
	/// </summary>
	public void blur() {
		Vector3 scale = gameObject.transform.localScale;
		scale -= new Vector3(0.2f, 0.2f, 0.2f);
		gameObject.transform.localScale = scale;
	}
}
