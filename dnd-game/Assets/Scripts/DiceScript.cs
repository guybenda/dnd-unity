using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using Unity.VisualScripting;
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
        facePoints = MakeFacePoints();

        // Outline
        outline.enabled = false;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 2f;
    }

    Vector3[] MakeFacePoints()
    {
        // TODO: do this once and cache it
        var mesh = gameObject.GetComponent<MeshCollider>().sharedMesh;
        Vector3[] points;
        var v = mesh.vertices;

        switch (Type)
        {
            case DiceType.D4:
                points = new Vector3[4]{
                    AveragePoints(v[0], v[1], v[2]),
                    AveragePoints(v[3], v[4], v[5]),
                    AveragePoints(v[6], v[7], v[8]),
                    AveragePoints(v[9], v[10], v[11])
                };
                break;
            case DiceType.D6:
                points = new Vector3[6]{
                    AveragePoints(v[4], v[5], v[6], v[7]),
                    AveragePoints(v[12], v[13], v[14], v[15]),
                    AveragePoints(v[16], v[17], v[18], v[19]),
                    AveragePoints(v[8], v[9], v[10], v[11]),
                    AveragePoints(v[20], v[21], v[22], v[23]),
                    AveragePoints(v[0], v[1], v[2], v[3]),
                };
                break;
            case DiceType.D8:
                points = new Vector3[8]{
                    AveragePoints(v[15], v[16], v[17]),
                    AveragePoints(v[21], v[22], v[23]),
                    AveragePoints(v[12], v[13], v[14]),
                    AveragePoints(v[18], v[19], v[20]),
                    AveragePoints(v[0], v[1], v[2]),
                    AveragePoints(v[6], v[7], v[8]),
                    AveragePoints(v[3], v[4], v[5]),
                    AveragePoints(v[9], v[10], v[11]),
                };
                break;
            case DiceType.D10:
            case DiceType.D100:
                points = new Vector3[10]{
                    AveragePoints(v[24], v[25], v[26], v[27]),
                    AveragePoints(v[12], v[13], v[14], v[15]),
                    AveragePoints(v[32], v[33], v[34], v[35]),
                    AveragePoints(v[8], v[9], v[10], v[11]),
                    AveragePoints(v[20], v[21], v[22], v[23]),
                    AveragePoints(v[0], v[1], v[2], v[3]),
                    AveragePoints(v[28], v[29], v[30], v[31]),
                    AveragePoints(v[16], v[17], v[18], v[19]),
                    AveragePoints(v[36], v[37], v[38], v[39]),
                    AveragePoints(v[4], v[5], v[6], v[7]),
                };
                break;
            case DiceType.D12:
                points = new Vector3[12]{
                    AveragePoints(v[30], v[31], v[32], v[33], v[34]),
                    AveragePoints(v[20], v[21], v[22], v[23], v[24]),
                    AveragePoints(v[25], v[26], v[27], v[28], v[29]),
                    AveragePoints(v[55], v[56], v[57], v[58], v[59]),
                    AveragePoints(v[35], v[36], v[37], v[38], v[39]),
                    AveragePoints(v[40], v[41], v[42], v[43], v[44]),
                    AveragePoints(v[15], v[16], v[17], v[18], v[19]),
                    AveragePoints(v[10], v[11], v[12], v[13], v[14]),
                    AveragePoints(v[50], v[51], v[52], v[53], v[54]),
                    AveragePoints(v[0], v[1], v[2], v[3], v[4]),
                    AveragePoints(v[45], v[46], v[47], v[48], v[49]),
                    AveragePoints(v[5], v[6], v[7], v[8], v[9]),
                };
                break;
            case DiceType.D20:
                points = new Vector3[20]{
                    AveragePoints(v[18], v[19], v[20]),
                    AveragePoints(v[36], v[37], v[38]),
                    AveragePoints(v[12], v[13], v[14]),
                    AveragePoints(v[45], v[46], v[47]),
                    AveragePoints(v[33], v[34], v[35]),

                    AveragePoints(v[57], v[58], v[59]),
                    AveragePoints(v[21], v[22], v[23]),
                    AveragePoints(v[6], v[7], v[8]),
                    AveragePoints(v[54], v[55], v[56]),
                    AveragePoints(v[3], v[4], v[5]),

                    AveragePoints(v[42], v[43], v[44]),
                    AveragePoints(v[24], v[25], v[26]),
                    AveragePoints(v[30], v[31], v[32]),
                    AveragePoints(v[51], v[52], v[53]),
                    AveragePoints(v[27], v[28], v[29]),

                    AveragePoints(v[9], v[10], v[11]),
                    AveragePoints(v[0], v[1], v[2]),
                    AveragePoints(v[39], v[40], v[41]),
                    AveragePoints(v[15], v[16], v[17]),
                    AveragePoints(v[48], v[49], v[50]),
                };
                break;
            default:
                points = new Vector3[0] { };
                break;

        }


        return points;
    }

    void OnDrawGizmos()
    {
        // var mesh = gameObject.GetComponent<MeshCollider>().sharedMesh;
        // var verts = mesh.vertices;

        // var t = MakeFacePoints().Select(a => transform.TransformPoint(a)).ToList();
        // for (int i = 0; i < t.Count; i++)
        // {
        //     Handles.Label(t[i], (1 + i).ToString());
        // }

        // Gizmos.DrawLineStrip(t.ToArray(), false);
        Handles.Label(transform.position + new Vector3(0f, 0.7f, 0f), Result().ToString());
    }

    Vector3 AveragePoints(params Vector3[] points)
    {
        var sum = Vector3.zero;
        foreach (var point in points)
        {
            sum += point;
        }
        return sum / points.Length;
    }

    public int Result()
    {
        var lowestIdx = 0;
        var lowestPos = transform.TransformPoint(facePoints[0]).y;

        for (int i = 1; i < facePoints.Length; i++)
        {
            var pos = transform.TransformPoint(facePoints[i]);
            if (pos.y < lowestPos)
            {
                lowestIdx = i;
                lowestPos = pos.y;
            }
        }

        if (Type == DiceType.D100)
        {
            return (lowestIdx + 1) * 10 % 100;
        }

        return lowestIdx + 1;
    }

    public bool IsStatic()
    {
        return isStatic;
    }
}
