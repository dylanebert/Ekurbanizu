using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseButton : LensButton {

    public override void Select() {
        if (!selected) {
            gameController.DeselectPointerButtons();
            gameController.SetLens(Lens.Erase);
            gameController.SetPointerState(Lens.Erase);
            glow.color = Color.white;
            transform.localScale = Vector3.one * 1.25f;
            selected = true;
        }
        else {
            Deselect();
        }
    }

    public override void MouseOver() {
        base.MouseOver();

        if (gameController.pointerState == Lens.Default) {
            gameController.SetLens(Lens.Erase);
        }
    }

    public override void MouseExit() {
        base.MouseExit();

        if (gameController.pointerState == Lens.Default) {
            gameController.SetLens(Lens.Default);
        }
    }
}
