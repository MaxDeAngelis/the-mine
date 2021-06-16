using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Map {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     			CONSTANTS												     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static int MAP_WIDTH = 30;
    public static int MAP_HEIGHT = 20;
    private static float IRON_NOISE = UnityEngine.Random.Range (7f, 10f);
    private static float IRON_MOD = UnityEngine.Random.Range (7f, 10f);
    private static float GOLD_NOISE = UnityEngine.Random.Range (6f, 8f);
    private static float GOLD_MOD = UnityEngine.Random.Range (6f, 8f);
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PRIVATE VARIABLES											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Vector3 _origin = Vector3.zero;
    Vector3 _upperLeft;
    Vector3 _lowerRight;
    Transform _mapContainer;
    private Dictionary<Vector3, Node> _visibleNodes = new Dictionary<Vector3, Node> ();
    private Dictionary<Vector2, MapDataNode> _allDataNodes = new Dictionary<Vector2, MapDataNode> ();
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		   CONSTRUCTOR   											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Map (Transform container) {
        _mapContainer = container;

        for (int x = -30; x < MAP_WIDTH; x++) {
            for (int y = -20; y < MAP_HEIGHT; y++) {
                _generateIndividualChunk (new Vector2 (x, y));
            }
        }
    }

    private void _generateIndividualChunk (Vector2 chunkLoc) {
        Vector3 nodeLocation = Vector3.zero;
        for (int x = 0; x < 20f; x++) {
            nodeLocation.x = (chunkLoc.x * 20f) + x;

            for (int y = 0; y < 10f; y++) {
                // Setup default ground block information
                nodeLocation.y = (chunkLoc.y * 10f) + y;

                // Make sure to start with a tunnel
                GameObject blockToCreate = ItemManager.Instance.tunnelBlock;
                if (y != _origin.y || Vector3.Distance (nodeLocation, _origin) > 3f) {
                    blockToCreate = ItemManager.Instance.stoneBlock;
                }

                // Generate the actual data node and save to master list
                MapDataNode mapDataNode = new MapDataNode (blockToCreate, new Vector2 (nodeLocation.x, nodeLocation.y));
                _allDataNodes.Add (mapDataNode.getLocation (), mapDataNode);
            }
        }
    }

    private void _createVisibleMapNode (MapDataNode mapNodeData) {
        Node mapNode = _createMapNode (mapNodeData);
        _visibleNodes.Add (mapNodeData.getLocation (), mapNode);
    }

    private Node _createMapNode (MapDataNode mapNodeData) {
        // Create the new terrain node and add data to the data node
        GameObject newTerrain = GameObject.Instantiate (mapNodeData.getNodeType ()) as GameObject;
        newTerrain.transform.position = mapNodeData.getLocation ();
        newTerrain.transform.parent = _mapContainer;
        mapNodeData.setTerrain (newTerrain);

        // Get the terrain node controller and add to visible list
        Node mapNode = newTerrain.GetComponent<Node> ();

        // Check all the surrounding nodes and update their accents
        List<Node> nodes = MapManager.Instance.getSurroundingNodes (mapNode);
        mapNode.updateAccents ();
        foreach (Node node in nodes) {
            node.updateAccents ();
        }

        float iron = _perlinNoise (mapNodeData.getLocation ().x, mapNodeData.getLocation ().y, IRON_NOISE, IRON_MOD);
        if (iron > 5f) {
            mapNode.addResource (RESOURCE_TYPE.Iron, 2);
            //setNodeMarker(mapNode, true, Color.cyan, iron.ToString());
        }

        float gold = _perlinNoise (mapNodeData.getLocation ().x, mapNodeData.getLocation ().y, GOLD_NOISE, GOLD_MOD);
        if (gold > 5f) {
            mapNode.addResource (RESOURCE_TYPE.Gold, 1);
            //setNodeMarker(mapNode, true, Color.cyan, "");
        }
        return mapNode;
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

    private bool _isInView (Vector2 loc) {
        if (loc.y < _upperLeft.y && loc.y > _lowerRight.y && loc.x > _upperLeft.x && loc.x < _lowerRight.x) {
            return true;
        }

        return false;
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 								     		PUBLIC FUNCTIONS											     ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void update () {
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

        Debug.Log ("Total nodes: " + _allDataNodes.Count);
        foreach (KeyValuePair<Vector2, MapDataNode> item in _allDataNodes) {
            if (_isInView (item.Key)) {
                if (item.Value.getTerrain () == null) {
                    _createVisibleMapNode (item.Value);
                } else if (_visibleNodes.ContainsKey (item.Value.getTerrain ().transform.position) == false) {
                    _visibleNodes.Add (item.Value.getTerrain ().transform.position, item.Value.getTerrain ().GetComponent<Node> ());
                }

            } else {
                _visibleNodes.Remove (item.Key);
            }
        }

        Debug.Log ("Visible nodes: " + _visibleNodes.Count);
    }

    public void add (Node newNode) {
        _visibleNodes.Add (newNode.transform.position, newNode);
        newNode.transform.parent = _mapContainer;
        _allDataNodes[new Vector2 (newNode.transform.position.x, newNode.transform.position.y)].setTerrain (newNode.gameObject);
    }

    public void remove (Node node) {
        if (_visibleNodes.ContainsKey (node.transform.position)) {
            _allDataNodes[new Vector2 (node.transform.position.x, node.transform.position.y)].setTerrain (null);
            _visibleNodes.Remove (node.transform.position);
        }
    }

    /// <summary>
    /// Called to get a map node based on its location
    /// </summary>
    /// <returns>The node</returns>
    /// <param name="location">The location of the node you want</param>
    public Node get (Vector3 location) {
        Node returnNode = null;

        if (_allDataNodes.ContainsKey ((Vector2) location)) {
            MapDataNode dataNode = _allDataNodes[(Vector2) location];
            if (dataNode.getTerrain () == null) {
                returnNode = _createMapNode (dataNode);
            } else {
                returnNode = dataNode.getTerrain ().GetComponent<Node> ();
            }
        }
        return returnNode;
    }

    public List<Node> getSurroundingNodes (Node root, bool returnDiagonals, bool checkDiagonalAccessability) {
        Dictionary<string, Node> foundNodes = new Dictionary<string, Node> ();

        // Directly adjacent node locations
        Vector3 top = root.transform.position + Vector3.up;
        Vector3 bottom = root.transform.position + Vector3.down;
        Vector3 left = root.transform.position + Vector3.left;
        Vector3 right = root.transform.position + Vector3.right;

        if (_visibleNodes.ContainsKey (top)) {
            foundNodes.Add ("TOP", _visibleNodes[top]);
        }
        if (_visibleNodes.ContainsKey (bottom)) {
            foundNodes.Add ("BOTTOM", _visibleNodes[bottom]);
        }
        if (_visibleNodes.ContainsKey (left)) {
            foundNodes.Add ("LEFT", _visibleNodes[left]);
        }
        if (_visibleNodes.ContainsKey (right)) {
            foundNodes.Add ("RIGHT", _visibleNodes[right]);
        }

        // If the diagonal param is set then return diagonal locations
        if (returnDiagonals) {
            // Diagonal node locations
            Vector3 topRight = top + Vector3.right;
            Vector3 topLeft = top + Vector3.left;
            Vector3 bottomRight = bottom + Vector3.right;
            Vector3 bottomLeft = bottom + Vector3.left;

            // Check the diagonal nodes to make sure they can be accessed
            if (_visibleNodes.ContainsKey (topRight) && (!checkDiagonalAccessability || _isDiagonalAccessible (_visibleNodes[topRight], foundNodes["TOP"], foundNodes["RIGHT"]))) {
                foundNodes.Add ("TOP_RIGHT", _visibleNodes[topRight]);
            }
            if (_visibleNodes.ContainsKey (topLeft) && (!checkDiagonalAccessability || _isDiagonalAccessible (_visibleNodes[topLeft], foundNodes["TOP"], foundNodes["LEFT"]))) {
                foundNodes.Add ("TOP_LEFT", _visibleNodes[topLeft]);
            }
            if (_visibleNodes.ContainsKey (bottomRight) && (!checkDiagonalAccessability || _isDiagonalAccessible (_visibleNodes[bottomRight], foundNodes["BOTTOM"], foundNodes["RIGHT"]))) {
                foundNodes.Add ("BOTTOM_RIGHT", _visibleNodes[bottomRight]);
            }
            if (_visibleNodes.ContainsKey (bottomLeft) && (!checkDiagonalAccessability || _isDiagonalAccessible (_visibleNodes[bottomLeft], foundNodes["BOTTOM"], foundNodes["LEFT"]))) {
                foundNodes.Add ("BOTTOM_LEFT", _visibleNodes[bottomLeft]);
            }
        }
        return new List<Node> (foundNodes.Values);
    }
}