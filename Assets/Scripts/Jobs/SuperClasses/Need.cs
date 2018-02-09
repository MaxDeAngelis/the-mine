using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Need : Job {
    protected NEED_TYPE _needSubType;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Need(float duration) : base(null, duration, 0f) {
        _title = "Need";
        _type = JOB_TYPE.Need;
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

        workLocations.Add(_location);

        return workLocations;
    }
}
