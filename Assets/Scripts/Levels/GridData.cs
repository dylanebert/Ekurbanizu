using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class GridData : ScriptableObject {

    public CellData[] cellData;

    [HideInInspector]
    public int roadCount;
    [HideInInspector]
    public Vector2 mapSize;
    [HideInInspector]
    public int goalCommutes;

    public void UpdateMapSize() {
        cellData = new CellData[(int)mapSize.x * (int)mapSize.y + Mathf.FloorToInt(mapSize.y / 2f)];
        int offset = 0;
        for (int y = 0; y < mapSize.y; y++) {
            if (y % 2 == 1) {
                for (int x = 0; x < mapSize.x + 1; x++) {
                    cellData[y * (int)mapSize.x + x + offset] = new CellData() {
                        coords = new Vector2(x - 1, y)
                    };
                }
                offset++;
            }
            else {
                for (int x = 0; x < mapSize.x; x++) {
                    cellData[y * (int)mapSize.x + x + offset] = new CellData() {
                        coords = new Vector2(x, y)
                    };
                }
            }
        }
    }
}

[System.Serializable]
public class CellData {
    public Vector2 coords;
    public bool enabled = true;
    public int baseResidentialCapacity = 1;
    public int baseIndustrialCapacity = 1;    
}