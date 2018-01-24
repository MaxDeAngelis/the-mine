using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildTunnel : Build {
    private Node _nodeToReplace;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public BuildTunnel(Node location, float duration, float progress) : base(location, duration, progress, ItemLibrary.Instance.tunnelBlock) {
        _title = "Build \nTunnel";
        _nodeToReplace = location;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PUBLIC FUNCTIONS                                               ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the job selection constraints.
    /// </summary>
    /// <returns>The selection constraints.</returns>
    public override Vector2 getSelectionConstraints() {
        return new Vector2(-1f, 1f);
    }

    /// <summary>
    /// Overridden version of getWorkLocation to return the correct locations
    /// </summary>
    /// <returns>The list of nodes that the Job can be completed from</returns>
    public override List<Node> getWorkLocations() {
        List<Node> potentialLocations = MapManager.Instance.getSurroundingNodes(_location, false);
        List<Node> workLocations = new List<Node>();

        Vector3 left = _location.transform.position + Vector3.left;
        Vector3 right = _location.transform.position + Vector3.right;

        foreach (Node node in potentialLocations) {
            if (node.isTravelable() && (node.transform.position == left || node.transform.position == right)) {
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

    /// <summary>
    /// Called to see if this is a valid location for this job to be completed
    /// </summary>
    /// <returns><c>true</c>, if valid location, <c>false</c> otherwise.</returns>
    public override bool isValidLocation() {
        bool isValid = false;
        if (_location.getType() == NODE_TYPE.Stone) {
            isValid = true;
            List<Node> locations = MapManager.Instance.getSurroundingNodes(_location);

            Vector3 left = _location.transform.position + Vector3.left;
            Vector3 right = _location.transform.position + Vector3.right;

            foreach (Node node in locations) {
                if (node.transform.position != left && node.transform.position != right) {
                    if (node.getType() == NODE_TYPE.Tunnel) {
                        isValid = false;
                        break;
                    }
                }
            }
        }
        return isValid;
    }
}
