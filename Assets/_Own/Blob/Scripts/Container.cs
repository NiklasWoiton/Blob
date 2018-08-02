using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Container : MonoBehaviour {
    public float safeZone;
    public float resolution;
    public float threshold;
    public ComputeShader computeShader;
    public MeshFilter meshFilter;
    public bool calculateNormals;

    private CubeGrid grid;

    public void Start() {
        this.grid = new CubeGrid(this, this.computeShader);
        meshFilter = !meshFilter ? this.GetComponent<MeshFilter>() : meshFilter;
    }

    public void Update() {
        this.grid.evaluateAll(this.GetComponentsInChildren<MetaBall>());

        Mesh mesh = this.meshFilter.mesh;
        mesh.Clear();
        mesh.vertices = this.grid.vertices.ToArray();
        mesh.triangles = this.grid.getTriangles();

        if(this.calculateNormals) {
            mesh.RecalculateNormals();
        }
    }
}