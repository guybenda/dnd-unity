using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using Unity.Netcode;
using UnityEngine;

public class DiceScript : NetworkBehaviour
{
    public DiceType Type;
    public int MaterialId;
    public int Container;

    Outline outline;
    Rigidbody rb;
    MeshRenderer meshRenderer;
    NetworkVariable<bool> isStatic = new(false);

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

        isStatic.Value = rb.velocity.magnitude <= 0.1f && rb.angularVelocity.magnitude <= 0.05f;
    }

    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref materialId);
        serializer.SerializeValue(ref container);
        base.OnSynchronize(ref serializer);
    }

    public override void OnNetworkSpawn()
    {
        // Init
        rb = gameObject.GetComponent<Rigidbody>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        gameObject.GetComponent<MeshFilter>().mesh = DiceManager.Instance.DieMesh(Type);
        SetMaterial(MaterialId);

        // Outline
        outline = gameObject.AddComponent<Outline>();
        outline.enabled = false;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 1f;
        SetOutline(isStatic.Value);


        // Events
        isStatic.OnValueChanged += OnIsStaticUpdate;

        if (IsServer)
        {
            gameObject.GetComponent<MeshCollider>().sharedMesh = DiceManager.Instance.DieColliderMesh(Type);
        }
        else
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

    public int Result()
    {
        return DiceManager.Instance.GetResultOf(Type, transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        var position = other.GetComponentInParent<DiceContainer>().Target.position;

        DiceManager.Instance.ResetDicePosition(this.gameObject, position);
    }
}
