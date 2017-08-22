using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseOverButton : MonoBehaviour {

    public virtual void MouseOver() {
        transform.localScale = Vector3.one * 1.2f;
    }

    public virtual void MouseExit() {
        transform.localScale = Vector3.one;
    }
}
