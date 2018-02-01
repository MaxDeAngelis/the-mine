﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildRoom : Build {
    private Node _nodeToReplace;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public BuildRoom(Node location, float duration, float progress) : base(location, duration, progress, ItemLibrary.Instance.roomBlock) {
        _title = "Build \nRoom";
        _buildSubType = BUILD_SUB_TYPE.Room;
        _nodeToReplace = location;

        //_resourceCost.Add(RESOURCE_TYPE.Stone, 2);
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
    /// Called to complete the build job. Handles clearing the tree and updating the ground nodes to be walkable
    /// </summary>
    public override void complete() {
        base.complete();

        MapManager.Instance.removeMapNode(_nodeToReplace);
        MapManager.Instance.addMapNode(_finishedObject.GetComponent<Node>());

        _nodeToReplace.mine();

        JobManager.Instance.checkBlockedJobs();
    }

    /// <summary>
    /// Called to see if the current node a valid location for this job
    /// </summary>
    /// <returns><c>true</c>, if valid location, <c>false</c> otherwise.</returns>
    public override bool isValidLocation() {
        bool isValid = false;
        if (isResourcesAvailable()) {
            isValid = true;
            /*List<Node> locations = MapManager.Instance.getSurroundingNodes(_location);

            Vector3 top = _location.transform.position + Vector3.up;
            Vector3 bottom = _location.transform.position + Vector3.down;

            foreach (Node node in locations) {
                if (node.transform.position != top && node.transform.position != bottom) {
                    Build job = (Build)JobManager.Instance.getJobByLocation(node.transform.position);
                    if (node.getType() == NODE_TYPE.Shaft || (job != null && job.getType() == JOB_TYPE.Build && job.getSubType() == BUILD_SUB_TYPE.Shaft)) {
                        isValid = false;
                        break;
                    }
                }
            }*/
        }
        return isValid;
    }
}