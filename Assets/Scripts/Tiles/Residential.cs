using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residential : Tile {

    [HideInInspector]
    public List<Resident> residents;

    float birthTimer;

    private void Start() {
        gameController.UpdateTileColors();
        birthTimer = gameController.birthInterval - 1f;
    }

    private void Update() {
        if (gameController.stage != Stage.CommutingToWork) {
            birthTimer += Time.deltaTime;
            if (birthTimer > gameController.birthInterval) {
                birthTimer = 0f;
                SpawnResident();
            }
        }
    }

    public void SpawnResident() {
        if (residents.Count < cell.residentialCapacity) {
            Resident resident = Instantiate(gameController.residentObj, gameController.grid.transform).GetComponent<Resident>();
            AddResident(resident);
            Arrive(resident);
        }
    }

    public void AddResident(Resident resident) {
        gameController.residents.Add(resident);
        gameController.grid.residents.Push(resident);
        resident.SetHome(this);
        resident.homePos = FillPosition(resident);
        resident.transform.position = resident.homePos;
        residents.Add(resident);
    }

    public void RemoveResident(Resident resident) {
        if (resident == null) return;
        residents.Remove(resident);
        gameController.residents.Remove(resident);
        VacatePosition(resident.transform.position);
        if (resident.atHome)
            Leave(resident);
    }

    public override bool Arrive(Resident resident) {
        if (residents.Count > cell.residentialCapacity) {
            return false;
        }
        else {
            gameController.residentsHomeCount++;
            resident.atHome = true;
            return true;
        }
    }

    public override void Leave(Resident resident) {
        gameController.residentsHomeCount--;
        resident.atHome = false;
    }

    public override void UpdateColor() {
        switch (gameController.lens) {
            case Lens.Default:
                baseColor = Palette.ResidentialTile;
                highlightedColor = baseColor;
                glow.enabled = false;
                indicator.text = "";
                break;
            case Lens.Residential:
                baseColor = Palette.Gradient3(Palette.ResidentialCapacityMin, Palette.ResidentialCapacityMid, Palette.ResidentialCapacityMax, (cell.residentialCapacity - 1) / (float)gameController.maxResidentialCapacity);
                baseColor = Color.Lerp(baseColor, Palette.Gray, .3f);
                highlightedColor = baseColor;
                glow.color = Color.white;
                glow.enabled = true;
                indicator.text = cell.residentialCapacity.ToString();
                break;
            case Lens.Industrial:
                baseColor = Palette.Gradient3(Palette.IndustrialCapacityMin, Palette.IndustrialCapacityMid, Palette.IndustrialCapacityMax, (cell.industrialCapacity - 1) / (float)gameController.maxIndustrialCapacity);
                baseColor = Color.Lerp(baseColor, Palette.Gray, .3f);
                highlightedColor = baseColor;
                glow.color = Color.white;
                glow.enabled = true;
                indicator.text = cell.industrialCapacity.ToString();
                break;
            case Lens.Road:
                baseColor = Palette.ResidentialTileHighlighted;
                highlightedColor = baseColor;
                glow.enabled = false;
                break;
            case Lens.Erase:
                baseColor = Palette.ResidentialTileHighlighted;
                highlightedColor = Palette.EmptyTile;
                glow.color = Palette.OffBlack;
                glow.enabled = true;
                break;
            default:
                break;
        }
        base.UpdateColor();
    }

    public override void MouseUp() {
        if(gameController.pointerState == Lens.Erase) {
            gameController.grid.ReplaceTile(gameController.emptyTileObj, this);
        }
    }
}
