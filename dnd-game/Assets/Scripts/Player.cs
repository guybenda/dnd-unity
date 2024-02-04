using System;
using Unity.Collections;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public event Action<Player> OnChange;

    NetworkVariable<FixedString512Bytes> email = new("");
    public string Email
    {
        get => email.Value.ToString();
        set
        {
            email.Value = value;
            OnChange?.Invoke(this);
        }
    }

    NetworkVariable<bool> isAllowedToRoll = new(false);
    public bool IsAllowedToRoll
    {
        get => isAllowedToRoll.Value;
        set
        {
            isAllowedToRoll.Value = value;
            OnChange?.Invoke(this);
        }
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

        // There's a race here but the email should only be set once
        User = await User.Get(newValue.ToString());

        OnChange?.Invoke(this);
    }
}