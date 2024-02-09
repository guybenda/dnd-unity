using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DndCommon;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class DiceScript : NetworkBehaviour
{
    public DiceType Type;
    public UserDice userDice;
    public uint RollId;

    const int minimumStaticFrames = 10;
    int currentStaticFrames = 0;

    FixedString512Bytes rollerEmail;
    public string RollerEmail
    {
        get => rollerEmail.ToString();
        set => rollerEmail = value;
    }

    Outline outline;
    Rigidbody rb;
    MeshRenderer meshRenderer;

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

    NetworkVariable<bool> m_isStatic = new(false);
    public bool IsStatic
    {
        get
        {
            return m_isStatic.Value;
        }
        private set
        {
            m_isStatic.Value = value;
        }
    }

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

        var isStaticTemp = rb.velocity.magnitude <= 0.05f && rb.angularVelocity.magnitude <= 0.05f;
        if (isStaticTemp)
        {
            currentStaticFrames++;
            if (currentStaticFrames >= minimumStaticFrames)
            {
                IsStatic = true;
            }
        }
        else
        {
            currentStaticFrames = 0;
            IsStatic = false;
        }
    }

    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref Type);
        userDice.NetworkSerialize(serializer);
        serializer.SerializeValue(ref RollId);
        serializer.SerializeValue(ref rollerEmail);
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
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 3f;
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
