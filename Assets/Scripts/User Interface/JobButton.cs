using UnityEngine;
using System.Collections;

public class JobButton : Button {
	public JOB_TYPE type;
	public BUILD_SUB_TYPE buildSubType;
    public PLACE_SUB_TYPE placeSubType;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
    /// <summary>
    /// Select function for the button called on click
    /// </summary>
    public void select() {
		JobManager.Instance.isCommandSelected = true;
		JobManager.Instance.setCommandType(type);
		JobManager.Instance.setBuildSubType(buildSubType);
        JobManager.Instance.setPlaceSubType(placeSubType);
	}
}
