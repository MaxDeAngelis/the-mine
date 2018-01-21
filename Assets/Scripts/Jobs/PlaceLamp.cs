using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceLamp : Build {
    public PlaceLamp(Node location, float duration, float progress) : base(location, duration, progress, ItemLibrary.Instance.lamp) {
        _title = "Place \nLamp";
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
}
