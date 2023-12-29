using System.Collections;
using System.Collections.Generic;
using DndCommon;
using UnityEditor;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    public DiceType Type;

    Outline outline;
    Rigidbody rb;
    bool isStatic = false;

    Vector3[] facePoints;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        isStatic = rb.velocity.magnitude <= 0.3f && rb.angularVelocity.magnitude <= 0.1f;

        if (isStatic != outline.enabled)
        {
            outline.enabled = isStatic;
        }
    }

    void Awake()
    {
        // Init
        rb = gameObject.GetComponent<Rigidbody>();
        outline = gameObject.AddComponent<Outline>();
        // normals =

        // Outline
        outline.enabled = false;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 5f;


        var collider = gameObject.GetComponent<MeshCollider>();
        var mesh = gameObject.GetComponent<MeshCollider>().sharedMesh;
        Debug.Log(mesh.normals);
    }

    // public int? Result() {

    // }

    // Vector3[] MakeFacePoints() {
    //     var mesh = gameObject.GetComponent<MeshCollider>().sharedMesh;
    //     Vector3[] points;
    //     var verts = mesh.vertices;

    //     for (int v = 0; v < verts.Length; v++)
    //     {

    //     }
    //     switch (Type){
    //         case DiceType.D4:{
    //             points = new Vector3[4];

    //         }
    //     }


    //     return null;//TODO
    // }

    void OnDrawGizmos()
    {
        var mesh = gameObject.GetComponent<MeshCollider>().sharedMesh;
        var verts = mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
        {
            var v = verts[i];
            Handles.Label(transform.TransformPoint(v), i + 1 + "");
        }
    }

    Vector3 AveragePoints(Vector3[] points)
    {
        var sum = Vector3.zero;
        foreach (var point in points)
        {
            sum += point;
        }
        return sum / points.Length;
    }

}
