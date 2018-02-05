using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RoomNode : Node {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC VARIABLES                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Room accents
    public Sprite roomBottomLeft;
    public Sprite roomBottomMiddle;
    public Sprite roomBottomRight;
    public Sprite roomTopLeft;
    public Sprite roomTopMiddle;
    public Sprite roomTopRight;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE VARIABLES                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private bool _isWalkable = false;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE FUNCTIONS                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC FUNCTIONS                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Updates the block accents
    /// </summary>
    public override void updateAccents() {
        List<Node> nodes = MapManager.Instance.getSurroundingNodes(this, false);

        Vector3 right = transform.position + Vector3.right;
        Vector3 top = transform.position + Vector3.up;
        Vector3 bottom = transform.position + Vector3.down;
        Vector3 left = transform.position + Vector3.left;


        bool isRight = false;
        bool isLeft = false;
        bool isTop = false;
        bool isBottom = false;

        // Loop over all surrounding nodes to see if ther are tunnels or rocks 
        // Set flags accordingly for sprite assignment
        foreach (Node node in nodes) {
            Vector3 pos = node.transform.position;
            if (node.getType() == NODE_TYPE.Stone || node.getType() == NODE_TYPE.Tunnel) {
                if (pos == right) {
                    isRight = true;
                } else if (pos == top) {
                    isTop = true;
                } else if (pos == bottom) {
                    isBottom = true;
                } else if (pos == left) {
                    isLeft = true;
                }
            }
        }
        // Figure out what piece of a room it is to assign the sprite
        SpriteRenderer render = gameObject.GetComponent<SpriteRenderer>();
        if (isTop && isLeft) {
            render.sprite = roomTopLeft;
        } else if (isTop && isRight) {
            render.sprite = roomTopRight;
        } else if (isBottom && isLeft) {
            render.sprite = roomBottomLeft;
        } else if (isBottom && isRight) {
            render.sprite = roomBottomRight;
        } else if (isTop) {
            render.sprite = roomTopMiddle;
        } else if (isBottom) {
            render.sprite = roomBottomMiddle;
        }

        // Also update if the node is walkable based on if it was the bottom
        if (isTop) {
            _isWalkable = false;
        } else {
            _isWalkable = true;
        }
    }



    /// <summary>
    /// Called to check if the miner can travel on this node
    /// </summary>
    /// <returns><c>true</c>, if travelable, <c>false</c> otherwise.</returns>
    public override bool isTravelable() {
        return _isWalkable;
    }

    /// <summary>
    /// Called to check if the miner can walk on this node
    /// </summary>
    /// <returns><c>true</c>, if walkable, <c>false</c> otherwise.</returns>
    public override bool isWalkable() {
        return _isWalkable;
    }
        
}
