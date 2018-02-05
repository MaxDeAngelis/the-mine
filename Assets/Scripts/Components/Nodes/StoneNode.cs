using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class StoneNode : Node {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC VARIABLES                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Accents for stone edges
    public GameObject accentTop;
    public GameObject accentRight;
    public GameObject accentBottom;
    public GameObject accentLeft;

    // Resource settings
    public RESOURCE_TYPE resource = RESOURCE_TYPE.None;
    public int resourceAmount = 0;

    // Resource accents
    public GameObject accentIron;
    public GameObject accentGold;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE VARIABLES                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE FUNCTIONS                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC FUNCTIONS                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Updates the stone blocks accents
    /// </summary>
    public override void updateAccents() {
        List<Node> nodes = MapManager.Instance.getSurroundingNodes(this, false);

        Vector3 right = transform.position + Vector3.right;
        Vector3 top = transform.position + Vector3.up;
        Vector3 bottom = transform.position + Vector3.down;
        Vector3 left = transform.position + Vector3.left;

        foreach (Node node in nodes) {
            if (node.getType() != NODE_TYPE.Stone) {
                if (node.transform.position == right) {
                    accentRight.SetActive(true);
                } else if (node.transform.position == top) {
                    accentTop.SetActive(true);
                } else if (node.transform.position == bottom) {
                    accentBottom.SetActive(true);
                } else if (node.transform.position == left) {
                    accentLeft.SetActive(true);
                }
            }
        }

        if (resource == RESOURCE_TYPE.Iron) {
            accentIron.SetActive(true);
        }

        if (resource == RESOURCE_TYPE.Gold) {
            accentGold.SetActive(true);
        }
    }

    /// <summary>
    /// Called to mine any resources that may be on the node
    /// </summary>
    public override void mine() {
        MapManager.Instance.addResource(resource, resourceAmount);
        base.destroy();
    }

    /// <summary>
    /// Adds the given resource to the node
    /// </summary>
    /// <param name="type">Type of resource</param>
    /// <param name="ammount">Ammount of resource</param>
    public override void addResource(RESOURCE_TYPE type, int amount) {
        resource = type;
        resourceAmount = amount;
    }
}
