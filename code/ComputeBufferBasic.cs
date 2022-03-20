using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class ComputeBufferBasic : MonoBehaviour
{
    /*
    [SerializeField]
    private Material material;

    private Mesh mesh;

    // Transform here is a compressed transform information
    // xy is the position, z is rotation, w is the scale
    private ComputeBuffer transformBuffer;

    // uvBuffer contains float4 values in which xy is the uv dimension and zw is the texture offset
    private ComputeBuffer uvBuffer;
    private ComputeBuffer colorBuffer;

    private readonly uint[] args = {
        6, 1, 0, 0, 0
    };

    private ComputeBuffer argsBuffer;

    private void Awake()
    {
        this.mesh = CreateQuad();

        this.transformBuffer = new ComputeBuffer(1, 16);
        float scale = 0.2f;
        this.transformBuffer.SetData(new float4[] { new float4(0, 0, 0, scale) });
        int matrixBufferId = Shader.PropertyToID("transformBuffer");
        this.material.SetBuffer(matrixBufferId, this.transformBuffer);

        this.uvBuffer = new ComputeBuffer(1, 16);
        this.uvBuffer.SetData(new float4[] { new float4(0.25f, 0.25f, 0, 0) });
        int uvBufferId = Shader.PropertyToID("uvBuffer");
        this.material.SetBuffer(uvBufferId, this.uvBuffer);

        this.colorBuffer = new ComputeBuffer(1, 16);
        this.colorBuffer.SetData(new float4[] { new float4(1, 1, 1, 1) });
        int colorsBufferId = Shader.PropertyToID("colorsBuffer");
        this.material.SetBuffer(colorsBufferId, this.colorBuffer);

        this.argsBuffer = new ComputeBuffer(1, this.args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        this.argsBuffer.SetData(this.args);
    }

    private static readonly Bounds BOUNDS = new Bounds(Vector2.zero, Vector3.one);

    private void Update()
    {
        // Draw
        Graphics.DrawMeshInstancedIndirect(this.mesh, 0, this.material, BOUNDS, this.argsBuffer);
    }

    // This can be refactored to a utility class
    // Just added it here for the article
    private static Mesh CreateQuad()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 1, 0);
        vertices[3] = new Vector3(1, 1, 0);
        mesh.vertices = vertices;

        int[] tri = new int[6];
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;
        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;
        mesh.triangles = tri;

        Vector3[] normals = new Vector3[4];
        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        normals[3] = -Vector3.forward;
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);
        mesh.uv = uv;

        return mesh;
    }
    */
}