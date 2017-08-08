using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LensButton : MonoBehaviour {

    public Image glow;

    protected Color baseColor = Color.white;
    protected GameController gameController;
    protected Image image;
    protected bool selected;

    private void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        image = GetComponent<Image>();
        image.color = baseColor;
        glow.color = Palette.OffBlack;
    }

    public virtual void Select() {
        gameController.DeselectPointerButtons();
    }

    public virtual void Deselect() {
        gameController.SetPointerState(Lens.Default);
        gameController.SetLens(Lens.Default);
        image.color = baseColor;
        transform.localScale = Vector3.one;
        glow.color = Palette.OffBlack;
        selected = false;
    }

    public virtual void MouseOver() {
        if (!selected)
            transform.localScale = Vector3.one * 1.25f;
    }

    public virtual void MouseExit() {
        if (!selected)
            transform.localScale = Vector3.one;
    }
}
