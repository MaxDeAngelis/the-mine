using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     			CONSTANTS												     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static int MAP_WIDTH = 50;
    public static int MAP_HEIGHT = 50;

    private static float IRON_NOISE;

    private static float IRON_MOD;

    private static float GOLD_NOISE;

    private static float GOLD_MOD;

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
    private Vector3 _origin = Vector3.zero;

    /* Dictionaries of objects */
    private Dictionary<Vector3, Node> _visibleMapNodes = new Dictionary<Vector3, Node> ();

    private Dictionary<Vector2, MapNode> _allMapNodes = new Dictionary<Vector2, MapNode> ();

    private Dictionary<RESOURCE_TYPE, int> _resources = new Dictionary<RESOURCE_TYPE, int> ();
    private Dictionary<RESOURCE_TYPE, int> _pendingResources = new Dictionary<RESOURCE_TYPE, int> ();

    Vector3 _upperLeft;
    Vector3 _lowerRight;
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

        IRON_NOISE = Random.Range (7f, 10f);
        IRON_MOD = Random.Range (7f, 10f);
        GOLD_NOISE = Random.Range (6f, 8f);
        GOLD_MOD = Random.Range (6f, 8f);

        for (int x = -30; x < 30; x++) {
            for (int y = -20; y < 20; y++) {
                _generateChunk (new Vector2 (x, y));
            }
        }

        updateMap ();
        _updateResources ();
    }

    public void updateMap () {
        Vector3 upperLeftScreen = new Vector3 (0, Screen.height, 0);
        Vector3 upperRightScreen = new Vector3 (Screen.width, Screen.height, 0);
        Vector3 lowerLeftScreen = new Vector3 (0, 0, 0);
        Vector3 lowerRightScreen = new Vector3 (Screen.width, 0, 0);

        //Corner locations in world coordinates
        Vector3 upperLeftRaw = Camera.main.ScreenToWorldPoint (upperLeftScreen);
        Vector3 upperRightRaw = Camera.main.ScreenToWorldPoint (upperRightScreen);
        Vector3 lowerLeftRaw = Camera.main.ScreenToWorldPoint (lowerLeftScreen);
        Vector3 lowerRightRaw = Camera.main.ScreenToWorldPoint (lowerRightScreen);

        _upperLeft = new Vector3 (Mathf.Round (upperLeftRaw.x) - 1f, Mathf.Round (upperLeftRaw.y) + 1f, 0);
        _lowerRight = new Vector3 (Mathf.Round (lowerRightRaw.x) + 1f, Mathf.Round (lowerRightRaw.y) - 1f, 0);

        Debug.Log ("Total nodes: " + _allMapNodes.Count);
        _visibleMapNodes.Clear ();
        int test = 0;
        foreach (KeyValuePair<Vector2, MapNode> item in _allMapNodes) {
            if (!item.Value.isActive ()) {
                //Debug.Log(item.Key);
            }
            if (_isInView (item.Key)) {
                test += 1;
                //item.Value.setNodeMarker (false, Color.cyan, "");
                //item.Value.gameObject.SetActive (true);

                GameObject newNode = Instantiate (item.Value.getNodeType ()) as GameObject;
                newNode.transform.position = new Vector3 (item.Key.x, item.Key.y, 0f);
                _visibleMapNodes.Add (newNode.transform.position, newNode.GetComponent<Node> ());
            } else {
                //item.Value.setNodeMarker(true, Color.cyan, "");
                //item.Value.gameObject.SetActive(false);
            }
        }

        Debug.Log ("Visible nodes: " + test);
    }

    private bool _isInView (Vector2 loc) {
        if (loc.y < _upperLeft.y && loc.y > _lowerRight.y && loc.x > _upperLeft.x && loc.x < _lowerRight.x) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Helper method to calculate the perlin noise value
    /// </summary>
    /// <returns>The noise value</returns>
    /// <param name="x">The x coordinate</param>
    /// <param name="y">The y coordinate</param>
    /// <param name="size">Size</param>
    /// <param name="mod">Modifier</param>
    private float _perlinNoise (float x, float y, float size, float mod) {
        //Generate a value from the given position, position is divided to make the noise more frequent.
        float noise = Mathf.PerlinNoise (x / size, y / size);

        //Return the noise value
        return noise * mod;

    }

    public void _generateChunk (Vector2 chunkLoc) {
        Vector3 nodeLocation = Vector3.zero;

        for (int x = 0; x < 20f; x++) {
            nodeLocation.x = (chunkLoc.x * 20f) + x;

            for (int y = 0; y < 10f; y++) {
                // Setup default ground block information
                nodeLocation.y = (chunkLoc.y * 10f) + y;

                GameObject blockToCreate = ItemManager.Instance.tunnelBlock;
                if (y != _origin.y || Vector3.Distance (nodeLocation, _origin) > 3f) {
                    blockToCreate = ItemManager.Instance.stoneBlock;
                }

                _createMapNode (blockToCreate, nodeLocation);
            }
        }
    }

    /// <summary>
    /// Helper method to create a map node
    /// </summary>
    /// <param name="type">Node to create</param>
    /// <param name="location">The location of the new node</param>
    /// <param name="parent">The parent container</param>
    private void _createMapNode (GameObject type, Vector3 location) {
        MapNode mapNodeData = new MapNode (type, new Vector2 (location.x, location.y));
        Vector2 loc = new Vector2 (location.x, location.y);

        _allMapNodes.Add (loc, mapNodeData);

        if (_isInView (loc)) {
            GameObject newNode = Instantiate (mapNodeData.getNodeType ()) as GameObject;
            newNode.transform.position = location;
            Node mapNode = newNode.GetComponent<Node> ();

            addMapNode (mapNode);

            float iron = _perlinNoise (location.x, location.y, IRON_NOISE, IRON_MOD);
            if (iron > 5f) {
                mapNode.addResource (RESOURCE_TYPE.Iron, 2);
                //setNodeMarker(mapNode, true, Color.cyan, iron.ToString());
            }

            float gold = _perlinNoise (location.x, location.y, GOLD_NOISE, GOLD_MOD);
            if (gold > 5f) {
                mapNode.addResource (RESOURCE_TYPE.Gold, 1);
                //setNodeMarker(mapNode, true, Color.cyan, "");
            }
        }
    }

    /// <summary>
    /// Called to check if the two nodes are walkable
    /// </summary>
    /// <returns><c>true</c>, if both nodes are walkable, <c>false</c> otherwise.</returns>
    /// <param name="first">First node location to check</param>
    /// <param name="second">Second node location to check</param>
    private bool _isDiagonalAccessible (Node nodeToCheck, Node testOne, Node testTwo) {
        bool returnValue = false;

        if (testOne != null && testTwo != null && nodeToCheck.isTravelable () && testOne.isTravelable () && testTwo.isTravelable ()) {
            returnValue = true;
        }

        return returnValue;
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
    /// Called to get a map node based on its location
    /// </summary>
    /// <returns>The node</returns>
    /// <param name="location">The location of the node you want</param>
    public Node getNode (Vector3 location) {

        Node returnNode = null;
        Debug.Log ("MapNodes Count:" + _visibleMapNodes.Count);

        if (_visibleMapNodes.ContainsKey (location)) {
            returnNode = _visibleMapNodes[location];
        }

        return returnNode;
    }

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
        Dictionary<string, Node> foundNodes = new Dictionary<string, Node> ();

        // Directly adjacent node locations
        Vector3 top = root.transform.position + Vector3.up;
        Vector3 bottom = root.transform.position + Vector3.down;
        Vector3 left = root.transform.position + Vector3.left;
        Vector3 right = root.transform.position + Vector3.right;

        if (_visibleMapNodes.ContainsKey (top)) {
            foundNodes.Add ("TOP", _visibleMapNodes[top]);
        }
        if (_visibleMapNodes.ContainsKey (bottom)) {
            foundNodes.Add ("BOTTOM", _visibleMapNodes[bottom]);
        }
        if (_visibleMapNodes.ContainsKey (left)) {
            foundNodes.Add ("LEFT", _visibleMapNodes[left]);
        }
        if (_visibleMapNodes.ContainsKey (right)) {
            foundNodes.Add ("RIGHT", _visibleMapNodes[right]);
        }

        // If the diagonal param is set then return diagonal locations
        if (returnDiagonals) {
            // Diagonal node locations
            Vector3 topRight = top + Vector3.right;
            Vector3 topLeft = top + Vector3.left;
            Vector3 bottomRight = bottom + Vector3.right;
            Vector3 bottomLeft = bottom + Vector3.left;

            // Check the diagonal nodes to make sure they can be accessed
            if (_visibleMapNodes.ContainsKey (topRight) && (!checkDiagonalAccessability || _isDiagonalAccessible (_visibleMapNodes[topRight], foundNodes["TOP"], foundNodes["RIGHT"]))) {
                foundNodes.Add ("TOP_RIGHT", _visibleMapNodes[topRight]);
            }
            if (_visibleMapNodes.ContainsKey (topLeft) && (!checkDiagonalAccessability || _isDiagonalAccessible (_visibleMapNodes[topLeft], foundNodes["TOP"], foundNodes["LEFT"]))) {
                foundNodes.Add ("TOP_LEFT", _visibleMapNodes[topLeft]);
            }
            if (_visibleMapNodes.ContainsKey (bottomRight) && (!checkDiagonalAccessability || _isDiagonalAccessible (_visibleMapNodes[bottomRight], foundNodes["BOTTOM"], foundNodes["RIGHT"]))) {
                foundNodes.Add ("BOTTOM_RIGHT", _visibleMapNodes[bottomRight]);
            }
            if (_visibleMapNodes.ContainsKey (bottomLeft) && (!checkDiagonalAccessability || _isDiagonalAccessible (_visibleMapNodes[bottomLeft], foundNodes["BOTTOM"], foundNodes["LEFT"]))) {
                foundNodes.Add ("BOTTOM_LEFT", _visibleMapNodes[bottomLeft]);
            }
        }

        return new List<Node> (foundNodes.Values);
    }

    /// <summary>
    /// Adds the map node.
    /// </summary>
    /// <param name="newNode">New node.</param>
    public void addMapNode (Node newNode) {
        _visibleMapNodes.Add (newNode.transform.position, newNode);
        newNode.transform.parent = mapContainer;
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
        if (_visibleMapNodes.ContainsKey (node.transform.position)) {
            _visibleMapNodes.Remove (node.transform.position);
        }
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
    /// Clears the color of all nodes on the map
    /// </summary>
    public void clearAllNodeMarkers (Color color) {
        foreach (KeyValuePair<Vector3, Node> item in _visibleMapNodes) {
            Node node = item.Value;
            node.setNodeMarker (false, color, "");
        }
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