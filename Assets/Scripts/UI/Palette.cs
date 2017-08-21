using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Palette {

    public static Color OffBlack = Color255(50, 50, 50);
    public static Color Gray = Color255(182, 197, 198);

    public static Color EmptyTile = Color.white;
    public static Color EmptyTileHighlighted = Color255(236, 240, 241);    

    public static Color ResidentialCapacityMin = Color255(229, 245, 249);
    public static Color ResidentialCapacityMid = Color255(153, 216, 201);
    public static Color ResidentialCapacityMax = Color255(44, 162, 95);
    public static Color ResidentialTile = Color255(102, 232, 157);
    public static Color ResidentialTileHighlighted = Color255(142, 249, 188);

    public static Color IndustrialCapacityMin = Color255(255, 247, 188);
    public static Color IndustrialCapacityMid = Color255(254, 196, 79);
    public static Color IndustrialCapacityMax = Color255(217, 95, 79);
    public static Color IndustrialTile = Color255(255, 221, 15);
    public static Color IndustrialTileHighlighted = Color255(255, 233, 150);

    public static Color Gradient3(Color a, Color b, Color c, float v) {
        if (v < .5f) {
            return Color.Lerp(a, b, v / .5f);
        } else {
            return Color.Lerp(b, c, (v - .5f) / .5f);
        }
    }

    public static Color Color255(int r, int g, int b) {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}
