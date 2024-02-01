using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using UnityEngine;

public class DiceMaterialManager : MonoBehaviour
{
    const int SIZE = 2048;
    public static DiceMaterialManager Instance { get; private set; }

    readonly Dictionary<UserDice, DiceMaterial> diceMaterials = new();

    public Material diceMaterial { get; private set; }
    public Material diceTextureMaterial { get; private set; }
    public Texture2D nonmetal { get; private set; }
    public Texture2D metal { get; private set; }


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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        diceMaterial = Resources.Load<Material>("Dice/DiceMaterial");
        diceTextureMaterial = Resources.Load<Material>("Dice/DiceTextureMaterial");

        nonmetal = Resources.Load<Texture2D>("Dice/Textures/nonmetal");
        metal = Resources.Load<Texture2D>("Dice/Textures/metal");
    }

    DiceMaterial New(UserDice userDice)
    {
        var diceMat = new DiceMaterial()
        {
            userDice = userDice
        };

        diceMaterials[userDice] = diceMat;

        return diceMat;
    }

    public Material GenerateMaterialFromUserDice(UserDice userDice)
    {
        if (diceMaterials.TryGetValue(userDice, out var diceMat))
        {
            return diceMat.material;
        }

        diceMat = New(userDice);
        diceMat.Draw();

        return diceMat.material;
    }

    public void Dispose(DiceMaterial diceMat)
    {
        diceMaterials.Remove(diceMat.userDice);
        diceMat.renderTexture.Release();
    }

}
