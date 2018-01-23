﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move : Job {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTRUCTOR												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Move(Node location, float duration, float progress) : base(location, duration, progress) {
		_title = "Move";
		_type = JOB_TYPE.Move;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     	      PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Overridden version of getWorkLocation to return the correct locations
	/// </summary>
	/// <returns>The list of nodes that the Job can be completed from</returns>
	public override List<Node> getWorkLocations() {
		List<Node> workLocations = new List<Node>();

		workLocations.Add(_location);

		return workLocations;
	}

    public override bool isValidLocation() {
        return (_location.isTravelable());
    }
}
