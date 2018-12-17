using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {
    public HexGrid hexGrid;
    public Material terrainMaterial;

    bool isDrag;
    HexDirection dragDirection;
    HexCell previousCell;

    void Awake () {
        terrainMaterial.DisableKeyword ("GRID_ON");
        SetEditMode (false);
    }
    void Update () {
        if (!EventSystem.current.IsPointerOverGameObject ()) {
            if (Input.GetMouseButton (0)) {
                HandleInput ();
                return;
            }
            if (Input.GetKeyDown (KeyCode.U)) {
                if (Input.GetKey (KeyCode.LeftShift)) {
                    DestroyUnit ();
                } else {
                    CreateUnit ();
                }
                return;
            }
        }
        previousCell = null;
    }

    HexCell GetCellUnderCursor () {
        return hexGrid.GetCell (Camera.main.ScreenPointToRay (Input.mousePosition));
    }

    void HandleInput () {
        HexCell currentCell = GetCellUnderCursor ();
        if (currentCell) {
            if (previousCell && previousCell != currentCell) {
                ValidateDrag (currentCell);
            } else {
                isDrag = false;
            }
            EditCells (currentCell);
            previousCell = currentCell;
        } else {
            previousCell = null;
        }
    }

    #region Edit Cells 

    void EditCells (HexCell center) {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
            for (int x = centerX - r; x <= centerX + brushSize; x++) {
                EditCell (hexGrid.GetCell (new HexCoordinates (x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
            for (int x = centerX - brushSize; x <= centerX + r; x++) {
                EditCell (hexGrid.GetCell (new HexCoordinates (x, z)));
            }
        }
    }

    void EditCell (HexCell cell) {
        if (cell) {
            if (activeTerrainTypeIndex >= 0) {
                cell.TerrainTypeIndex = activeTerrainTypeIndex;
            }
            if (applyElevation) {
                cell.Elevation = activeElevation;
            }
            if (applyWaterLevel) {
                cell.WaterLevel = activeWaterLevel;
            }
            if (applyUrbanLevel) {
                cell.UrbanLevel = activeUrbanLevel;
            }
            if (applyFarmLevel) {
                cell.FarmLevel = activeFarmLevel;
            }
            if (applyPlantLevel) {
                cell.PlantLevel = activePlantLevel;
            }
            if (applySpecialIndex) {
                cell.SpecialIndex = activeSpecialIndex;
            }
            if (riverMode == OptionalToggle.No) {
                cell.RemoveRiver ();
            }
            if (roadMode == OptionalToggle.No) {
                cell.RemoveRoads ();
            }
            if (walledMode != OptionalToggle.Ignore) {
                cell.Walled = walledMode == OptionalToggle.Yes;
            }
            if (isDrag) {
                HexCell otherCell = cell.GetNeighbor (dragDirection.Opposite ());
                if (otherCell) {
                    if (riverMode == OptionalToggle.Yes) {
                        otherCell.SetOutgoingRiver (dragDirection);
                    }
                    if (roadMode == OptionalToggle.Yes) {
                        otherCell.AddRoad (dragDirection);
                    }
                }
            }
        }
    }

    int activeTerrainTypeIndex;
    public void SetTerrainTypeIndex (int index) {
        activeTerrainTypeIndex = index;
    }

    int activeElevation;
    bool applyElevation = true;
    public void SetApplyElevation (bool toggle) {
        applyElevation = toggle;
    }

    public void SetElevation (float elevation) {
        activeElevation = (int) elevation;
    }

    int activeWaterLevel;
    bool applyWaterLevel = true;
    public void SetApplyWaterLevel (bool toggle) {
        applyWaterLevel = toggle;
    }

    public void SetWaterLevel (float level) {
        activeWaterLevel = (int) level;
    }

    int activeUrbanLevel, activeFarmLevel, activePlantLevel, activeSpecialIndex;
    bool applyUrbanLevel, applyFarmLevel, applyPlantLevel, applySpecialIndex;
    public void SetApplyUrbanLevel (bool toggle) {
        applyUrbanLevel = toggle;
    }
    public void SetUrbanLevel (float level) {
        activeUrbanLevel = (int) level;
    }

    public void SetApplyFarmLevel (bool toggle) {
        applyFarmLevel = toggle;
    }
    public void SetFarmLevel (float level) {
        activeFarmLevel = (int) level;
    }

    public void SetApplyPlantLevel (bool toggle) {
        applyPlantLevel = toggle;
    }
    public void SetPlantLevel (float level) {
        activePlantLevel = (int) level;
    }

    public void SetApplySpecialLevel (bool toggle) {
        applySpecialIndex = toggle;
    }
    public void SetSpecialLevel (float level) {
        activeSpecialIndex = (int) level;
    }

    int brushSize;
    public void SetBrushSize (float size) {
        brushSize = (int) size;
    }

    enum OptionalToggle {
        Ignore,
        Yes,
        No
    }
    OptionalToggle riverMode, roadMode, walledMode;
    public void SetRiverMode (int mode) {
        riverMode = (OptionalToggle) mode;
    }

    public void SetRoadMode (int mode) {
        roadMode = (OptionalToggle) mode;
    }

    public void SetWalledMode (int mode) {
        walledMode = (OptionalToggle) mode;
    }
    #endregion

    #region Units 
    void CreateUnit () {
        HexCell cell = GetCellUnderCursor ();
        if (cell && !cell.Unit) {
            hexGrid.AddUnit (Instantiate (HexUnit.unitPrefab), cell, Random.Range (0f, 360f));
        }
    }

    void DestroyUnit () {
        HexCell cell = GetCellUnderCursor ();
        if (cell && cell.Unit) {
            hexGrid.RemoveUnit (cell.Unit);
        }
    }
    #endregion
    void ValidateDrag (HexCell currentCell) {
        for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++) {
            if (previousCell.GetNeighbor (dragDirection) == currentCell) {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    public void ShowGrid (bool visible) {
        if (visible) {
            terrainMaterial.EnableKeyword ("GRID_ON");
        } else {
            terrainMaterial.DisableKeyword ("GRID_ON");
        }
    }

    public void SetEditMode (bool toggle) {
        enabled = toggle;
    }
}