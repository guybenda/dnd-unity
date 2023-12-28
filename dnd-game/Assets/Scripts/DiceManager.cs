using System.Collections;
using System.Collections.Generic;
using DndCommon;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    Material[] diceMaterials;

    GameObject d4Prefab;
    GameObject d6Prefab;
    GameObject d8Prefab;
    GameObject d10Prefab;
    GameObject d12Prefab;
    GameObject d20Prefab;
    GameObject d100Prefab;

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
        d4Prefab = Resources.Load<GameObject>("Dice/Prefabs/d4");
        d6Prefab = Resources.Load<GameObject>("Dice/Prefabs/d6");
        d8Prefab = Resources.Load<GameObject>("Dice/Prefabs/d8");
        d10Prefab = Resources.Load<GameObject>("Dice/Prefabs/d10");
        d12Prefab = Resources.Load<GameObject>("Dice/Prefabs/d12");
        d20Prefab = Resources.Load<GameObject>("Dice/Prefabs/d20");
        d100Prefab = Resources.Load<GameObject>("Dice/Prefabs/d100");

        diceMaterials = Resources.LoadAll<Material>("Dice/Materials");
    }

    Quaternion getRandomRotation()
    {
        return Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }

    DiceType getRandomDiceType()
    {
        return Dice.types[Random.Range(0, 7)];
    }

    GameObject instantiateDie(DiceType type, Vector3? position = null)
    {
        GameObject die = null;

        switch (type)
        {
            case DiceType.D4:
                die = d4Prefab;
                break;
            case DiceType.D6:
                die = d6Prefab;
                break;
            case DiceType.D8:
                die = d8Prefab;
                break;
            case DiceType.D10:
                die = d10Prefab;
                break;
            case DiceType.D12:
                die = d12Prefab;
                break;
            case DiceType.D20:
                die = d20Prefab;
                break;
            case DiceType.D100:
                die = d100Prefab;
                break;
        }

        return Instantiate(die, position ?? Vector3.zero, getRandomRotation());
    }

    public GameObject MakeDie(DiceType type, int? materialIndex = null, Vector3? position = null)
    {
        var die = instantiateDie(type, position);

        Material material;
        if (materialIndex.HasValue)
        {
            material = diceMaterials[materialIndex.Value];
        }
        else
        {
            material = diceMaterials[Random.Range(0, diceMaterials.Length)];
        }

        die.GetComponent<MeshRenderer>().material = material;

        return die;
    }



    public void MakeD6()
    {
        var die = MakeDie(DiceType.D6, position: new(0, 3, 0));
    }

    public void MakeRandom()
    {
        var die = MakeDie(getRandomDiceType(), position: new(0, 3, 0));
    }
}
