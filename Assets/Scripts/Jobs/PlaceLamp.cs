using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceLamp : Build {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public PlaceLamp(Node location, float duration, float progress) : base(location, duration, progress, ItemManager.Instance.lamp) {
        _title = "Place \nLamp";

        _resourceCost.Add(RESOURCE_TYPE.Iron, 1);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PUBLIC FUNCTIONS                                               ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Gets the action text.
    /// </summary>
    /// <returns>The action text.</returns>
    public override string getActionText() {
        return "Placing lamp";
    }

    /// <summary>
    /// Overridden version of getWorkLocation to return the correct locations
    /// </summary>
    /// <returns>The list of nodes that the Job can be completed from</returns>
    public override List<Node> getWorkLocations() {
        List<Node> workLocations = new List<Node>();
        workLocations.Add(_location);

        Node nodeBelow = MapManager.Instance.getNode(this.getLocation() + Vector3.down);
        if (nodeBelow != null && nodeBelow.isWalkable()) {
            workLocations.Add(nodeBelow);
        }

        return workLocations;
    }

    /// <summary>
    /// Ises the valid location.
    /// </summary>
    /// <returns><c>true</c>, if valid location was ised, <c>false</c> otherwise.</returns>
    public override bool isValidLocation() {
        if (isResourcesAvailable()) {
            Job job = JobManager.Instance.getJobByLocation(_location.transform.position);
            if ((_location.getType() == NODE_TYPE.Tunnel || _location.getType() == NODE_TYPE.Room) &&
            ItemManager.Instance.getItem(_location.transform.position) == null &&
            job == null) {
                return true;
            }
        }

        return false;
    }
}
