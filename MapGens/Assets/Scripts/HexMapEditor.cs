using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {
    public Color[] colors;
    public HexGrid hexGrid;
    private Color activeColor;

    void Awake () {
        SelectColor(0);
    }

    void Update() {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
            HandleInput();
        }
    }
    
    void HandleInput() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            Debug.DrawRay(hit.point, hit.normal * 10, Color.green, 0.1f);
            hexGrid.ColorCell(hit.point, activeColor);
        }
    }

    public void SelectColor(int index) {
        activeColor = colors[index];
    }
}