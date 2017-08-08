using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Palette {

    public static Color OffBlack = Color255(79, 79, 79);
    public static Color Gray = Color255(182, 197, 198);

    public static Color EmptyTile = Color.white;
    public static Color EmptyTileHighlighted = Color255(236, 240, 241);    

    public static Color ResidentialCapacityMin = Color255(193, 237, 207);
    public static Color ResidentialCapacityMid = Color255(44, 204, 114);
    public static Color ResidentialCapacityMax = Color255(107, 172, 130);
    public static Color ResidentialTile = Color255(102, 232, 157);
    public static Color ResidentialTileHighlighted = Color255(142, 249, 188);

    public static Color IndustrialCapacityMin = Color255(248, 204, 166);
    public static Color IndustrialCapacityMid = Color255(242, 196, 14);
    public static Color IndustrialCapacityMax = Color255(198, 109, 14);
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
