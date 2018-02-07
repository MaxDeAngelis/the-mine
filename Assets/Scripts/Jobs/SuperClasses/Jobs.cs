using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Job {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	protected string _title;
	protected JOB_TYPE _type;
	protected Node _location;
	protected float _duration;
	protected float _progress;
    private List<Unit> _workers = new List<Unit>();




	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTRUCTOR												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public Job(Node location, float duration, float progress) {
		_location = location;
		_duration = duration;
		_progress = progress;
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                            PRIVATE FUNCTIONS                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	/// <summary>
	/// Called to get the type of Job
	/// </summary>
	/// <returns>The type</returns>
	public JOB_TYPE getType() {
		return _type;
	}

    /// <summary>
    /// Adds a worker to this job
    /// </summary>
    /// <param name="worker">Worker.</param>
    public void addWorker(Unit worker) {
        _workers.Add(worker);
    }

    /// <summary>
    /// Removes a worker from this job
    /// </summary>
    /// <param name="worker">Worker.</param>
    public void removeWorker(Unit worker) {
        _workers.Remove(worker);
    }

    /// <summary>
    /// Get the workers for this job
    /// </summary>
    /// <returns>The workers</returns>
    public List<Unit> getWorkers() {
        return _workers;
    }

    /// <summary>
    /// Cancels this job by telling workers to stop working
    /// </summary>
    public void cancel() {
        foreach(Unit worker in _workers) {
            worker.cancelJob();
        }
        JobManager.Instance.cancelJob(this);
    }

	/// <summary>
	/// Called to get the Title of the Job
	/// </summary>
	/// <returns>The title of the Job</returns>
	public string getTitle() {
		return _title;
	}

	/// <summary>
	/// Called to get the duration of the job
	/// </summary>
	/// <returns>The duration</returns>
	public float getDuration() {
		return _duration;
	}

	/// <summary>
	/// Called to get the current progress of the job
	/// </summary>
	/// <returns>The progress</returns>
	public float getProgress() {
		return _progress;
	}

    /// <summary>
    /// Gets the location node.
    /// </summary>
    /// <returns>The location node.</returns>
    public Node getLocationNode() {
        return _location;
    }

    /// <summary>
    /// Gets the location.
    /// </summary>
    /// <returns>The location.</returns>
	public Vector3 getLocation() {
		return _location.transform.position;
	}

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                            VIRTUAL FUCTIONS                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets the action text.
    /// </summary>
    /// <returns>The action text.</returns>
    public virtual string getActionText() {
        return "";
    }

    /// <summary>
    /// Gets the job selection constraints.
    /// </summary>
    /// <returns>The selection constraints.</returns>
    public virtual Vector2 getSelectionConstraints() {
        return new Vector2(-1f, -1f);
    }

    /// <summary>
    /// Get the minimum size of the job selection
    /// </summary>
    /// <returns>The selection minimum.</returns>
    public virtual Vector2 getSelectionMinimum() {
        return new Vector2(1f, 1f);
    }

    /// <summary>
    /// Get the resource cost of this job
    /// </summary>
    /// <returns>The resource cost</returns>
    public virtual Dictionary<RESOURCE_TYPE, int> getResourceCost() {
        return new Dictionary<RESOURCE_TYPE, int>();
    }

    /// <summary>
    /// Ises the valid location.
    /// </summary>
    /// <returns><c>true</c>, if valid location was ised, <c>false</c> otherwise.</returns>
    public virtual bool isValidLocation() {
        return true;
    }

    /// <summary>
    /// Called to see if this job is an instant build
    /// </summary>
    /// <returns><c>true</c>, if job should be completed instantly, <c>false</c> otherwise.</returns>
    public virtual bool isInstant() {
        return false;
    }

	/// <summary>
	/// Called to get all the possable work locations for a given job
	/// </summary>
	/// <returns>The list of nodes that the Job can be completed from</returns>
	public virtual List<Node> getWorkLocations() {
		return null;
	}

	/// <summary>
	/// Called when the work is complete
	/// </summary>
	public virtual void complete() { 
		// Clear marker from the location node
        JobManager.Instance.finishJob(this);
	}
}
