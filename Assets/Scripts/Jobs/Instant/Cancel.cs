using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cancel : Job {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Cancel(Node location) : base(location, 0f, 0f) {
        _title = "Cancel";
        _type = JOB_TYPE.Cancel;
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

    /// <summary>
    /// Called to see if this job is an instant build
    /// </summary>
    /// <returns><c>true</c>, if job should be completed instantly, <c>false</c> otherwise.</returns>
    public override bool isInstant() {
        return true;
    }

    /// <summary>
    /// Ises the valid location.
    /// </summary>
    /// <returns><c>true</c>, if valid location was ised, <c>false</c> otherwise.</returns>
    public override bool isValidLocation() {
        return (JobManager.Instance.getJobByLocation(_location.transform.position) != null);
    }

    /// <summary>
    /// Called to complete the build job. Handles clearing the tree and updating the ground nodes to be walkable
    /// </summary>
    public override void complete() {
        base.complete();

        // Calculate where to put new item
        Job job = JobManager.Instance.getJobByLocation(_location.transform.position);
        MapManager.Instance.setNodeMarker(job.getLocationNode(), false, Color.yellow, "");
        job.cancel();
    }
}
