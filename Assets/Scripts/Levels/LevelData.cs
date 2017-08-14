using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelData : ScriptableObject {
    public Sprite image;
    [TextArea]
    public string description;
    public GridData gridData;
}
