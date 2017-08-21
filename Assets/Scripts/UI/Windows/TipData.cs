using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TipData : ScriptableObject {

    public string title;
    public Sprite sprite;
    public Color spriteColor;
    [TextArea]
    public string text;
}
