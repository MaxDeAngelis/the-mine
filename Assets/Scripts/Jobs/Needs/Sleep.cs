using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sleep : Need {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Sleep(Node currentLocation) : base(10f) {
        Furniture bed = ItemManager.Instance.findBed();
        if (bed != null) {
            _location = bed.getLocationNode();
        } else {
            _location = currentLocation;
        }
        _title = "Sleep";
        _needSubType = NEED_TYPE.Sleep;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                               PUBLIC FUNCTIONS                                               ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Gets the action text.
    /// </summary>
    /// <returns>The action text.</returns>
    public override string getActionText() {
        return "Zzz..";
    }

    /// <summary>
    /// Called when the job is complete to tell the worker to wake up
    /// </summary>
    public override void complete() {
        base.complete();

        foreach(Unit worker in this.getWorkers()) {
            worker.needFulfilled(NEED_TYPE.Sleep);
        }
    }
}
