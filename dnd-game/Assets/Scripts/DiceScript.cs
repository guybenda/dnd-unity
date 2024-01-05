using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using Unity.Netcode;
using UnityEngine;

public class DiceScript : NetworkBehaviour
{
    public NetworkVariable<DiceType> type = new();
    public NetworkVariable<int> materialId = new();


    Outline outline;
    Rigidbody rb;
    MeshRenderer meshRenderer;
    NetworkVariable<bool> isStatic = new(false);

    public DiceType Type
    {
        get => type.Value;
        set => type.Value = value;
    }

    public bool IsStatic
    {
        get => isStatic.Value;
    }

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

    public override void OnNetworkSpawn()
    {
        // Init
        rb = gameObject.GetComponent<Rigidbody>();
        outline = gameObject.AddComponent<Outline>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        // Outline
        outline.enabled = false;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 2f;

        SetMaterial(materialId.Value);
        SetOutline(isStatic.Value);

        isStatic.OnValueChanged += OnIsStaticUpdate;
        materialId.OnValueChanged += OnMaterialIdUpdate;

        if (!IsServer)
        {
            rb.isKinematic = true;
        }

    }

    void OnIsStaticUpdate(bool prev, bool curr)
    {
        SetOutline(curr);
    }

    void SetOutline(bool value)
    {
        outline.enabled = value;
    }

    void OnMaterialIdUpdate(int prev, int curr)
    {
        SetMaterial(curr);
    }

    void SetMaterial(int value)
    {
        meshRenderer.material = DiceManager.Instance.MaterialByIndex(value);
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
        return DiceManager.Instance.GetResultOf(Type, transform);
    }
}
