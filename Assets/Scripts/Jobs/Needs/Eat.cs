using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Eat : Need {
    private Food _food;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Eat() : base(3f) {
        _food = ItemManager.Instance.findFood();
        if (_food != null) {
            _location = _food.getLocationNode();
        } else {
            _location = null;
        }
        
        _title = "Eat";
        _needSubType = NEED_TYPE.Eat;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PUBLIC FUNCTIONS                                               ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Called to see if there is any food available
    /// </summary>
    /// <returns><c>true</c>, if food available was ised, <c>false</c> otherwise.</returns>
    public bool isFoodAvailable() {
        return (_location != null);
    }

    /// <summary>
    /// Gets the action text.
    /// </summary>
    /// <returns>The action text.</returns>
    public override string getActionText() {
        return "Yumm..";
    }

    /// <summary>
    /// Called when the job is complete to tell the worker to wake up
    /// </summary>
    public override void complete() {
        base.complete();
        ItemManager.Instance.removeItem(_food);

        _food.eat();

        foreach(Unit worker in this.getWorkers()) {
            worker.needFulfilled(NEED_TYPE.Eat);
        }
    }
}
