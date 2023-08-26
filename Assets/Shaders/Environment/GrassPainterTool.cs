using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class GrassPainterTool : MonoBehaviour {

    private Mesh mesh;
    private MeshFilter filter;

    public Color GrassColor;

    public int toolbarInt = 0;
    
    [SerializeField] List<Vector3> positions = new List<Vector3>();
    [SerializeField] List<Color> colors = new List<Color>();
    [SerializeField] List<int> indicies = new List<int>();
    [SerializeField] List<Vector3> normals = new List<Vector3>();
    [SerializeField] List<Vector2> length = new List<Vector2>();

    public bool isPainting;
    public bool isErasing;
    public bool isEditing;

    public int i = 0;

    public float sizeWidth = 1f;
    public float sizeLength = 1f;
    public float density = 1f;
}
