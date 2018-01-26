using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTANTS                                                    ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC VARIABLES                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public static GameManager Instance;      // Static singleton property
    public Text time;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE VARIABLES                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PRIVATE FUNCTIONS                                                ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Called when the Game Object wakes up
    /// </summary>
    private void Awake () {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of GameManager!");
        }
        Instance = this;
    }

    private void _updateTime() {
        if (Time.timeScale == 0) {
            time.text = "0x";
        } else if (Time.timeScale == 0.5f) {
            time.text = "1/2x";
        } else if (Time.timeScale == 1f) {
            time.text = "1x";
        } else if (Time.timeScale == 2f) {
            time.text = "2x";
        } else {
            time.text = Time.timeScale.ToString();
        }

        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                             PUBLIC FUNCTIONS                                                 ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /**/
    /// <summary>
    /// Sets the time of the game to the given time
    /// </summary>
    /// <param name="time">Time.</param>
    public void setGameTime(float time) {
        Time.timeScale = time;

        _updateTime();
    }

    /// <summary>
    /// Adjust the game time by the given interval
    /// </summary>
    /// <param name="time">Time.</param>
    public void adjustGameTime(float time) {
        if (Time.timeScale == 1f && time > 0f) {
           time = 1f;
        } else if (Time.timeScale == 2f && time < 0f) {
           time = -1f;
        }

        float newTime = Time.timeScale + time;
        if (newTime >= 0 && newTime <= 2) {
            Time.timeScale = newTime;
        }

        _updateTime();
    }
}
