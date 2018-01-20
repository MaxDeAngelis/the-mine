using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PathFinder {

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private PriorityQueue<float, PathNode> _openNodes = new PriorityQueue<float, PathNode>();	// Open list of nodes
	private Dictionary<Vector3, PathNode> _closedNodes = new Dictionary<Vector3, PathNode>();
	private List<PathNode> _currentPath;

    private List<Node> _highlightedNodes = new List<Node>();

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	PathNode _findNext(PathNode currentNode, PathNode finish) {
        /* DEBUG */ MapManager.Instance.setNodeMarker(currentNode.getNode(), true, Color.red, "");
        /* DEBUG */ _highlightedNodes.Add(currentNode.getNode());

		PathNode returnNode = null;
		Vector3 currentPosition = currentNode.getPosition();
		Vector3 finishPosition = finish.getPosition();

		// Loop over surrounding nodes to find path
		foreach (Node individualNode in currentNode.getSurroundingNodes()) {
			Vector3 individualPosition = individualNode.transform.position;
			float TempG = currentNode.getG();
			float TempH = Vector3.Distance(individualPosition, finishPosition);

			// Calculate G value (Movement cost)
			if (Vector3.Distance(currentPosition, individualPosition) == 1) {
				TempG += 1f;
			} else {
				TempG += 1.414f;
			}

			// If its in the closed list then update it or add to both open and closed
			if (_closedNodes.ContainsKey(individualPosition)) {
				// Inly update if it costs less
				if (_closedNodes[individualPosition].getG() > TempG) {
					_closedNodes[individualPosition].setG(TempG);
					_closedNodes[individualPosition].setH(TempH);
					_closedNodes[individualPosition].setParent(currentNode);
				}
			} else {
				PathNode newNode = new PathNode(individualNode, currentNode, TempG, TempH);
				_openNodes.Enqueue(newNode.getF(), newNode);
				_closedNodes.Add(individualNode.transform.position, newNode);
			}
		}

		//Dequeue minimum node from the Priority list
		if (!_openNodes.IsEmpty) {
			returnNode = _openNodes.DequeueValue();
		}
		return returnNode;
	}

    private void _clearPathHighlight() {
        if (_highlightedNodes.Count > 0) {
            foreach (Node individualNode in _highlightedNodes) {
                MapManager.Instance.setNodeMarker(individualNode, false, Color.red, "");
                MapManager.Instance.setNodeMarker(individualNode, false, Color.blue, "");
            }
            _highlightedNodes.Clear();
        }
    }

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called to find an accessable path for travel
	/// </summary>
	/// <returns><c>true</c>, if path was found, <c>false</c> otherwise.</returns>
	/// <param name="start">Start path node</param>
	/// <param name="finish">Finish path node</param>
	public bool findPath(Node start, Node finish) {
        /* DEBUG */ _clearPathHighlight();

  		bool isPathFound = false;
		float initialH = Vector3.Distance(start.transform.position, finish.transform.position);
		PathNode iterNode = new PathNode(start, null, 0, initialH);
		//iterNode.setH(initialF);

		PathNode finishNode = new PathNode(finish);

		//If you dont have to move just return the single node
		if (start == finish) {
			_currentPath = new List<PathNode>();	
			_currentPath.Add(iterNode);
			return true;
		}

		//_openNodes.Enqueue(initialF, iterNode);
		_closedNodes.Add(start.transform.position, iterNode);

		//Keep looping until a path is found or the origin is returned
		while(iterNode != null && iterNode.getNode() != finish) {
			iterNode = _findNext(iterNode, finishNode);
		}


		//If the last result is the end location then build a new array
		if (iterNode != null && iterNode.getNode() == finish) {
			List<PathNode> pathFound = new List<PathNode>();	
			while (iterNode.getNode() != start && pathFound.Count < 1000) {

				/* DEBUG */ //MapManager.Instance.changeNodeColor(iterNode.getNode(), Color.blue);

				pathFound.Add(iterNode);
				iterNode = iterNode.getParent();				
			}
			pathFound.Reverse();

			// If the path that was found is better than the current path then replace it 
			if (_currentPath == null || pathFound.Count <= _currentPath.Count) {
				// If the Count of nodes match tie break using F fo the best cost match
				if (_currentPath != null && pathFound.Count == _currentPath.Count) {
					if (pathFound.Last().getF() < _currentPath.Last().getF()) {
						_currentPath = pathFound;
					}
				} else {
					_currentPath = pathFound;
				}
			}

			isPathFound = true;
		}

		//Clear arrays
		_closedNodes.Clear();
		_openNodes.Clear();

		return isPathFound;
	}

	/// <summary>
	/// Called to get the length of the current path
	/// </summary>
	/// <returns>The length of the path</returns>
	public int getLength() {
		return _currentPath.Count;
	}

	/// <summary>
	/// Called to get the next node in the path
	/// </summary>
	/// <returns>The next node in the current path</returns>
	public Node getNextNode() {
		Node tempNode = _currentPath[0].getNode();
		_currentPath.RemoveAt(0);
		return tempNode;
	}

	/// <summary>
	/// Called to check if the current path is empty
	/// </summary>
	/// <returns><c>true</c>, if current path is empty, <c>false</c> otherwise.</returns>
	public bool isEmpty() {
		return (_currentPath == null || _currentPath.Count <=0);
	}

	/// <summary>
	/// Called to nullify the current object
	/// </summary>
	public void nullify() {
        /* DEBUG */ _clearPathHighlight();
		_closedNodes.Clear();
		_openNodes.Clear();
		_currentPath.Clear();
		_currentPath = null;
	}

	public void highlight() {
		int count = 0;
		foreach(PathNode node in _currentPath) {
            /* DEBUG */ _highlightedNodes.Add(node.getNode());
            /* DEBUG */ MapManager.Instance.setNodeMarker(node.getNode(), false, Color.red, "");
            string pathData = count + "\nF: " + node.getF() + "\nG: " + node.getG() + "\nH: " + node.getH();
            /* DEBUG */ MapManager.Instance.setNodeMarker(node.getNode(), true, Color.blue, pathData);
			count++;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE CLASS												     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private class PathNode {
		/* ---- PRIVATE VARIABLES ---- */
		private float _g;
		private float _h;
		private Node _node;
		private PathNode _parent;

		/* ---- CONSTRUCTORS ---- */
		public PathNode(Node node): this(node, null, 0, 0) {
			// Do nothing
		}

		public PathNode(Node node, PathNode parent, float g, float h) {
			_node = node;
			_parent = parent;
			_g = g;
			_h = h;
		}

		private void _initialize(Node node, PathNode parent, float g, float h) {
			_node = node;
			_parent = parent;
			_g = g;
			_h = h;
		}

		/* ---- GETTERS ---- */
		public Node getNode() {
			return _node;
		}

		public PathNode getParent() {
			return _parent;
		}

		public Vector3 getPosition() {
			return _node.transform.position;
		}

		public List<Node> getSurroundingNodes() {
            List<Node> tavelable = new List<Node>();
            List<Node> allNodes = _node.getSurroundingNodes();

            foreach(Node node in allNodes) {
                if (node.isTravelable()) {
                    tavelable.Add(node);
                }
            }
            return tavelable;
		}

		public float getG() {
			return _g;
		}

		public float getH() {
			return _h;
		}

		public float getF() {
			return _g + _h;
		}

		/* ---- SETTERS ---- */
		public void setG(float value) {
			_g = value;
		}

		public void setH(float value) {
			_h = value;
		}

		public void setParent(PathNode parent) {
			_parent = parent;
		}
	}
}
