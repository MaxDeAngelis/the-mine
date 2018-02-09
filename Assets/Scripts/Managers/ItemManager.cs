using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour {
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PUBLIC VARIABLES											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public static ItemManager Instance;
	public GameObject tunnelBlock;
	public GameObject stoneBlock;
    public GameObject shaftBlock;
    public GameObject roomBlock;

    public GameObject miner;
    public GameObject lamp;

    public GameObject potato;
    public GameObject marker;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE VARIABLES                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private List<Item> _items = new List<Item>();
    private List<Item> _pendingsItems = new List<Item>();

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 								     		PRIVATE FUNCTIONS											     ///
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Called on Awake of the Game Object
	/// </summary>
	private void Awake () {
		if (Instance != null)
		{
			Debug.LogError("Multiple instances of ItemManager!");
		}
		Instance = this;
	}

    /// <summary>
    /// Adds the item.
    /// </summary>
    /// <param name="item">Item.</param>
    public void addItem(Item item) {
        _items.Add(item);
    }

    /// <summary>
    /// Gets the item based off the location given
    /// </summary>
    /// <returns>The item.</returns>
    /// <param name="location">Location to find item at</param>
    public Item getItem(Vector3 location) {
        if (_items.Count > 0) {
            foreach(Item item in _items) {
                if (item.getLocation() == location) {
                    return item;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Removes the item from the pending list
    /// </summary>
    /// <param name="item">Item to remove</param>
    public void removeItem(Item item) {
        _pendingsItems.Remove(item);
    }

    /// <summary>
    /// Finds the first food in the items
    /// </summary>
    /// <returns>The food</returns>
    public Food findFood() {
        Food returnFood = null;

        foreach(Item item in _items) {
            if (item.getType() == ITEM_TYPE.Food) {
                returnFood = (Food)item;
                break;
            }
        }

        _pendingsItems.Add(returnFood);
        _items.Remove(returnFood);

        return returnFood;
    }
}
