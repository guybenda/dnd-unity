using System;
using Unity.Collections;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    NetworkVariable<FixedString512Bytes> email = new("");
    public string Email
    {
        get => email.Value.ToString();
        set => email.Value = value;
    }

    NetworkVariable<bool> isAllowedToRoll = new(false);
    public bool IsAllowedToRoll
    {
        get => isAllowedToRoll.Value;
        set => isAllowedToRoll.Value = value;
    }

    public User User { get; private set; }

    void Update()
    {

    }

    void Awake()
    {
        email.OnValueChanged += OnEmailChange;
    }

    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {
        base.OnSynchronize(ref serializer);
    }

    public override void OnNetworkSpawn()
    {

    }

    async void OnEmailChange(FixedString512Bytes prevValue, FixedString512Bytes newValue)
    {
        if (string.IsNullOrEmpty(newValue.ToString()))
        {
            return;
        }

        User = await User.Get(newValue.ToString());
    }
}