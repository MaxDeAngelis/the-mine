using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     			CONSTANTS												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static int MAP_WIDTH = 50;
	public static int MAP_HEIGHT = 50;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static MapManager Instance;		// Static singleton property

	public Unit startingUnit;

	/* Containers to place new objects in */
	public Transform mapContainer;


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private Vector3 _origin;

	/* Dictionaries of objects */
	private Dictionary<Vector3, Node> _mapNodes = new Dictionary<Vector3, Node>();	

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called when the Game Object wakes up
	/// </summary>
	private void Awake () {
		if (Instance != null)
		{
			Debug.LogError("Multiple instances of MapManager!");
		}
		Instance = this;
	}

	/// <summary>
	/// Called on start of this game object
	/// </summary>
	private void Start() {
		_generate();
	}

	/// <summary>
	/// Helper method to calculate the perlin noise value
	/// </summary>
	/// <returns>The noise value</returns>
	/// <param name="x">The x coordinate</param>
	/// <param name="y">The y coordinate</param>
	/// <param name="size">Size</param>
	/// <param name="mod">Modifier</param>
	private float _perlinNoise(float x, float y, float size, float mod) {
		//Generate a value from the given position, position is divided to make the noise more frequent.
		float noise = Mathf.PerlinNoise( x / size, y / size );

		//Return the noise value
		return noise * mod;

	}

	/// <summary>
	/// Called to generate a brand new map
	/// </summary>
	private void _generate() {
		_origin = new Vector3(UnityEngine.Random.Range(0, MAP_WIDTH), UnityEngine.Random.Range(0, MAP_HEIGHT), 0);
		startingUnit.transform.position = _origin;
		Camera.main.GetComponentInParent<CameraControl>().transform.position = _origin;

		// Generate the actual map
		_generateMap(UnityEngine.Random.Range(20f, 40f), UnityEngine.Random.Range(1f, 3f));
	}

	/// <summary>
	/// Called to generate a map
	/// </summary>
	private void _generateMap(float noise, float mod) {
		Vector3 nodeLocation = new Vector3(0, 0, 0);
		for(int x = 0; x <= MAP_WIDTH; x++) {
			nodeLocation.x = x;
			for(int y = 0; y <= MAP_HEIGHT; y++) {
				// Setup default ground block information
				nodeLocation.y = y;

				GameObject blockToCreate = ItemLibrary.Instance.tunnelBlock;
				if (y != _origin.y || Vector3.Distance(nodeLocation, _origin) > 3f) {
					blockToCreate = ItemLibrary.Instance.stoneBlock;
				} 

				_createMapNode(blockToCreate, nodeLocation);
			}
		}
	}

	/// <summary>
	/// Helper method to create a map node
	/// </summary>
	/// <param name="type">Node to create</param>
	/// <param name="location">The location of the new node</param>
	/// <param name="parent">The parent container</param>
	private void _createMapNode(GameObject type, Vector3 location) {
		GameObject newNode = Instantiate(type) as GameObject;
		newNode.transform.position = location;
		Node mapNode = newNode.GetComponent<Node>();

        addMapNode(mapNode);
	}

    /// <summary>
    /// Called to check if the two nodes are walkable
    /// </summary>
    /// <returns><c>true</c>, if both nodes are walkable, <c>false</c> otherwise.</returns>
    /// <param name="first">First node location to check</param>
    /// <param name="second">Second node location to check</param>
    private bool _isDiagonalAccessible(Node nodeToCheck, Node testOne, Node testTwo) {
        bool returnValue = false;

        if (testOne != null && testTwo != null && nodeToCheck.isTravelable() && testOne.isTravelable() && testTwo.isTravelable()) {
            returnValue= true;
        }

        return returnValue;
    }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to get a map node based on its location
	/// </summary>
	/// <returns>The node</returns>
	/// <param name="location">The location of the node you want</param>
	public Node getNode(Vector3 location) {

		Node returnNode = null;

		if (_mapNodes.ContainsKey(location)) {
			returnNode = _mapNodes[location];
		}

		return returnNode;
	}

	/// <summary>
	/// Called to get all adjacent nodes to the given node. Always returns diagonals
	/// </summary>
	/// <returns>The surrounding nodes</returns>
	/// <param name="root">The root node to find all adjacent nodes for</param>
	public List<Node> getSurroundingNodes(Node root) {
		return getSurroundingNodes(root, true);
	}

    public List<Node> getSurroundingNodes(Node root, bool returnDiagonals) {
        return getSurroundingNodes(root, returnDiagonals, false);
    }

	/// <summary>
	/// Called to get all adjacent nodes to the given node
	/// </summary>
	/// <returns>The surrounding nodes</returns>
	/// <param name="root">The root node to find all adjacent nodes for</param>
	/// <param name="returnDiagonal">Flag for if the diagonal nodes should be returned</param>
    public List<Node> getSurroundingNodes(Node root, bool returnDiagonals, bool checkDiagonalAccessability) {
        Dictionary<string, Node> foundNodes = new Dictionary<string, Node>();

		// Directly adjacent node locations
        Vector3 top = root.transform.position + Vector3.up;
        Vector3 bottom = root.transform.position + Vector3.down;
        Vector3 left = root.transform.position + Vector3.left;
        Vector3 right = root.transform.position + Vector3.right;

        if (_mapNodes.ContainsKey(top)) {
            foundNodes.Add("TOP", _mapNodes[top]);
        }
        if (_mapNodes.ContainsKey(bottom)) {
            foundNodes.Add("BOTTOM", _mapNodes[bottom]);
        }
        if (_mapNodes.ContainsKey(left)) {
            foundNodes.Add("LEFT", _mapNodes[left]);
        }
        if (_mapNodes.ContainsKey(right)) {
            foundNodes.Add("RIGHT", _mapNodes[right]);
        }

		// If the diagonal param is set then return diagonal locations
		if (returnDiagonals) {
			// Diagonal node locations
            Vector3 topRight = top + Vector3.right;
            Vector3 topLeft = top + Vector3.left;
            Vector3 bottomRight = bottom + Vector3.right;
            Vector3 bottomLeft = bottom + Vector3.left;

            // Check the diagonal nodes to make sure they can be accessed
            if (_mapNodes.ContainsKey(topRight) && (!checkDiagonalAccessability || _isDiagonalAccessible(_mapNodes[topRight], foundNodes["TOP"], foundNodes["RIGHT"]))) {
                foundNodes.Add("TOP_RIGHT", _mapNodes[topRight]);
            }
            if (_mapNodes.ContainsKey(topLeft) && (!checkDiagonalAccessability || _isDiagonalAccessible(_mapNodes[topLeft], foundNodes["TOP"], foundNodes["LEFT"]))) {
                foundNodes.Add("TOP_LEFT", _mapNodes[topLeft]);
            }
            if (_mapNodes.ContainsKey(bottomRight) && (!checkDiagonalAccessability || _isDiagonalAccessible(_mapNodes[bottomRight], foundNodes["BOTTOM"], foundNodes["RIGHT"]))) {
                foundNodes.Add("BOTTOM_RIGHT", _mapNodes[bottomRight]);
            }
            if (_mapNodes.ContainsKey(bottomLeft) && (!checkDiagonalAccessability || _isDiagonalAccessible(_mapNodes[bottomLeft], foundNodes["BOTTOM"], foundNodes["LEFT"]))) {
                foundNodes.Add("BOTTOM_LEFT", _mapNodes[bottomLeft]);
            }
		}

        return new List<Node>(foundNodes.Values);
	}

    /// <summary>
    /// Adds the map node.
    /// </summary>
    /// <param name="newNode">New node.</param>
    public void addMapNode(Node newNode) {
        _mapNodes.Add(newNode.transform.position, newNode);
        newNode.transform.parent = mapContainer;
        List<Node> nodes = getSurroundingNodes(newNode);

        foreach (Node node in nodes) {
            node.updateAccents();
        }
    }

    /// <summary>
    /// Removes the map node.
    /// </summary>
    /// <param name="node">Node.</param>
    public void removeMapNode(Node node) {
        if (_mapNodes.ContainsKey(node.transform.position)) {
            _mapNodes.Remove(node.transform.position);
        }
    }

	/// <summary>
	/// Called to change the color of the given node
	/// </summary>
	/// <param name="node">The node</param>
	/// <param name="color">The color to change it to</param>
    public void setNodeMarker(Node node, bool state, Color color, string text) {
        node.setNodeMarker(state, color, text);
	}

	/// <summary>
	/// Clears the color of all nodes on the map
	/// </summary>
    public void clearAllNodeMarkers(Color color) {
		foreach(KeyValuePair<Vector3, Node> item in _mapNodes) {
			Node node = item.Value;
            node.setNodeMarker(false, color, "");
		}
	}
}
