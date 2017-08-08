using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResidentialButton : LensButton {

    private void Start() {
        baseColor = Palette.ResidentialCapacityMin;
        image.color = baseColor;
    }

    public override void Select() {
        if (!selected) {
            gameController.DeselectPointerButtons();
            gameController.SetLens(Lens.Residential);
            gameController.SetPointerState(Lens.Residential);
            glow.color = Color.white;
            transform.localScale = Vector3.one * 1.25f;
            selected = true;
        } else {
            Deselect();
        }
    }

    public override void MouseOver() {
        base.MouseOver();

        if (gameController.pointerState == Lens.Default) {
            gameController.SetLens(Lens.Residential);
        }
    }

    public override void MouseExit() {
        base.MouseExit();

        if (gameController.pointerState == Lens.Default) {
            gameController.SetLens(Lens.Default);
        }
    }
}
