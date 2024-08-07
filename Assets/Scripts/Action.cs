using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public ActionSO ActionInfo { get; private set; }
    public float MagicCost { get; private set; }

    public Action(ActionSO actionInfo){
        ActionInfo = actionInfo;
        MagicCost = actionInfo.MagicCost;
    }
}
