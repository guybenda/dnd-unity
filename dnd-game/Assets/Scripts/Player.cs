using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public string Email;
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

    }

    protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref Email);
        base.OnSynchronize(ref serializer);
    }

    async void LoadUser()
    {
        User = await User.Get(Email);
    }

    public override void OnNetworkSpawn()
    {
        LoadUser();
    }
}