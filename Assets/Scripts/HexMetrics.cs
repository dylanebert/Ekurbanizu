using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics {

    public const float OuterRadius = 1f;
    public const float InnerRadius = OuterRadius * .866025404f;

    public static Vector2[] Corners = {
        new Vector2(0f, OuterRadius),
        new Vector2(InnerRadius, .5f * OuterRadius),
        new Vector2(InnerRadius, -.5f * OuterRadius),
        new Vector2(0f, -OuterRadius),
        new Vector2(-InnerRadius, -.5f * OuterRadius),
        new Vector2(-InnerRadius, .5f * OuterRadius)
    };

    public static Vector2[] ResidentPositions = {
        new Vector2(0f, .25f),
        new Vector2(-.2165f, .125f),
        new Vector2(-.2165f, -.125f),
        new Vector2(0f, -.25f),
        new Vector2(.2165f, -.125f),
        new Vector2(.2165f, .125f),
        new Vector2(-.2165f, .375f),
        new Vector2(-.433f, 0f),
        new Vector2(-.2165f, -.375f),
        new Vector2(.2165f, -.375f),
        new Vector2(.433f, 0f),
        new Vector2(.2165f, .375f)
    };
}
