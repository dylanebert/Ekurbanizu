using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Industrial : Tile {

    [HideInInspector]
    public List<Resident> workers;

    private void Start() {
        foreach(Cell cell in cell.adjacent) {
            cell.tile.IncreaseIndustrialCapacity();
        }
        gameController.UpdateTileColors();
    }

    public void AddWorker(Resident worker) {
        workers.Add(worker);
        worker.workPos = FillPosition(worker);
        gameController.workers.Add(worker);
    }

    public void RemoveWorker(Resident worker) {
        if (worker == null) return;
        workers.Remove(worker);
        VacatePosition(worker.transform.position);
        if (worker.atWork)
            Leave(worker);
        gameController.workers.Remove(worker);
    }

    public override bool Arrive(Resident resident) {
        if (workers.Count > cell.industrialCapacity) {
            return false;
        }
        else {
            gameController.workersAtWorkCount++;
            resident.atWork = true;
            return true;
        }
    }

    public override void Leave(Resident resident) {
        gameController.workersAtWorkCount--;
        resident.atWork = false;
    }

    public override void UpdateColor() {
        switch (gameController.lens) {
            case Lens.Default:
                baseColor = Palette.IndustrialTile;
                highlightedColor = baseColor;
                glow.enabled = false;
                break;
            case Lens.Residential:
                baseColor = Palette.Gradient3(Palette.ResidentialCapacityMin, Palette.ResidentialCapacityMid, Palette.ResidentialCapacityMax, (cell.residentialCapacity - 1) / (float)gameController.maxResidentialCapacity);
                baseColor = Color.Lerp(baseColor, Palette.Gray, .3f);
                highlightedColor = baseColor;
                glow.color = Palette.OffBlack;
                glow.enabled = true;
                break;
            case Lens.Industrial:
                baseColor = Palette.Gradient3(Palette.IndustrialCapacityMin, Palette.IndustrialCapacityMid, Palette.IndustrialCapacityMax, (cell.industrialCapacity - 1) / (float)gameController.maxIndustrialCapacity);
                baseColor = Color.Lerp(baseColor, Palette.Gray, .3f);
                highlightedColor = baseColor;
                glow.color = Palette.OffBlack;
                glow.enabled = true;
                break;
            case Lens.Road:
                baseColor = Palette.IndustrialTileHighlighted;
                highlightedColor = baseColor;
                glow.enabled = false;
                break;
            case Lens.Erase:
                baseColor = Palette.IndustrialTileHighlighted;
                highlightedColor = Palette.EmptyTile;
                glow.color = Color.white;
                glow.enabled = true;
                break;
            default:
                break;
        }
        base.UpdateColor();
    }

    public override void MouseUp() {
        if (gameController.pointerState == Lens.Erase) {
            gameController.grid.ReplaceTile(gameController.emptyTileObj, this);
        }
    }
}
