using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadButton : LensButton {

    public override void Select() {
        if(!selected) {
            gameController.DeselectPointerButtons();
            gameController.SetPointerState(Lens.Road);
            gameController.SetLens(Lens.Road);
            glow.color = Color.white;
            transform.localScale = Vector3.one * 1.25f;
            selected = true;
        } else {
            Deselect();
        }
    }

    public override void MouseOver() {
        base.MouseOver();

        if(gameController.pointerState == Lens.Default) {
            gameController.SetLens(Lens.Road);
        }
    }

    public override void MouseExit() {
        base.MouseExit();

        if(gameController.pointerState == Lens.Default) {
            gameController.SetLens(Lens.Default);
        }
    }
}
