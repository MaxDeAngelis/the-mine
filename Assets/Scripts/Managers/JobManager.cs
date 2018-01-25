using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class JobManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static JobManager Instance;
	public bool isCommandSelected = false;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Dictionary<Vector3, Job> _availableJobs = new Dictionary<Vector3, Job>();	
    private Dictionary<Vector3, Job> _blockedJobs = new Dictionary<Vector3, Job>();
    private Dictionary<Vector3, Job> _inProgressJobs = new Dictionary<Vector3, Job>();

	private JOB_TYPE _commandType;
	private BUILD_SUB_TYPE _buildSubType;
    private PLACE_SUB_TYPE _placeSubType;

    private PathFinder _pathFinder = new PathFinder();
	private bool _isMouseDown = false;
	private bool _isMultiSelect = false;
	private Node _multiSelectStart;
	private Node _hoveredNode;
	private List<Node> _selectedNodes = new List<Node>();

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on Awake of the Game Object
	/// </summary>
	private void Awake () {
		if (Instance != null)
		{
			Debug.LogError("Multiple instances of JobManager!");
		}
		Instance = this;

		InvokeRepeating("checkBlockedJobs", 5f, 5f);
	}

	/// <summary>
	/// Called to check if the current node should be highlighted for the job type
	/// </summary>
	/// <returns><c>true</c>, if node should be highlighted for the job type, <c>false</c> otherwise</returns>
	/// <param name="node">The node to check</param>
	private bool _shouldNodeHighlight(Node node) {
        bool shouldHighlight = false;

        // If there is no job defined
        // FIXME need to find a better way of doing this
        //if (isJobDefined(node.transform.position) == false) {
            // Create temp job and check if valid node
            Job tempJob = _createJob(node);
            if (tempJob != null) {
                shouldHighlight = tempJob.isValidLocation();
            }
        //}

		return shouldHighlight;
	}

	/// <summary>
	/// Called to show job selection for multi select jobs and single select
	/// </summary>
	private void _showJobSelection() {
		// Always start by clearing the selected nodes 
		foreach(Node node in _selectedNodes) {
            MapManager.Instance.setNodeMarker(node, false, Color.green, "");
		}
		_selectedNodes.Clear();

		// If multi select job then loop to highlight all nodes
		if (_isMultiSelect) {
            Job tempJob = _createJob(_multiSelectStart);
            Vector2 constraints = tempJob.getSelectionConstraints();

			// Get a reference to the X,Z locations of the nodes on the map
			float startX = _multiSelectStart.transform.position.x;
			float endX = _hoveredNode.transform.position.x;
			float startY = _multiSelectStart.transform.position.y;
			float endY = _hoveredNode.transform.position.y;
            float tempX = startX;
			// Start looping for X values
			while(true) {
				float tempY = startY;
				// Loop over Z values

                if (constraints.x != -1f && constraints.x == Mathf.Abs(tempX - startX)) {
                    break;
                }
				while(true) {
                    if (constraints.y != -1f && constraints.y == Mathf.Abs(tempY - startY)) {
                        break;
                    }
					// Get the node based on calculated location
                    Node node = MapManager.Instance.getNode(new Vector3(tempX, tempY, 0f));
					if (node != null && _shouldNodeHighlight(node)) {
						_selectedNodes.Add(node);
					}
					if (tempY == endY) {
						break;
					} else if (startY > endY) {
						tempY--;
					} else {
						tempY++;
					}
				}
                if (tempX == endX) {
					break;
				} else if (startX > endX) {
                    tempX--;
				} else {
                    tempX++;
				}
			}
		} else if (_shouldNodeHighlight(_hoveredNode)) {
			// If sindle select then just add the hovered node to selected nodes
			_selectedNodes.Add(_hoveredNode);
		}

		// Highlight all the selected nodes
		foreach(Node node in _selectedNodes) {
            MapManager.Instance.setNodeMarker(node, true, Color.green, "");
		}
	}

	/// <summary>
	/// Called to register a new Job
	/// </summary>
	/// <param name="newJob">New job.</param>
	private void _registerJob(Job newJob) {
		if (newJob != null) {
            _availableJobs.Add(newJob.getLocation(), newJob);
		}
		/* DEBUG */ //GameManager.Instance.updateAvailableJobs(_availableJobs);
		/* DEBUG */ //GameManager.Instance.clearAllNodeColor();
	}

	/// <summary>
	/// Create a job for the given node
	/// </summary>
	/// <param name="node">The node</param>
	private Job _createJob(Node node) {
		Job newJob = null;
		switch(_commandType) {
            case JOB_TYPE.Place:
                switch (_placeSubType) {
                    case PLACE_SUB_TYPE.Miner:
                        newJob = new PlaceMiner(node);
                        break;
                    case PLACE_SUB_TYPE.Lamp:
                        newJob = new PlaceLamp(node, 2, 0);
                        break;
                }
                break;
            case JOB_TYPE.Build:
                switch (_buildSubType) {
                    case BUILD_SUB_TYPE.Tunnel:
                        newJob = new BuildTunnel(node, 3, 0);
                        break;
                    case BUILD_SUB_TYPE.Shaft:
                        newJob = new BuildShaft(node, 4, 0);
                        break;
                }
                break;
    		case JOB_TYPE.Move:
    			newJob = new Move(node);
    			break;
            case JOB_TYPE.Cancel:
                newJob = new Cancel(node);
                break;
		}

        return newJob;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to add the given job to the blocked list
	/// </summary>
	/// <param name="job">The Job</param>
	public void blockJob(Job job) {
        _blockedJobs.Add(job.getLocation(), job);
        if (_inProgressJobs.ContainsKey(job.getLocation())) {
            _inProgressJobs.Remove(job.getLocation());
        }
	}

    /// <summary>
    /// Remove the finished job from the in progress list
    /// </summary>
    /// <param name="job">Job</param>
    public void finishJob(Job job) {
        if (job.getType() != JOB_TYPE.Cancel) {
            _inProgressJobs.Remove(job.getLocation());
        }
    }

    public void cancelJob(Job job) {
        if (_availableJobs.ContainsKey(job.getLocation())) {
            _availableJobs.Remove(job.getLocation());
        }

        if (_blockedJobs.ContainsKey(job.getLocation())) {
            _blockedJobs.Remove(job.getLocation());
        }

        if (_inProgressJobs.ContainsKey(job.getLocation())) {
            _inProgressJobs.Remove(job.getLocation());
        }
    }

    public bool isJobDefined(Vector3 loc) {
        return (_blockedJobs.ContainsKey(loc) || _availableJobs.ContainsKey(loc) || _inProgressJobs.ContainsKey(loc));
    }

    /// <summary>
    /// Called periodically to check if any of the blocked Jobs are now available
    /// </summary>
    public void checkBlockedJobs() {
        List<Job> unblockedJobs = new List<Job>();

        // Loop over blocked Jobs to find the ones to migrate
        foreach(Job job in _blockedJobs.Values) {
            List<Node> jobWorkLocations = job.getWorkLocations();

            if (jobWorkLocations.Count > 0) {
                unblockedJobs.Add(job);
            }
        }

        // Migrate to available
        foreach(Job job in unblockedJobs) {
            _blockedJobs.Remove(job.getLocation());
            _availableJobs.Add(job.getLocation(), job);
        }

        unblockedJobs.Clear();
        /* DEBUG */ //GameManager.Instance.updateAvailableJobs(_availableJobs);
    }

	/// <summary>
	/// Called to get a Job
	/// </summary>
	/// <returns>The job</returns>
    public Job getJob(Node currentNode) {
		Job returnJob = null;
        int jobDistance = -1;
		if (_availableJobs.Count > 0) {
            foreach (Job job in _availableJobs.Values) {
                List<Node> workLocations = job.getWorkLocations();
                foreach (Node node in workLocations) {
                    _pathFinder.findPath(currentNode, node);
                }
                // Find best job for current 
                if (!_pathFinder.isEmpty() && (jobDistance == -1 || _pathFinder.getLength() < jobDistance)) {
                    returnJob = job;
                }
            }

            // If a job was found then add to in progress list
            if (returnJob != null) {
                _availableJobs.Remove(returnJob.getLocation());
                _inProgressJobs.Add(returnJob.getLocation(), returnJob);
            }
			/* DEBUG */ //GameManager.Instance.updateAvailableJobs(_availableJobs);
		}

		return returnJob;
	}

    public Job getJobByLocation(Vector3 location) {
        if (_availableJobs.ContainsKey(location)) {
            return _availableJobs[location];
        }

        if (_blockedJobs.ContainsKey(location)) {
            return _blockedJobs[location];
        }

        if (_inProgressJobs.ContainsKey(location)) {
            return _inProgressJobs[location];
        }

        return null;
    }

	/// <summary>
	/// Called to set the command type that is about to be issued
	/// </summary>
	/// <param name="type">Type of job</param>
	public void setCommandType(JOB_TYPE type) {
		_commandType = type;
	}

    /// <summary>
    /// Called to set the build sub type command
    /// </summary>
    /// <param name="subType">Sub type.</param>
	public void setBuildSubType(BUILD_SUB_TYPE subType) {
		_buildSubType = subType;
	}

    /// <summary>
    /// Called to set the place sub type
    /// </summary>
    /// <param name="subType">Sub type.</param>
    public void setPlaceSubType(PLACE_SUB_TYPE subType) {
        _placeSubType = subType;
    }

	/// <summary>
	/// Called to handle mouse enter of a map node
	/// </summary>
	/// <param name="node">Map node</param>
	public void handleMouseEnterNode(Node node) {
		if (!isCommandSelected) { return; }

		_hoveredNode = node;
		_showJobSelection();
	}

	/// <summary>
	/// Called to handle mouse exit of a map node
	/// </summary>
	/// <param name="node">Map node</param>
	public void handleMouseExitNode(Node node) {
		if (!isCommandSelected) { return; }

		_hoveredNode = null;

		// If your mouse leaves the current node and you are not a multi select job then cancel 
		if (!_isMultiSelect && _isMouseDown) {
			isCommandSelected = false;
			_isMouseDown = false;
		}
	}

	/// <summary>
	/// Called to handle mouse up on a map node
	/// </summary>
	/// <param name="node">Map node</param>
	public void handleMouseUpNode(Node node) {
		if (!isCommandSelected) { return; }

		// Loop over all the selected nodes and create the appropriate jobs
		foreach(Node singleNode in _selectedNodes) {
            MapManager.Instance.setNodeMarker(singleNode, false, Color.green, "");

			Job newJob = _createJob(singleNode);

            // If a Job was found
            if (newJob != null) {
                if (newJob.isInstant()) {
                    newJob.complete();
                } else {
                    MapManager.Instance.setNodeMarker(singleNode, true, Color.yellow, newJob.getTitle());
                    _registerJob(newJob);
                }
            }
		}
		_selectedNodes.Clear();
		isCommandSelected = false;
		_isMouseDown = false;
		_isMultiSelect = false;
		_multiSelectStart = null;
		_hoveredNode = null;
	}

	/// <summary>
	/// Called to handle mouse udown on a map node
	/// </summary>
	/// <param name="node">Map node</param>
	public void handleMouseDownNode(Node node) {
		if (!isCommandSelected) { return; }

		_isMouseDown = true;
		switch(_commandType) {
            case JOB_TYPE.Place:
    		case JOB_TYPE.Move:
    			break;
            case JOB_TYPE.Build:
            case JOB_TYPE.Cancel:
    		default:
    			_isMultiSelect = true;	
    			_multiSelectStart = node;
    			break;
		}
	}
}
