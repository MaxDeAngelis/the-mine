using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	private SpriteRenderer _marker;

	private Dictionary<Color, Marker> _markers = new Dictionary<Color, Marker>();	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called when the game object starts
	/// </summary>
	private void Start () {
	}

	/// <summary>
	/// Called on mouse enter to show where cursor is
	/// </summary>
	private void OnMouseEnter() {
		JobManager.Instance.handleMouseEnterNode(this);
	}

	/// <summary>
	/// Called on mouse exit to clear cursor position
	/// </summary>
	private void OnMouseExit() {
		JobManager.Instance.handleMouseExitNode(this);
	}

	/// <summary>
	/// Called on Mouse Down to start multi select
	/// </summary>
	private void OnMouseDown() {
		JobManager.Instance.handleMouseDownNode(this);
	}

	/// <summary>
	/// Called on Mouse Up to create a new job
	/// </summary>
	private void OnMouseUp() {
		JobManager.Instance.handleMouseUpNode(this);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void initalize() {
		_marker = gameObject.FindChildWithTag("Marker").transform.GetComponent<SpriteRenderer>();
		_marker.material.color = Color.white;
        _marker.gameObject.FindChildWithTag("Marker-Text").GetComponent<Renderer>().sortingLayerName = "Marker-Text";
	}

	public List<Node> getSurroundingNodes() {
		return MapManager.Instance.getSurroundingNodes(this);
	}

	public bool isWalkable() {
		return true;
	}

	/// <summary>
	/// Called to set the marker for this node. Overloaded to also set color
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> is active.</param>
	/// <param name="color">Color.</param>
    public void setNodeMarker(bool state, Color color, string text) {
        Marker currentMarker;
        if (_markers.ContainsKey(color)) {
            currentMarker = _markers[color];
        } else {
            currentMarker = new Marker(_marker.gameObject, color, text);
            _markers.Add(color, currentMarker);
        }

        if (!state) {
            _markers[color].nullify();
            _markers.Remove(color);
        }

        // TODO: This really only allows two colors the last one in the stack is the one that
        // is displayed but not really the last displayed one
        foreach(KeyValuePair<Color, Marker> item in _markers) {
            Marker marker = item.Value;
            Color key = item.Key;

            if (key == color) {
                continue;
            }

            if (state) {
                marker.hide();
            } else {
                marker.show();
                break;
            }
        }

        currentMarker.setActive(state);
        currentMarker.setText(text);
	}

	private class Marker {
		private Color color;
		private TextMesh textMesh;
		public GameObject obj;

		public Marker(GameObject _obj, Color _color, string _text) {
			obj = Instantiate(_obj) as GameObject;
            obj.transform.SetParent(_obj.transform.parent);
            obj.transform.localPosition = Vector3.zero;
			color = _color;
            color.a = 0.3f;
            obj.GetComponent<SpriteRenderer>().material.color = color;

            textMesh = obj.FindChildWithTag("Marker-Text").GetComponent<TextMesh>();
		}

		public void setText(string val) {
            textMesh.text = val;
            textMesh.gameObject.SetActive(true);
		}

		public void setActive(bool state) {
			obj.SetActive(state);
		}

        public void nullify() {
            Destroy(obj);
        }

        public void hide() {
            this.setActive(false);
        }

        public void show() {
            this.setActive(true);
        }
	}
}
