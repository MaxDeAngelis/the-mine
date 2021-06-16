using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     			CONSTANTS												     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PUBLIC VARIABLES											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static MapManager Instance; // Static singleton property

    public Unit startingUnit;

    /* Containers to place new objects in */
    public Transform mapContainer;

    /* Resources */
    public Text wood;
    public Text food;
    public Text stone;
    public Text iron;
    public Text gold;
    public Text pendingWood;
    public Text pendingFood;
    public Text pendingStone;
    public Text pendingIron;
    public Text pendingGold;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PRIVATE VARIABLES											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /* Dictionaries of objects */
    private Dictionary<RESOURCE_TYPE, int> _resources = new Dictionary<RESOURCE_TYPE, int> ();
    private Dictionary<RESOURCE_TYPE, int> _pendingResources = new Dictionary<RESOURCE_TYPE, int> ();

    Map _gameMap;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PRIVATE FUNCTIONS											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Called when the Game Object wakes up
    /// </summary>
    private void Awake () {
        if (Instance != null) {
            Debug.LogError ("Multiple instances of MapManager!");
        }
        Instance = this;
    }

    /// <summary>
    /// Called on start of this game object
    /// </summary>
    private void Start () {
        _resources[RESOURCE_TYPE.Wood] = 50;
        _resources[RESOURCE_TYPE.Food] = 0;
        _resources[RESOURCE_TYPE.Stone] = 0;
        _resources[RESOURCE_TYPE.Iron] = 5;
        _resources[RESOURCE_TYPE.Gold] = 0;

        _pendingResources[RESOURCE_TYPE.Wood] = 0;
        _pendingResources[RESOURCE_TYPE.Food] = 0;
        _pendingResources[RESOURCE_TYPE.Stone] = 0;
        _pendingResources[RESOURCE_TYPE.Iron] = 0;
        _pendingResources[RESOURCE_TYPE.Gold] = 0;

        _gameMap = new Map (mapContainer);
        _gameMap.update ();
        _updateResources ();
    }

    /// <summary>
    /// Updated the resources list both pending and available
    /// </summary>
    private void _updateResources () {
        wood.text = _resources[RESOURCE_TYPE.Wood].ToString ();
        food.text = _resources[RESOURCE_TYPE.Food].ToString ();
        stone.text = _resources[RESOURCE_TYPE.Stone].ToString ();
        iron.text = _resources[RESOURCE_TYPE.Iron].ToString ();
        gold.text = _resources[RESOURCE_TYPE.Gold].ToString ();

        _updatePendingResource (RESOURCE_TYPE.Wood, pendingWood);
        _updatePendingResource (RESOURCE_TYPE.Food, pendingFood);
        _updatePendingResource (RESOURCE_TYPE.Stone, pendingStone);
        _updatePendingResource (RESOURCE_TYPE.Iron, pendingIron);
        _updatePendingResource (RESOURCE_TYPE.Gold, pendingGold);
    }

    /// <summary>
    /// Update the pending tag for the given resource
    /// </summary>
    /// <param name="type">Type of resource</param>
    /// <param name="text">Text object for the resource</param>
    private void _updatePendingResource (RESOURCE_TYPE type, Text text) {
        if (_pendingResources[type] > 0) {
            text.gameObject.SetActive (true);
            text.text = "(" + _pendingResources[type].ToString () + ")";
        } else {
            text.gameObject.SetActive (false);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PUBLIC FUNCTIONS											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called to update the map. Handles updating visible list and generates new terrain if needed
    /// </summary>
    public void updateMap () {
        _gameMap.update ();
    }

    /// <summary>
    /// Called to get a map node based on its location
    /// </summary>
    /// <returns>The node</returns>
    /// <param name="location">The location of the node you want</param>
    public Node getNode (Vector3 location) {
        return _gameMap.get (location);
    }

    /*
        /// <summary>
        /// Called to see if the node to check is a corner based on root
        /// </summary>
        /// <returns><c>true</c>, if nodeToCheck is a corner node for root, <c>false</c> otherwise.</returns>
        /// <param name="root">Root.</param>
        /// <param name="nodeToCheck">Node to check.</param>
        public bool isCorner (Node root, Node nodeToCheck) {
            Vector3 posToCheck = nodeToCheck.transform.position;
            Vector3 topRight = root.transform.position + Vector3.up + Vector3.right;
            Vector3 topLeft = root.transform.position + Vector3.up + Vector3.left;
            Vector3 bottomRight = root.transform.position + Vector3.down + Vector3.right;
            Vector3 bottomLeft = root.transform.position + Vector3.down + Vector3.left;

            return (posToCheck == topRight || posToCheck == topLeft || posToCheck == bottomRight || posToCheck == bottomLeft);

        }
    */

    /// <summary>
    /// Called to get all adjacent nodes to the given node. Always returns corners
    /// </summary>
    /// <returns>The surrounding nodes</returns>
    /// <param name="root">The root node to find all adjacent nodes for</param>
    public List<Node> getSurroundingNodes (Node root) {
        return getSurroundingNodes (root, true);
    }

    /// <summary>
    /// Gets the nodes that are surround the given root. Does not evaluate if corners are accessable
    /// </summary>
    /// <returns>The surrounding nodes</returns>
    /// <param name="root">Root node to start</param>
    /// <param name="returnDiagonals">If set to <c>true</c> return all corners</param>
    public List<Node> getSurroundingNodes (Node root, bool returnDiagonals) {
        return getSurroundingNodes (root, returnDiagonals, false);
    }

    /// <summary>
    /// Gets the adjacent nodes around the given
    /// </summary>
    /// <returns>The surrounding nodes</returns>
    /// <param name="root">Root.</param>
    /// <param name="returnDiagonals">If set to <c>true</c> return corners</param>
    /// <param name="checkDiagonalAccessability">If set to <c>true</c> check diagonal accessability</param>
    public List<Node> getSurroundingNodes (Node root, bool returnDiagonals, bool checkDiagonalAccessability) {
        return _gameMap.getSurroundingNodes (root, returnDiagonals, checkDiagonalAccessability);
    }

    /// <summary>
    /// Adds the map node.
    /// </summary>
    /// <param name="newNode">New node.</param>
    public void addMapNode (Node newNode) {
        _gameMap.add (newNode);

        List<Node> nodes = getSurroundingNodes (newNode);

        // Everytime a node is added update the accents
        newNode.updateAccents ();
        foreach (Node node in nodes) {
            node.updateAccents ();
        }
    }

    /// <summary>
    /// Removes the map node.
    /// </summary>
    /// <param name="node">Node.</param>
    public void removeMapNode (Node node) {
        _gameMap.remove (node);
    }

    /// <summary>
    /// Called to change the color of the given node
    /// </summary>
    /// <param name="node">The node</param>
    /// <param name="color">The color to change it to</param>
    public void setNodeMarker (Node node, bool state, Color color, string text) {
        node.setNodeMarker (state, color, text);
    }

    /// <summary>
    /// Mark a resource as ear marked and update the UI accordingly
    /// </summary>
    /// <param name="type">Type of resource</param>
    /// <param name="amount">Amount to mark</param>
    public void earMarkResource (RESOURCE_TYPE type, int amount) {
        _resources[type] -= amount;
        _pendingResources[type] += amount;
        _updateResources ();
    }

    /// <summary>
    /// Add a resource to the available resources as long as its not None
    /// </summary>
    /// <param name="type">Type of resource</param>
    /// <param name="amount">Amount to add</param>
    public void addResource (RESOURCE_TYPE type, int amount) {
        if (type != RESOURCE_TYPE.None) {
            _resources[type] += amount;
            _updateResources ();
        }
    }

    /// <summary>
    /// Use a resource. NOTE it will always come from the pending resources since things
    /// should be marked as pending before a job is complete
    /// </summary>
    /// <param name="type">Type of resource</param>
    /// <param name="amount">Amount to use</param>
    public void useResource (RESOURCE_TYPE type, int amount) {
        _pendingResources[type] -= amount;
        _updateResources ();
    }

    /// <summary>
    /// Check if a resource is available
    /// </summary>
    /// <returns><c>true</c>, if resource is available, <c>false</c> otherwise.</returns>
    /// <param name="type">Type.</param>
    /// <param name="amount">Amount.</param>
    public bool isResourceAvailable (RESOURCE_TYPE type, int amount) {
        if (_resources[type] >= amount) {
            return true;
        }
        return false;
    }
}