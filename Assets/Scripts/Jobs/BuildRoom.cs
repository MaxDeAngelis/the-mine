using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildRoom : Build {
    private Node _nodeToReplace;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public BuildRoom(Node location, float duration, float progress) : base(location, duration, progress, ItemManager.Instance.roomBlock) {
        _title = "Build \nRoom";
        _buildSubType = BUILD_SUB_TYPE.Room;
        _nodeToReplace = location;

        //_resourceCost.Add(RESOURCE_TYPE.Stone, 2);
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
        return (job != null && job.getType() == JOB_TYPE.Build);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PUBLIC FUNCTIONS                                               ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets the action text.
    /// </summary>
    /// <returns>The action text.</returns>
    public override string getActionText() {
        return "Building room";
    }

    /// <summary>
    /// Gets the job selection constraints
    /// </summary>
    /// <returns>The selection constraints</returns>
    public override Vector2 getSelectionConstraints() {
        return new Vector2(-1f, 2f);
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

        Vector3 below = this.getLocation() + Vector3.down;

        foreach (Node node in potentialLocations) {
            if (node.transform.position == below) {
                workLocations.Add(node);
                break;
            }
        }

        return workLocations;
    }

    /// <summary>
    /// Called to see if the current node a valid location for this job
    /// </summary>
    /// <returns><c>true</c>, if valid location, <c>false</c> otherwise.</returns>
    public override bool isValidLocation() {
        bool isValid = false;
        if (isResourcesAvailable() && !_isJobInWay(this.getLocation())) {
            
            // Only allowed to build on or above a tunnel
            Node nodeBelow = MapManager.Instance.getNode(this.getLocation() + Vector3.down);
            if (_location.getType() == NODE_TYPE.Tunnel || nodeBelow.getType() == NODE_TYPE.Tunnel) {
                // Must have one tunnel space on each end
                Vector3 bottomLeftLoc = this.getLocation() + Vector3.left;
                Vector3 bottomRightLoc = this.getLocation() + Vector3.right;
                if (nodeBelow.getType() == NODE_TYPE.Tunnel) {
                    bottomLeftLoc += Vector3.down;
                    bottomRightLoc += Vector3.down;
                }
                Node bottomLeft = MapManager.Instance.getNode(bottomLeftLoc);
                Node bottomRight = MapManager.Instance.getNode(bottomRightLoc);
                if (bottomLeft != null && (bottomLeft.getType() == NODE_TYPE.Tunnel || bottomLeft.getType() == NODE_TYPE.Room) && 
                    bottomRight != null && (bottomRight.getType() == NODE_TYPE.Tunnel || bottomRight.getType() == NODE_TYPE.Room)) {
                    isValid = true;
                }
            } 

            // If there is a tunnel above the top then not valid
            if (nodeBelow.getType() == NODE_TYPE.Tunnel) {
                Vector3 above = this.getLocation() + Vector3.up;
                List<Node> aboveNodes = new List<Node>();
                aboveNodes.Add(MapManager.Instance.getNode(above));
                aboveNodes.Add(MapManager.Instance.getNode(above + Vector3.right));
                aboveNodes.Add(MapManager.Instance.getNode(above + Vector3.left));

                foreach (Node node in aboveNodes) {
                    if (node.getType() != NODE_TYPE.Stone || _isJobInWay(node.transform.position)) {
                        isValid = false;
                        break;
                    }
                }
            }
        }
        return isValid;
    }

    /// <summary>
    /// Get the minimum size of the job selection
    /// </summary>
    /// <returns>The selection minimum.</returns>
    public override Vector2 getSelectionMinimum() {
        return new Vector2(2f, 2f);
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
