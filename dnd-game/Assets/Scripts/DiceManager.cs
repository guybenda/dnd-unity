using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DndCommon;
using Unity.Netcode;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance { get; private set; }

    DiceMap<Mesh> meshesLow;
    DiceMap<Mesh> meshesMed;
    DiceMap<Mesh> meshesHigh;

    GameObject diePrefab;
    DiceMap<Vector3[]> facepoints;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        diePrefab = Resources.Load<GameObject>("Dice/Prefabs/die");
        meshesLow = DiceMap.FromResources<Mesh>("Dice/Meshes/low");
        meshesMed = DiceMap.FromResources<Mesh>("Dice/Meshes/med");
        meshesHigh = DiceMap.FromResources<Mesh>("Dice/Meshes/high");

        MakeFacePoints();
    }

    Quaternion getRandomRotation()
    {
        return Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    Vector3 getRandomAngularVelocity()
    {
        return Random.rotation.eulerAngles * Random.Range(-8f, 8f);
    }

    Vector3 getRandomVelocity()
    {
        return Random.insideUnitSphere * 1.5f;
    }

    GameObject instantiateDie(Vector3 position)
    {
        var die = Instantiate(diePrefab, position, getRandomRotation());

        return die;
    }

    public DiceScript MakeDie(DiceType type, User roller, Vector3 position = default, Vector3 velocity = default, uint rollId = 0)
    {
        var die = instantiateDie(position);

        var diceScript = die.GetComponent<DiceScript>();

        diceScript.userDice = roller?.Dice ?? UserDice.Default();
        diceScript.Type = type;
        diceScript.RollId = rollId;
        diceScript.RollerEmail = roller?.Email.ToString() ?? "";

        var rb = die.GetComponent<Rigidbody>();
        rb.angularVelocity = getRandomAngularVelocity();
        rb.velocity = getRandomVelocity() + velocity;

        die.GetComponent<NetworkObject>().Spawn();

        return diceScript;
    }

    public void ResetDicePosition(GameObject die, Vector3 position)
    {
        die.transform.position = position;

        var rb = die.GetComponent<Rigidbody>();
        rb.angularVelocity = getRandomAngularVelocity();
    }

    void MakeFacePoints()
    {
        DiceMap<Vector3[]> vertices = new();

        foreach (var (type, mesh) in meshesLow.m)
        {
            vertices[type] = mesh.vertices;
        }

        facepoints = new();

        {
            var v = vertices[DiceType.D4];
            facepoints[DiceType.D4] = new Vector3[4]{
                AveragePoints(v[0], v[1], v[2]),
                AveragePoints(v[3], v[4], v[5]),
                AveragePoints(v[6], v[7], v[8]),
                AveragePoints(v[9], v[10], v[11])
            };
        }
        {
            var v = vertices[DiceType.D6];
            facepoints[DiceType.D6] = new Vector3[6]{
                AveragePoints(v[4], v[5], v[6], v[7]),
                AveragePoints(v[12], v[13], v[14], v[15]),
                AveragePoints(v[16], v[17], v[18], v[19]),
                AveragePoints(v[8], v[9], v[10], v[11]),
                AveragePoints(v[20], v[21], v[22], v[23]),
                AveragePoints(v[0], v[1], v[2], v[3]),
            };
        }
        {
            var v = vertices[DiceType.D8];
            facepoints[DiceType.D8] = new Vector3[8]{
                AveragePoints(v[15], v[16], v[17]),
                AveragePoints(v[21], v[22], v[23]),
                AveragePoints(v[12], v[13], v[14]),
                AveragePoints(v[18], v[19], v[20]),
                AveragePoints(v[0], v[1], v[2]),
                AveragePoints(v[6], v[7], v[8]),
                AveragePoints(v[3], v[4], v[5]),
                AveragePoints(v[9], v[10], v[11]),
            };
        }
        {
            var v = vertices[DiceType.D10];
            facepoints[DiceType.D10] = new Vector3[10]{
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
            facepoints[DiceType.D100] = facepoints[DiceType.D10];
        }
        {
            var v = vertices[DiceType.D12];
            facepoints[DiceType.D12] = new Vector3[12]{
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
        }
        {
            var v = vertices[DiceType.D20];
            facepoints[DiceType.D20] = new Vector3[20]{
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
        }
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

    public int GetResultOf(DiceType type, Transform transform)
    {
        var points = facepoints[type];

        var lowestIdx = 0;
        var lowestPos = transform.TransformPoint(points[0]).y;

        for (int i = 1; i < points.Length; i++)
        {
            var pos = transform.TransformPoint(points[i]);
            if (pos.y < lowestPos)
            {
                lowestIdx = i;
                lowestPos = pos.y;
            }
        }

        if (type == DiceType.D100)
        {
            return (lowestIdx + 1) * 10 % 100;
        }

        return lowestIdx + 1;
    }

    public Mesh DieMesh(DiceType type, DiceQuality quality = DiceQuality.High)
    {
        return quality switch
        {
            DiceQuality.Low => meshesLow[type],
            DiceQuality.Medium => meshesMed[type],
            DiceQuality.High => meshesHigh[type],
            _ => null
        };
    }

    public Mesh DieColliderMesh(DiceType type)
    {
        return meshesLow[type];
    }
}

public class DiceMap<V>
{
    public readonly Dictionary<DiceType, V> m = new();

    public DiceMap()
    {
    }

    public DiceMap(V d4, V d6, V d8, V d10, V d12, V d20, V d100)
    {
        m[DiceType.D4] = d4;
        m[DiceType.D6] = d6;
        m[DiceType.D8] = d8;
        m[DiceType.D10] = d10;
        m[DiceType.D12] = d12;
        m[DiceType.D20] = d20;
        m[DiceType.D100] = d100;
    }

    public V this[DiceType type]
    {
        get
        {
            return m[type];
        }
        set
        {
            m[type] = value;
        }
    }
}

public class DiceMap
{
    public static DiceMap<T> FromResources<T>(string path) where T : Object
    {
        return new DiceMap<T>(
            Resources.Load<T>($"{path}/d4"),
            Resources.Load<T>($"{path}/d6"),
            Resources.Load<T>($"{path}/d8"),
            Resources.Load<T>($"{path}/d10"),
            Resources.Load<T>($"{path}/d12"),
            Resources.Load<T>($"{path}/d20"),
            Resources.Load<T>($"{path}/d100")
        );
    }
}