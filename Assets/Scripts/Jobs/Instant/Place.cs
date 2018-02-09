using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Place : Build {
    private Node _nodeToReplace;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Place(Node location, GameObject objectToPlace) : base(location, 0f, 0f, objectToPlace) {
        _title = "Place \n" + objectToPlace.name;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PUBLIC FUNCTIONS                                               ///
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
    /// Called to see if this job is an instant build
    /// </summary>
    /// <returns><c>true</c>, if job should be completed instantly, <c>false</c> otherwise.</returns>
    public override bool isInstant() {
        return true;
    }

    /// <summary>
    /// Ises the valid location.
    /// </summary>
    /// <returns><c>true</c>, if valid location was ised, <c>false</c> otherwise.</returns>
    public override bool isValidLocation() {
        return (_location.isTravelable());
    }
}
