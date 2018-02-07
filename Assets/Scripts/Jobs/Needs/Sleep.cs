using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sleep : Need {
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///                                                 CONSTRUCTOR                                                  ///
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public Sleep() : base(10f) {
        _location = MapManager.Instance.findBed();
        _title = "Sleep";
        _needSubType = NEED_SUB_TYPE.Sleep;
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
            worker.wakeup();
        }
    }
}
