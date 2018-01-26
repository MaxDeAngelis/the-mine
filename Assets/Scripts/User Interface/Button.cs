using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
    protected Vector3 _scaleMod = new Vector3(0.2f, 0.2f, 0.2f);
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC FUNCTIONS                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Called on mouse enter to show where cursor is
    /// </summary>
    public void hover() {
        Vector3 scale = gameObject.transform.localScale;
        scale += _scaleMod;
        gameObject.transform.localScale = scale;
    }

    /// <summary>
    /// Called on mouse enter to show where cursor is
    /// </summary>
    public void blur() {
        Vector3 scale = gameObject.transform.localScale;
        scale -= _scaleMod;
        gameObject.transform.localScale = scale;
    }
}





