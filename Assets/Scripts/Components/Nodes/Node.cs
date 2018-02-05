using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public NODE_TYPE type;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private MarkerFactory _mark;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
	/// 								     		VIRTUAL FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates the block accents
    /// </summary>
    public virtual void updateAccents() {}

    /// <summary>
    /// Mine this instance.
    /// </summary>
    public virtual void mine() {
        destroy();
    }

    /// <summary>
    /// Called to check if the miner can travel on this node
    /// </summary>
    /// <returns><c>true</c>, if travelable, <c>false</c> otherwise.</returns>
    public virtual bool isTravelable() {
        return false;
    }

    /// <summary>
    /// Called to check if the miner can walk on this node
    /// </summary>
    /// <returns><c>true</c>, if walkable, <c>false</c> otherwise.</returns>
    public virtual bool isWalkable() {
        return false;
    }

    /// <summary>
    /// Adds the given resource to the node
    /// </summary>
    /// <param name="type">Type of resource</param>
    /// <param name="ammount">Ammount of resource</param>
    public virtual void addResource(RESOURCE_TYPE type, int amount) {}


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC FUNCTIONS                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Start this instance.
    /// </summary>
    public void Start() {
        _mark = new MarkerFactory(transform);
    }

    /// <summary>
    /// Destroy this instance.
    /// </summary>
    public void destroy() {
        Destroy(gameObject);
    }

    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <returns>The NODE_TYPE of this node</returns>
    public NODE_TYPE getType() {
        return type;
    }

	/// <summary>
	/// Called to set the marker for this node. Overloaded to also set color
	/// </summary>
	/// <param name="isActive">If set to <c>true</c> marker is displayed</param>
	/// <param name="color">Color.</param>
    public void setNodeMarker(bool state, Color color, string text) {
        if (state) {
            _mark.show(color, text);
        } else {
            _mark.hide(color);
        }
	}
}
