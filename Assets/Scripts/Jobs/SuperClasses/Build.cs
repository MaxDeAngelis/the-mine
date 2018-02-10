using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Build : Job {
    protected BUILD_SUB_TYPE _buildSubType;
    protected GameObject _finishedObject;
    protected Dictionary<RESOURCE_TYPE, int> _resourceCost = new Dictionary<RESOURCE_TYPE, int>();
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
    /// Gets the build subtype
    /// </summary>
    /// <returns>The sub type.</returns>
    public BUILD_SUB_TYPE getSubType() {
        return _buildSubType;
    }

    /// <summary>
    /// Called to check if the resources for this job pending
    /// </summary>
    /// <returns><c>true</c>, if resources are available, <c>false</c> otherwise.</returns>
    public bool isResourcesAvailable() {
        foreach(KeyValuePair<RESOURCE_TYPE, int> resource in _resourceCost) {
            if (MapManager.Instance.isResourceAvailable(resource.Key, resource.Value) == false) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Called to check if a job is in the way of the given location
    /// </summary>
    /// <returns><c>true</c>, if there is a job in the way, <c>false</c> otherwise.</returns>
    public bool isJobInWay(Vector3 pos, BUILD_SUB_TYPE subType) {
        Build job = (Build)JobManager.Instance.getJobByLocation(pos);
        if (subType != BUILD_SUB_TYPE.None) {
            return (job != null && job.getType() == JOB_TYPE.Build && job.getSubType() == subType);
        } else {
            return (job != null && job.getType() == JOB_TYPE.Build);
        }
    }

    /// <summary>
    /// Gets the resource cost of this job
    /// </summary>
    /// <returns>The resource cost</returns>
    public override Dictionary<RESOURCE_TYPE, int> getResourceCost() {
        return _resourceCost;
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

        foreach(KeyValuePair<RESOURCE_TYPE, int> resource in _resourceCost) {
            MapManager.Instance.useResource(resource.Key, resource.Value);
        }
    }
}
