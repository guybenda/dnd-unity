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

    Material material;

    Texture2D main;
    Texture2D secondary;
    Texture2D numbers;
    Texture2D nonmetal;
    Texture2D metal;

    NativeArray<Color32> mainData;
    NativeArray<Color32> secondaryData;
    NativeArray<Color32> numbersData;


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

        DontDestroyOnLoad(this);

        material = Resources.Load<Material>("Dice/DiceMaterial");

        main = Resources.Load<Texture2D>("Dice/Textures/main");
        secondary = Resources.Load<Texture2D>("Dice/Textures/secondary");
        numbers = Resources.Load<Texture2D>("Dice/Textures/numbers");
        nonmetal = Resources.Load<Texture2D>("Dice/Textures/nonmetal");
        metal = Resources.Load<Texture2D>("Dice/Textures/metal");

        mainData = main.GetPixelData<Color32>(0);
        secondaryData = secondary.GetPixelData<Color32>(0);
        numbersData = numbers.GetPixelData<Color32>(0);
    }

    public Material New(UserDice userDice)
    {
        var diceMat = new DiceMaterial
        {
            userDice = userDice,
            renderTexture = new RenderTexture(SIZE, SIZE, 0),
            material = new Material(material),
        };

        // diceMat.renderTexture.Create();

        diceMaterials[userDice] = diceMat;
        material.SetTexture("_MainTex", diceMat.renderTexture);


        return diceMat.material;
    }

    public void Draw(UserDice userDice)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        var diceMaterial = diceMaterials[userDice];

        if (diceMaterial == null)
        {
            New(userDice);
            diceMaterial = diceMaterials[userDice];
        }

        diceMaterial.material.SetFloat("_Glossiness", userDice.Smoothness);
        diceMaterial.material.SetColor("_Color1", userDice.MainColor);
        diceMaterial.material.SetColor("_Color2", userDice.SecondaryColor);
        diceMaterial.material.SetColor("_Color3", userDice.NumbersColor);

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Draw took {stopwatch.ElapsedMilliseconds}ms");
    }

}
