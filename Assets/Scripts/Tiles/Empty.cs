using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empty : Tile {

    public override void UpdateColor() {
        switch (gameController.lens) {
            case Lens.Default:
                baseColor = Palette.EmptyTile;
                highlightedColor = baseColor;
                glow.enabled = false;
                break;
            case Lens.Residential:
                baseColor = Palette.Gradient3(Palette.ResidentialCapacityMin, Palette.ResidentialCapacityMid, Palette.ResidentialCapacityMax, (cell.residentialCapacity - 1) / (float)gameController.maxResidentialCapacity);
                highlightedColor = Palette.ResidentialTile;
                glow.color = Color.white;
                glow.enabled = true;
                break;
            case Lens.Industrial:
                baseColor = Palette.Gradient3(Palette.IndustrialCapacityMin, Palette.IndustrialCapacityMid, Palette.IndustrialCapacityMax, (cell.industrialCapacity - 1) / (float)gameController.maxIndustrialCapacity);
                highlightedColor = Palette.IndustrialTile;
                glow.color = Color.white;
                glow.enabled = true;
                break;
            case Lens.Road:
                baseColor = Palette.EmptyTile;
                highlightedColor = baseColor;
                glow.enabled = false;
                break;
            case Lens.Erase:
                baseColor = Palette.EmptyTile;
                highlightedColor = baseColor;
                glow.color = Palette.OffBlack;
                glow.enabled = true;
                break;
            default:
                break;
        }
        base.UpdateColor();
    }

    public override void MouseUp() {
        switch (gameController.pointerState) {
            case Lens.Residential:
                gameController.grid.ReplaceTile(gameController.residentialTileObj, this);
                Destroy(this.gameObject);
                break;
            case Lens.Industrial:
                gameController.grid.ReplaceTile(gameController.industrialTileObj, this);
                Destroy(this.gameObject);
                break;
            default:
                break;
        }
    }

    public override void MouseOver() {
        if (!Input.GetMouseButton(0)) {
            switch (gameController.lens) {
                case Lens.Residential:
                    foreach (Cell adj in cell.adjacent) {
                        adj.tile.SetColor(Palette.Gradient3(Palette.ResidentialCapacityMin, Palette.ResidentialCapacityMid, Palette.ResidentialCapacityMax, adj.residentialCapacity / (float)gameController.maxResidentialCapacity));
                    }
                    break;
                case Lens.Industrial:
                    foreach (Cell adj in cell.adjacent) {
                        adj.tile.SetColor(Palette.Gradient3(Palette.IndustrialCapacityMin, Palette.IndustrialCapacityMid, Palette.IndustrialCapacityMax, adj.industrialCapacity / (float)gameController.maxIndustrialCapacity));
                    }
                    break;
                default:
                    break;
            }
        }
        base.MouseOver();
    }

    public override void MouseExit() {
        foreach(Cell adj in cell.adjacent) {
            adj.tile.UpdateColor();
        }
        base.MouseExit();
    }
}
