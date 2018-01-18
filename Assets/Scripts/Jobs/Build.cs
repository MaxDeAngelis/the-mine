using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Build : Job {
    protected GameObject _finishedObject;
    private GameObject _objectToBuild;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Build(Node location, float duration, float progress, GameObject objectToBuild) : base(location, duration, progress) {
        _type = JOB_TYPE.Build;
        _objectToBuild = objectToBuild;
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
        workLocations = MapManager.Instance.getSurroundingNodes(_location, false);

        return workLocations;
    }

    /// <summary>
    /// Called to complete the build job. Handles clearing the tree and updating the ground nodes to be walkable
    /// </summary>
    public override void complete() {
        base.complete();

        // Calculate where to put new item
        Vector3 targetLocation = _location.transform.position;

        // Instantiate item and move into place
        _finishedObject = GameObject.Instantiate(_objectToBuild) as GameObject;
        _finishedObject.transform.position = targetLocation;
    }
}
