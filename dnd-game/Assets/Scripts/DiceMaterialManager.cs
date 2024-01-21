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

    Material diceMaterial;
    Material diceTextureMaterial;

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

        diceMaterial = Resources.Load<Material>("Dice/DiceMaterial");
        diceTextureMaterial = Resources.Load<Material>("Dice/DiceTextureMaterial");

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
            renderTexture = new CustomRenderTexture(SIZE, SIZE, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
            {
                material = new Material(diceTextureMaterial),
                initializationMode = CustomRenderTextureUpdateMode.OnLoad,
                updateMode = CustomRenderTextureUpdateMode.OnDemand,

            },
            material = new Material(diceMaterial),
        };

        // diceMat.renderTexture.Create();

        diceMaterials[userDice] = diceMat;
        diceMat.material.SetTexture("_MainTex", diceMat.renderTexture);
        diceMat.material.SetTexture("_MetallicGlossMap", userDice.Metallic ? metal : nonmetal);

        return diceMat.material;
    }

    public Material Draw(UserDice userDice)
    {
        var diceMat = diceMaterials[userDice];

        if (diceMat == null)
        {
            New(userDice);
            diceMat = diceMaterials[userDice];
        }

        diceMat.renderTexture.material.SetColor("_Color1", userDice.MainColor);
        diceMat.renderTexture.material.SetColor("_Color2", userDice.SecondaryColor);
        diceMat.renderTexture.material.SetColor("_Color3", userDice.NumbersColor);

        diceMat.renderTexture.Update();

        diceMat.material.SetFloat("_GlossMapScale", userDice.Smoothness);
        diceMat.material.SetTexture("_MetallicGlossMap", userDice.Metallic ? metal : nonmetal);


        return diceMat.material;
    }

}
