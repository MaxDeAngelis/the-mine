using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move : Job {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTRUCTOR												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Move(Node location) : base(location, 0f, 0f) {
		_title = "Move";
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

    /// <summary>
    /// Ises the valid location.
    /// </summary>
    /// <returns><c>true</c>, if valid location was ised, <c>false</c> otherwise.</returns>
    public override bool isValidLocation() {
        return (_location.isTravelable());
    }
}
