using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public float movementSpeed = 0.5f;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private UNIT_STATE _state = UNIT_STATE.Idle;
	private Job _currentJob;
	private PathFinder _pathFinder = new PathFinder();
	private Node _nextPathNode;
    private IEnumerator _doJobCoroutine;
	// TODO: will want some concept of normal speed for walk penilties 
	// private float _normalMovementSpeed;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called when the Game Object is started
	/// </summary>
	private void Start () {

	}

	/// <summary>
	/// Called once per frame at a consistent 60FPS
	/// </summary>
	private void FixedUpdate () {
		if (isWorking()) {
			return;
        } else if (isIdle()) {
            Node currentNode = MapManager.Instance.getNode(transform.position);
            _currentJob = JobManager.Instance.getJob(currentNode);

			if (_currentJob != null) {
                _currentJob.addWorker(this);
				_state = UNIT_STATE.Busy;
			}
		} else if (isMoving()) {
			Vector3 target = _nextPathNode.transform.position;
			Vector3 dir = (target - transform.position).normalized;		
			dir *= movementSpeed * Time.deltaTime;									
			GetComponent<Rigidbody2D>().velocity = dir;	

			if (Vector3.Distance (transform.position, target) < 0.05f) {
				transform.position = target;
				if (_pathFinder.isEmpty()) {
					GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
					_pathFinder.nullify();
                    _state = UNIT_STATE.Working;
                    _doJobCoroutine = _doJob();
                    StartCoroutine(_doJobCoroutine);
				} else {
					_nextPathNode = _pathFinder.getNextNode();
				}
			}
		} else if (_currentJob != null) {
			List<Node> jobWorkLocations = _currentJob.getWorkLocations();

			// If the Job has work nodes then find the best path
			if (jobWorkLocations.Count > 0) {
				_state = UNIT_STATE.Moving;

				foreach(Node location in jobWorkLocations) {
					_pathFinder.findPath(_getNodeStandingOn(), location);
				}
			} 

			// If no path was found then return job
			if (_pathFinder.isEmpty()) {
				_state = UNIT_STATE.Idle;

                _currentJob.removeWorker(this);
				JobManager.Instance.blockJob(_currentJob);
			} else {
				/* DEBUG */ _pathFinder.highlight();
				_nextPathNode = _pathFinder.getNextNode();
			}
		}
	}

	/// <summary>
	/// Called to find the ground node that the unit is standing on
	/// </summary>
	/// <returns>The node that the unit is standing on</returns>
	private Node _getNodeStandingOn() {
		Vector3 locationStandingOn = transform.position;
		locationStandingOn.x = Mathf.RoundToInt(locationStandingOn.x);
		locationStandingOn.y = Mathf.RoundToInt(locationStandingOn.y);
		locationStandingOn.z = 0;

		return MapManager.Instance.getNode(locationStandingOn);
	}

    /// <summary>
    /// Called to actually do the work of the current Job
    /// </summary>
    /// <returns>The job.</returns>
    private IEnumerator _doJob() {
        Vector3 target = _currentJob.getLocation();
        target.y = transform.position.y;

        // Yield for the duration of the boost
        yield return new WaitForSeconds(_currentJob.getDuration());

        if (_currentJob != null) {
            _currentJob.complete();
            _currentJob = null;
        }
        _state = UNIT_STATE.Idle;

    }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to see if the unit is idle
	/// </summary>
	/// <returns><c>true</c>, if unit is idle, <c>false</c> otherwise.</returns>
	public bool isIdle() {
		return (_state == UNIT_STATE.Idle);
	}
	/// <summary>
	/// Called to see if the unit is moving
	/// </summary>
	/// <returns><c>true</c>, if unit is moving, <c>false</c> otherwise.</returns>
	public bool isMoving() {
		return (_state == UNIT_STATE.Moving);
	}

	/// <summary>
	/// Called to check if the unit is currently working
	/// </summary>
	/// <returns><c>true</c>, if working, <c>false</c> otherwise.</returns>
	public bool isWorking() {
		return (_state == UNIT_STATE.Working);
	}

    public void cancelJob() {
        _currentJob = null;
        _state = UNIT_STATE.Idle;
        StopCoroutine(_doJobCoroutine);
    }
}
