using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceMiner : Build {
    private Node _nodeToReplace;
    public PlaceMiner(Node location) : base(location, 0f, 0f, ItemLibrary.Instance.miner) {
        _title = "Place \nMiner";
    }

    /// <summary>
    /// Overridden version of getWorkLocation to return the correct locations
    /// </summary>
    /// <returns>The list of nodes that the Job can be completed from</returns>
    public override List<Node> getWorkLocations() {
        List<Node> workLocations = new List<Node>();
        workLocations.Add(_location);

        return workLocations;
    }

    public override bool isInstant() {
        return true;
    }
}
