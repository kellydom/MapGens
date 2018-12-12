using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {
    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;

    MeshCollider meshCollider;
    void Awake () {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
    }

    public void Triangulate(HexCell[] cells) {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        for (int i = 0; i < cells.Length; i++) {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
    }

    void Triangulate(HexCell cell) {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
            Triangulate(d, cell);
        }
    }

    void Triangulate(HexDirection direction, HexCell cell) {
        Vector3 center = cell.transform.localPosition;
            AddTriangle(
                center,
                center + HexMetrics.GetFirstSolidCorner(direction), 
                center + HexMetrics.GetSecondSolidCorner(direction)
            );
            HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
            HexCell neighbor = cell.GetNeighbor(direction) ?? cell;
            HexCell nextNeighbor = cell.GetNeighbor(direction.Next()) ?? cell;
            Color edgeColorPrev = (cell.color + prevNeighbor.color + neighbor.color) / 3f;
            Color edgeColorNext = (cell.color + nextNeighbor.color +  neighbor.color) / 3f;
            AddTriangleColor(
                cell.color, 
                edgeColorPrev, 
                edgeColorNext);

    }

    void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddTriangleColor(Color color) {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);        
    }

    void AddTriangleColor(Color c1, Color c2, Color c3) {
        colors.Add(c1);
        colors.Add(c2);
        colors.Add(c3);   
    }
}