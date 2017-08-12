using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedButton : MouseOverButton {

    [HideInInspector]
    public bool selected;

    public override void MouseOver() {
        if (!selected)
            base.MouseOver();
    }
}
