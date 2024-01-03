using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using Unity.Netcode;
using UnityEngine;

public class DiceScript : NetworkBehaviour
{
    public NetworkVariable<DiceType> Type;

    Outline outline;
    Rigidbody rb;
    NetworkVariable<bool> isStatic;

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
        if (!IsServer) return;

        isStatic.Value = rb.velocity.magnitude <= 0.3f && rb.angularVelocity.magnitude <= 0.1f;
    }

    void Awake()
    {
        // Init
        rb = gameObject.GetComponent<Rigidbody>();
        outline = gameObject.AddComponent<Outline>();

        // Outline
        outline.enabled = false;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 2f;

        isStatic.OnValueChanged += OnIsStaticUpdate;
    }

    void OnIsStaticUpdate(bool prev, bool curr)
    {
        outline.enabled = curr;
    }


    // void OnDrawGizmos()
    // {
    // var mesh = gameObject.GetComponent<MeshCollider>().sharedMesh;
    // var verts = mesh.vertices;

    // var t = MakeFacePoints().Select(a => transform.TransformPoint(a)).ToList();
    // for (int i = 0; i < t.Count; i++)
    // {
    //     Handles.Label(t[i], (1 + i).ToString());
    // }

    // Gizmos.DrawLineStrip(t.ToArray(), false);
    // Handles.Label(transform.position + new Vector3(0f, 0.7f, 0f), Result().ToString());
    // }

    public int Result()
    {
        return DiceManager.Instance.GetResultOf(Type.Value, transform);
    }

    public bool IsStatic()
    {
        return isStatic.Value;
    }
}
