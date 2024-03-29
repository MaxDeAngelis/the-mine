﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobManager : MonoBehaviour {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PUBLIC VARIABLES											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static JobManager Instance;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PRIVATE VARIABLES											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Dictionary<Vector3, Job> _availableJobs = new Dictionary<Vector3, Job> ();
    private Dictionary<Vector3, Job> _blockedJobs = new Dictionary<Vector3, Job> ();
    private Dictionary<Vector3, Job> _inProgressJobs = new Dictionary<Vector3, Job> ();
    private JobFactory _jobFactory;
    private PathFinder _pathFinder = new PathFinder ();
    private bool _isMouseDown = false;
    private Node _multiSelectStart;
    private Node _hoveredNode;
    private List<Node> _selectedNodes = new List<Node> ();
    private Dictionary<RESOURCE_TYPE, int> _selectedResourceCost = new Dictionary<RESOURCE_TYPE, int> ();

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PRIVATE FUNCTIONS											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Called on Awake of the Game Object
    /// </summary>
    private void Awake () {
        if (Instance != null) {
            Debug.LogError ("Multiple instances of JobManager!");
        }
        Instance = this;

        InvokeRepeating ("checkBlockedJobs", 5f, 5f);
    }

    /// <summary>
    /// Called to check if the current node should be highlighted for the job type
    /// </summary>
    /// <returns><c>true</c>, if node should be highlighted for the job type, <c>false</c> otherwise</returns>
    /// <param name="node">The node to check</param>
    private bool _shouldNodeHighlight (Node node) {
        bool shouldHighlight = false;

        // If there is no job defined
        // FIXME need to find a better way of doing this
        //if (isJobDefined(node.transform.position) == false) {
        // Create temp job and check if valid node
        Job tempJob = _jobFactory.createJob (node);
        if (tempJob != null) {
            shouldHighlight = tempJob.isValidLocation ();
        }
        //}
        return shouldHighlight;
    }

    /// <summary>
    /// Called to show job selection for multi select jobs and single select
    /// </summary>
    private void _showJobSelection () {
        // Always start by clearing the selected nodes 
        foreach (Node node in _selectedNodes) {
            MapManager.Instance.setNodeMarker (node, false, Color.green, "");
        }

        // Clear the selection befor re calculating
        _selectedNodes.Clear ();
        _selectedResourceCost.Clear ();

        // Get a temp job for selection
        Job tempJob = _jobFactory.createJob (_multiSelectStart);

        // Store off locations for calculating
        Vector2 start = new Vector2 (_multiSelectStart.transform.position.x, _multiSelectStart.transform.position.y);
        Vector2 end = new Vector2 (_hoveredNode.transform.position.x, _hoveredNode.transform.position.y);
        Vector2 current = new Vector2 (start.x, start.y);

        // Also store off constraint info
        Vector2 constraints = tempJob.getSelectionConstraints ();
        Vector2 minimum = tempJob.getSelectionMinimum ();
        Vector2 difference = new Vector2 (Mathf.Abs (end.x - start.x) + 1, Mathf.Abs (end.y - start.y) + 1);

        // Calculate the difference
        if (difference.x < minimum.x) {
            end.x += (minimum.x - difference.x);
        }

        // Need to convert to one based for comparison
        if (difference.y < minimum.y) {
            end.y += (minimum.y - difference.y);
        }

        // Start looping for X values
        while (true) {
            // Need to reset y for each loop
            current.y = start.y;

            // Loop over x values unless hitting the constraints of the job
            if (constraints.x != -1f && constraints.x == Mathf.Abs (current.x - start.x)) {
                break;
            }
            while (true) {
                // if the y constraint is met then fall out
                if (constraints.y != -1f && constraints.y == Mathf.Abs (current.y - start.y)) {
                    break;
                }
                // Get the node based on calculated location
                Node node = MapManager.Instance.getNode (new Vector3 (current.x, current.y, 0f));
                if (node != null && _shouldNodeHighlight (node)) {
                    bool isResourcesAvailable = true;
                    // Check the combined resources to see if there is enough to build multiple jobs
                    Dictionary<RESOURCE_TYPE, int> cost = tempJob.getResourceCost ();
                    foreach (KeyValuePair<RESOURCE_TYPE, int> resource in cost) {
                        // If the key does not exist we need to add it
                        if (_selectedResourceCost.ContainsKey (resource.Key)) {
                            _selectedResourceCost[resource.Key] += resource.Value;
                        } else {
                            _selectedResourceCost.Add (resource.Key, resource.Value);
                        }

                        if (MapManager.Instance.isResourceAvailable (resource.Key, _selectedResourceCost[resource.Key]) == false) {
                            isResourcesAvailable = false;
                            break;
                        }
                    }

                    if (isResourcesAvailable) {
                        _selectedNodes.Add (node);
                    }
                }
                if (current.y == end.y) {
                    break;
                } else if (start.y > end.y) {
                    current.y--;
                } else {
                    current.y++;
                }
            }
            if (current.x == end.x) {
                break;
            } else if (start.x > end.x) {
                current.x--;
            } else {
                current.x++;
            }
        }

        // Highlight all the selected nodes
        foreach (Node node in _selectedNodes) {
            MapManager.Instance.setNodeMarker (node, true, Color.green, "");
        }
    }

    /// <summary>
    /// Called to register a new Job
    /// </summary>
    /// <param name="newJob">New job.</param>
    private void _registerJob (Job newJob) {
        if (newJob != null) {
            _availableJobs.Add (newJob.getLocation (), newJob);

            // If the job is a build job then need to mark the resources as needed
            if (newJob.GetType () == typeof (Build)) {
                Dictionary<RESOURCE_TYPE, int> cost = ((Build) newJob).getResourceCost ();
                foreach (KeyValuePair<RESOURCE_TYPE, int> resource in cost) {
                    MapManager.Instance.earMarkResource (resource.Key, resource.Value);
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PUBLIC FUNCTIONS											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Called to add the given job to the blocked list
    /// </summary>
    /// <param name="job">The Job</param>
    public void blockJob (Job job) {
        _blockedJobs.Add (job.getLocation (), job);
        if (_inProgressJobs.ContainsKey (job.getLocation ())) {
            _inProgressJobs.Remove (job.getLocation ());
        }
    }

    /// <summary>
    /// Called to return the given job to the blocked list
    /// </summary>
    /// <param name="job">The Job</param>
    public void returnJob (Job job) {
        _availableJobs.Add (job.getLocation (), job);
        if (_inProgressJobs.ContainsKey (job.getLocation ())) {
            _inProgressJobs.Remove (job.getLocation ());
        }
        MapManager.Instance.setNodeMarker (job.getLocationNode (), true, Color.yellow, job.getTitle ());
    }

    /// <summary>
    /// Remove the finished job from the in progress list
    /// </summary>
    /// <param name="job">Job</param>
    public void finishJob (Job job) {
        if (job.GetType () != typeof (Cancel)) {
            _inProgressJobs.Remove (job.getLocation ());
        }
    }

    /// <summary>
    /// Cancel the given job
    /// </summary>
    /// <param name="job">Job to cancel</param>
    public void cancelJob (Job job) {
        if (_availableJobs.ContainsKey (job.getLocation ())) {
            _availableJobs.Remove (job.getLocation ());
        }

        if (_blockedJobs.ContainsKey (job.getLocation ())) {
            _blockedJobs.Remove (job.getLocation ());
        }

        if (_inProgressJobs.ContainsKey (job.getLocation ())) {
            _inProgressJobs.Remove (job.getLocation ());
        }
    }

    /// <summary>
    /// Called to check if there is a job at the goven location
    /// </summary>
    /// <returns><c>true</c>, if job is defined, <c>false</c> otherwise.</returns>
    /// <param name="loc">Location to check</param>
    public bool isJobDefined (Vector3 loc) {
        return (_blockedJobs.ContainsKey (loc) || _availableJobs.ContainsKey (loc) || _inProgressJobs.ContainsKey (loc));
    }

    /// <summary>
    /// Called periodically to check if any of the blocked Jobs are now available
    /// </summary>
    public void checkBlockedJobs () {
        List<Job> unblockedJobs = new List<Job> ();

        // Loop over blocked Jobs to find the ones to migrate
        foreach (Job job in _blockedJobs.Values) {
            List<Node> jobWorkLocations = job.getWorkLocations ();

            if (jobWorkLocations.Count > 0) {
                unblockedJobs.Add (job);
            }
        }

        // Migrate to available
        foreach (Job job in unblockedJobs) {
            _blockedJobs.Remove (job.getLocation ());
            _availableJobs.Add (job.getLocation (), job);
        }

        unblockedJobs.Clear ();
    }

    /// <summary>
    /// Called to get a Job
    /// </summary>
    /// <returns>The job</returns>
    public Job getJob (Node currentNode) {
        Job returnJob = null;
        int jobDistance = -1;
        if (_availableJobs.Count > 0) {
            foreach (Job job in _availableJobs.Values) {
                List<Node> workLocations = job.getWorkLocations ();
                foreach (Node node in workLocations) {
                    _pathFinder.findPath (currentNode, node);
                }
                // Find best job for current 
                if (!_pathFinder.isEmpty () && (jobDistance == -1 || _pathFinder.getLength () < jobDistance)) {
                    returnJob = job;
                }
            }

            // If a job was found then add to in progress list
            if (returnJob != null) {
                _availableJobs.Remove (returnJob.getLocation ());
                _inProgressJobs.Add (returnJob.getLocation (), returnJob);
            }
        }

        return returnJob;
    }

    /// <summary>
    /// Gets a job based on the location given 
    /// </summary>
    /// <returns>The job at the given location</returns>
    /// <param name="location">Location</param>
    public Job getJobByLocation (Vector3 location) {
        if (_availableJobs.ContainsKey (location)) {
            return _availableJobs[location];
        }

        if (_blockedJobs.ContainsKey (location)) {
            return _blockedJobs[location];
        }

        if (_inProgressJobs.ContainsKey (location)) {
            return _inProgressJobs[location];
        }

        return null;
    }

    /// <summary>
    /// Called to set the job factory for the given job
    /// </summary>
    /// <param name="factory">The factory to use</param>
    public void setJobFactory (JobFactory factory) {
        _jobFactory = factory;
    }
    /// <summary>
    /// Called to handle mouse enter of a map node
    /// </summary>
    /// <param name="node">Map node</param>
    public void handleMouseEnterNode (Node node) {
        Debug.Log ("handleMouseEnterNode");
        if (_jobFactory == null) { return; }

        Debug.Log ("handleMouseEnterNode factory defined");

        // Set multiselect if its null
        if (_multiSelectStart == null) {
            _multiSelectStart = node;
        }

        _hoveredNode = node;
        _showJobSelection ();
    }

    /// <summary>
    /// Called to handle mouse exit of a map node
    /// </summary>
    /// <param name="node">Map node</param>
    public void handleMouseExitNode (Node node) {
        if (_jobFactory == null) { return; }

        _hoveredNode = null;

        // If the mouse is not down then clear multiselect
        if (!_isMouseDown) {
            _multiSelectStart = null;
        }
    }

    /// <summary>
    /// Called to handle mouse up on a map node
    /// </summary>
    /// <param name="node">Map node</param>
    public void handleMouseUpNode (Node node) {
        if (_jobFactory == null) { return; }

        // Loop over all the selected nodes and create the appropriate jobs
        foreach (Node singleNode in _selectedNodes) {
            MapManager.Instance.setNodeMarker (singleNode, false, Color.green, "");

            Job newJob = _jobFactory.createJob (singleNode);
            // If a Job was found
            if (newJob != null) {
                if (newJob.isInstant ()) {
                    newJob.complete ();
                } else {
                    MapManager.Instance.setNodeMarker (singleNode, true, Color.yellow, newJob.getTitle ());
                    _registerJob (newJob);
                }
            }
        }
        _selectedNodes.Clear ();
        _isMouseDown = false;
        _multiSelectStart = null;
        _hoveredNode = null;
        _jobFactory = null;
    }

    /// <summary>
    /// Called to handle mouse udown on a map node
    /// </summary>
    /// <param name="node">Map node</param>
    public void handleMouseDownNode (Node node) {
        if (_jobFactory == null) { return; }

        _isMouseDown = true;
    }
}