using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class MarkerFactory {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE VARIABLES                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private Transform _target;
    private Dictionary<Color, Marker> _markers = new Dictionary<Color, Marker>(); // List of markers by color

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                CONSTRUCTOR                                                   ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public MarkerFactory(Transform target) {
        _target = target;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE FUNCTIONS                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets the currently colored marker from the list
    /// </summary>
    /// <returns>The marker for the given color</returns>
    /// <param name="color">Color of the marker to find</param>
    private Marker _getMarker(Color color) {
        Marker currentMarker;
        if (_markers.ContainsKey(color)) {
            currentMarker = _markers[color];
        } else {
            currentMarker = new Marker(color);
            currentMarker.setParent(_target);
            _markers.Add(color, currentMarker);
        }
        return currentMarker;
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                              PUBLIC FUNCTIONS                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Called to set the marker for this node. Overloaded to also set color
    /// </summary>
    /// <param name="isActive">If set to <c>true</c> marker is displayed</param>
    /// <param name="color">Color.</param>
    public void show(Color color, string text) {
        Marker currentMarker = _getMarker(color);

        foreach(KeyValuePair<Color, Marker> item in _markers) {
            Marker marker = item.Value;
            Color key = item.Key;

            if (key == color) {
                continue;
            }
                
            marker.hide();
        }

        currentMarker.setActive(true);
        currentMarker.setText(text);
    }

    /// <summary>
    /// Hide the specified color marker
    /// </summary>
    /// <param name="color">Color of the marker</param>
    public void hide(Color color) {
        Marker currentMarker = _getMarker(color);

        _markers[color].nullify();
        _markers.Remove(color);

        foreach(KeyValuePair<Color, Marker> item in _markers) {
            Marker marker = item.Value;
            Color key = item.Key;

            if (key == color) {
                continue;
            }

            // Show the next one in the list and break
            // TODO: This really only allows two colors the last one in the stack is the one that
            // is displayed but not really the last displayed one
            marker.show();
            break;
        }

        currentMarker.setActive(false);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PRIVATE MARKER                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private class Marker {
        private Color color;
        private TextMesh textMesh;
        public GameObject obj;

        public Marker(Color _color) {
            obj = GameObject.Instantiate(ItemManager.Instance.marker) as GameObject;

            color = _color;
            color.a = 0.3f;
            obj.GetComponent<SpriteRenderer>().material.color = color;

            textMesh = obj.FindChildWithTag("Marker-Text").GetComponent<TextMesh>();
            textMesh.gameObject.GetComponent<Renderer>().sortingLayerName = "Marker-Text";
        }

        public void setParent(Transform parent) {
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
        }

        public void setText(string val) {
            textMesh.text = val;
            textMesh.gameObject.SetActive(true);
        }

        public void setActive(bool state) {
            obj.SetActive(state);
        }

        public void nullify() {
            GameObject.Destroy(obj);
        }

        public void hide() {
            this.setActive(false);
        }

        public void show() {
            this.setActive(true);
        }
    }
}