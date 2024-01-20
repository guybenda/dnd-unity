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
            // renderTexture = new RenderTexture(SIZE, SIZE, 0),
            buffer = new NativeArray<Color32>(SIZE * SIZE, Allocator.Persistent),
            texture = new Texture2D(SIZE, SIZE),
            material = new Material(material),
        };

        diceMaterials[userDice] = diceMat;

        return diceMat.material;
    }

    // This is very stupid. This should be done in a shader.
    public void Draw(UserDice userDice)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        var diceMaterial = diceMaterials[userDice];

        if (diceMaterial == null)
        {
            New(userDice);
            diceMaterial = diceMaterials[userDice];
        }

        var buffer = diceMaterial.buffer;

        for (int i = 0; i < mainData.Length; i++)
        {
            // One last chance to look away.
            var step1 = mainData[i] * userDice.MainColor;
            var step2 = step1 * (1 - userDice.SecondaryColor.a);
            var step3 = secondaryData[i] * userDice.SecondaryColor * userDice.SecondaryColor.a;
            var step4 = step2 + step3;

            var step5 = step4 * (1 - userDice.NumbersColor.a);
            var step6 = numbersData[i] * userDice.NumbersColor * userDice.NumbersColor.a;
            var step7 = step5 + step6;

            buffer[i] = step7;
        }

        diceMaterial.texture.SetPixelData(buffer, 0);
        diceMaterial.texture.Apply();

        diceMaterial.material.SetTexture("_MainTex", diceMaterial.texture);
        diceMaterial.material.SetFloat("_Glossiness", userDice.Smoothness);

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Draw took {stopwatch.ElapsedMilliseconds}ms");
    }

}
