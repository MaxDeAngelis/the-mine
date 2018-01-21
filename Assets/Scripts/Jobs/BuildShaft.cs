using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildShaft : Build {
    private Node _nodeToReplace;
    public BuildShaft(Node location, float duration, float progress) : base(location, duration, progress, ItemLibrary.Instance.shaftBlock) {
        _title = "Build \nShaft";
        _nodeToReplace = location;
    }

    /// <summary>
    /// Overridden version of getWorkLocation to return the correct locations
    /// </summary>
    /// <returns>The list of nodes that the Job can be completed from</returns>
    public override List<Node> getWorkLocations() {
        List<Node> potentialLocations = new List<Node>();
        potentialLocations = MapManager.Instance.getSurroundingNodes(_location, false);

        List<Node> workLocations = new List<Node>();
        workLocations.Add(_location);

        Vector3 top = _location.transform.position + Vector3.up;
        Vector3 bottom = _location.transform.position + Vector3.down;

        foreach (Node node in potentialLocations) {
            if (node.isTravelable()) {
                workLocations.Add(node);
            }
        }

        return workLocations;
    }

    /// <summary>
    /// Called to complete the build job. Handles clearing the tree and updating the ground nodes to be walkable
    /// </summary>
    public override void complete() {
        base.complete();

        MapManager.Instance.removeMapNode(_nodeToReplace);
        MapManager.Instance.addMapNode(_finishedObject.GetComponent<Node>());

        _nodeToReplace.destroy();

        JobManager.Instance.checkBlockedJobs();
    }
}
