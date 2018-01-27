using UnityEngine;
using System.Collections;

public class ToggleActive : MonoBehaviour {
    public GameObject target;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC FUNCTIONS                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called on mouse enter to show where cursor is
    /// </summary>
    public void toggle() {
        if (target.activeSelf) {
            target.SetActive(false);
        } else {
            target.SetActive(true);
        }
    }
}





