using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildTunnel : Build {
    private Node _nodeToReplace;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public BuildTunnel(Node location, float duration, float progress) : base(location, duration, progress, ItemManager.Instance.tunnelBlock) {
        _title = "Build \nTunnel";
        _buildSubType = BUILD_SUB_TYPE.Tunnel;
        _nodeToReplace = location;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PUBLIC FUNCTIONS                                               ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Called to see if there is a job in the way
    /// </summary>
    /// <returns><c>true</c>, if job is in the way, <c>false</c> otherwise.</returns>
    /// <param name="pos">Position to check</param>
    private bool _isJobInWay(Vector3 pos) {
        Build job = (Build)JobManager.Instance.getJobByLocation(pos);
        return (job != null && job.getType() == JOB_TYPE.Build && job.getSubType() == BUILD_SUB_TYPE.Tunnel);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PUBLIC FUNCTIONS                                               ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets the action text.
    /// </summary>
    /// <returns>The action text.</returns>
    public override string getActionText() {
        return "Building tunnel";
    }

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
            if (node.transform.position == left || node.transform.position == right) {
                workLocations.Add(node);
            }
        }

        return workLocations;
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
                // Only consider nodes that are not left and right of root
                if (node.transform.position != left && node.transform.position != right) {
                    if (node.getType() == NODE_TYPE.Tunnel || node.getType() == NODE_TYPE.Room || _isJobInWay(node.transform.position)) {
                        isValid = false;
                        break;
                    }
                }
            }
        }
        return isValid;
    }

    /// <summary>
    /// Called to complete the build job. Handles clearing the tree and updating the ground nodes to be walkable
    /// </summary>
    public override void complete() {
        base.complete();

        MapManager.Instance.removeMapNode(_nodeToReplace);
        MapManager.Instance.addMapNode(_finishedObject.GetComponent<Node>());

        _nodeToReplace.mine();

        JobManager.Instance.checkBlockedJobs();
    }
}
