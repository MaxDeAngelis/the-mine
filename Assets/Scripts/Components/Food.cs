using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Food : Item {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC VARIABLES                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public int amount = 1;

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
    /// Start this instance
    /// </summary>
    public new void Start() {
        ItemManager.Instance.addItem(this);
        MapManager.Instance.addResource(RESOURCE_TYPE.Food, amount);
    }

    /// <summary>
    /// Eat this instance
    /// </summary>
    public void eat() {
        MapManager.Instance.useResource(RESOURCE_TYPE.Food, amount);
        Destroy(gameObject);
    }

    public int getAmount() {
        return amount;
    }
}
