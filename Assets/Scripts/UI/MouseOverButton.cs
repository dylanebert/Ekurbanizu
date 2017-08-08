using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseOverButton : MonoBehaviour {

    public void MouseOver() {
        transform.localScale = Vector3.one * 1.25f;
    }

    public void MouseExit() {
        transform.localScale = Vector3.one;
    }
}
