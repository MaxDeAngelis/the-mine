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
	private List<Job> _availableJobs = new List<Job>();	
	private List<Job> _blockedJobs = new List<Job>();

	private JOB_TYPE _commandType;
	private BUILD_SUB_TYPE _buildSubType;

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

		switch(_commandType) {
            case JOB_TYPE.Build:
                shouldHighlight = true;
                break;
    		case JOB_TYPE.Move:
                if (node.isTravelable()) {
    				shouldHighlight = true;
    			}
    			break;
		}

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
				while(true) {
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
			_availableJobs.Add(newJob);
		}
		/* DEBUG */ //GameManager.Instance.updateAvailableJobs(_availableJobs);
		/* DEBUG */ //GameManager.Instance.clearAllNodeColor();
	}

	/// <summary>
	/// Create a job for the given node
	/// </summary>
	/// <param name="node">The node</param>
	private void _createJob(Node node) {
        MapManager.Instance.setNodeMarker(node, false, Color.green, "");
		Job newJob = null;
		switch(_commandType) {
            case JOB_TYPE.Build:
                switch (_buildSubType) {
                    case BUILD_SUB_TYPE.Tunnel:
                        newJob = new BuildTunnel(node, 3, 0);
                        break;
                    case BUILD_SUB_TYPE.Shaft:
                        newJob = new BuildShaft(node, 3, 0);
                        break;
                }
                break;
    		case JOB_TYPE.Move:
    			if (node.isWalkable()) {
    				newJob = new Move(node, 0, 0);
    			}
    			break;
		}
        MapManager.Instance.setNodeMarker(node, true, Color.yellow, newJob.getTitle());
		_registerJob(newJob);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to add the given job to the blocked list
	/// </summary>
	/// <param name="job">The Job</param>
	public void blockJob(Job job) {
		_blockedJobs.Add(job);
	}

    /// <summary>
    /// Called periodically to check if any of the blocked Jobs are now available
    /// </summary>
    public void checkBlockedJobs() {
        List<Job> unblockedJobs = new List<Job>();

        // Loop over blocked Jobs to find the ones to migrate
        foreach(Job job in _blockedJobs) {
            List<Node> jobWorkLocations = job.getWorkLocations();

            if (jobWorkLocations.Count > 0) {
                unblockedJobs.Add(job);
            }
        }

        // Migrate to available
        foreach(Job job in unblockedJobs) {
            _blockedJobs.Remove(job);
            _availableJobs.Add(job);
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
            foreach (Job job in _availableJobs) {
                List<Node> workLocations = job.getWorkLocations();
                foreach (Node node in workLocations) {
                    _pathFinder.findPath(currentNode, node);
                }
                // Find best job for current 
                if (!_pathFinder.isEmpty() && (jobDistance == -1 || _pathFinder.getLength() < jobDistance)) {
                    returnJob = job;
                }
            }
			_availableJobs.Remove(returnJob);

			/* DEBUG */ //GameManager.Instance.updateAvailableJobs(_availableJobs);
		}

		return returnJob;
	}

	/// <summary>
	/// Called to set the command type that is about to be issued
	/// </summary>
	/// <param name="type">Type of job</param>
	public void setCommandType(JOB_TYPE type) {
		_commandType = type;
	}

	public void setBuildSubType(BUILD_SUB_TYPE subType) {
		_buildSubType = subType;
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
			_createJob(singleNode);
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
            case JOB_TYPE.Build:
                if (_buildSubType == BUILD_SUB_TYPE.Shaft || _buildSubType == BUILD_SUB_TYPE.Tunnel) {
                }
                _isMultiSelect = true;  
                _multiSelectStart = node;
                break;
    		case JOB_TYPE.Move:
    			break;
    		default:
    			_isMultiSelect = true;	
    			_multiSelectStart = node;
    			break;
		}
	}
}
