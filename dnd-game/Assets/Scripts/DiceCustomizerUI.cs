using System;
using System.Collections;
using System.Collections.Generic;
using DndFirebase;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class DiceCustomizerUI : MonoBehaviour
{
    public GameObject Die;
    public Slider H1;
    public Slider S1;
    public Slider V1;

    public Slider H2;
    public Slider S2;
    public Slider V2;
    public Slider A2;

    public Slider H3;
    public Slider S3;
    public Slider V3;

    public Slider Smoothness;
    public Slider Metallic;

    public TMP_Text ErrorText;

    UserDice userDice = UserDice.Default();

    Material diceMaterial;

    // Start is called before the first frame update
    void Start()
    {
        diceMaterial = DiceMaterialManager.Instance.New(userDice);

        Die.GetComponent<MeshRenderer>().material = diceMaterial;

        StartCoroutine(Drawer());
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = Die.transform.rotation.eulerAngles;
        rotation.y += Time.deltaTime * 10;
        Die.transform.rotation = Quaternion.Euler(rotation);
    }

    void Awake()
    {

    }

    public void OnClickRandom()
    {
        H1.value = UnityEngine.Random.value;
        S1.value = UnityEngine.Random.value;
        V1.value = UnityEngine.Random.value;

        H2.value = UnityEngine.Random.value;
        S2.value = UnityEngine.Random.value;
        V2.value = UnityEngine.Random.value;
        A2.value = math.max(UnityEngine.Random.value, UnityEngine.Random.value);

        H3.value = UnityEngine.Random.value;
        S3.value = UnityEngine.Random.value;
        V3.value = UnityEngine.Random.value;

        Smoothness.value = UnityEngine.Random.value;

        Metallic.value = UnityEngine.Random.value > 0.5f ? 1 : 0;
    }

    public async void OnClickSave()
    {
        ErrorText.text = "";

        if (!AuthManager.Instance.IsLoggedIn())
        {
            ErrorText.text = "You must be logged in to save your dice.";
            return;
        }

        var user = AuthManager.Instance.CurrentUser;
        user.Dice = userDice;
        try
        {
            await user.Save();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ErrorText.text = e.Message;
            return;
        }

        SceneTransitionManager.Instance.MainMenu();
    }

    public void OnClickExit()
    {
        SceneTransitionManager.Instance.MainMenu();
    }

    IEnumerator Drawer()
    {
        while (true)
        {
            userDice.MainColor = Color.HSVToRGB(H1.value, S1.value, V1.value);

            var secondary = Color.HSVToRGB(H2.value, S2.value, V2.value);
            secondary.a = A2.value;
            userDice.SecondaryColor = secondary;

            userDice.NumbersColor = Color.HSVToRGB(H3.value, S3.value, V3.value);

            userDice.Smoothness = Smoothness.value;
            userDice.Metallic = Metallic.value > 0.5f;

            DiceMaterialManager.Instance.Draw(userDice);
            yield return null;
            yield return null;
        }
    }
}
