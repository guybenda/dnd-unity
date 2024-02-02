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
    public UserDice userDice;
    public int Container;

    Outline outline;
    Rigidbody rb;
    MeshRenderer meshRenderer;
    // NetworkVariable<bool> isStatic = new(false);

    bool shouldUpdateOutline = true;

    bool m_isHovered;
    public bool IsHovered
    {
        get => m_isHovered;
        set
        {
            m_isHovered = value;
            shouldUpdateOutline = true;
        }
    }

    public bool IsStatic { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldUpdateOutline)
        {
            UpdateOutline();
            shouldUpdateOutline = false;
        }
    }

    void UpdateOutline()
    {
        // Enabled
        outline.enabled = IsHovered;
    }

    void FixedUpdate()
    {
        if (!IsServer) return;

        IsStatic = rb.velocity.magnitude <= 0.1f && rb.angularVelocity.magnitude <= 0.05f;
    }

    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref Type);
        userDice.NetworkSerialize(serializer);
        serializer.SerializeValue(ref Container);
        base.OnSynchronize(ref serializer);
    }

    public override void OnNetworkSpawn()
    {
        // Init
        rb = gameObject.GetComponent<Rigidbody>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        gameObject.GetComponent<MeshFilter>().mesh = DiceManager.Instance.DieMesh(Type);
        SetMaterial();

        // Outline
        outline = gameObject.AddComponent<Outline>();
        outline.enabled = false;
        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 5f;
        outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;

        gameObject.GetComponent<MeshCollider>().sharedMesh = DiceManager.Instance.DieColliderMesh(Type);

        if (IsServer)
        {

        }
        else
        {
            rb.isKinematic = true;
        }

    }

    void SetMaterial()
    {
        meshRenderer.material = DiceMaterialManager.Instance.GenerateMaterialFromUserDice(userDice);
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
