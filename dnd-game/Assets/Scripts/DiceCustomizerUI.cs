using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public Button SaveButton;

    public GameObject Loader;

    DiceMaterial diceMaterial;

    bool shouldInit = false;

    // Start is called before the first frame update
    void Start()
    {
        Loader.SetActive(true);

        AuthManager.Instance.AddOnUserLoadedListener(OnLoadUser);

    }

    void OnLoadUser(User user)
    {

        // This exists because Render Textures can only be created on the main thread
        shouldInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = Die.transform.rotation.eulerAngles;
        rotation.y += Time.deltaTime * 10;
        Die.transform.rotation = Quaternion.Euler(rotation);

        if (shouldInit)
        {
            shouldInit = false;

            var userDice = new UserDice(AuthManager.Instance.CurrentUser.Dice);

            Color.RGBToHSV(userDice.MainColor, out var h1, out var s1, out var v1);
            H1.value = h1;
            S1.value = s1;
            V1.value = v1;

            Color.RGBToHSV(userDice.SecondaryColor, out var h2, out var s2, out var v2);
            H2.value = h2;
            S2.value = s2;
            V2.value = v2;
            A2.value = userDice.SecondaryColor.a;

            Color.RGBToHSV(userDice.NumbersColor, out var h3, out var s3, out var v3);
            H3.value = h3;
            S3.value = s3;
            V3.value = v3;

            Smoothness.value = userDice.Smoothness;
            Metallic.value = userDice.Metallic ? 1 : 0;

            // TODO: dispose
            diceMaterial = new()
            {
                userDice = userDice
            };

            Die.GetComponent<MeshRenderer>().material = diceMaterial.material;

            var dieRotation = Die.transform.rotation;
            var dieEuler = dieRotation.eulerAngles;
            dieEuler.y = UnityEngine.Random.Range(0, 360);
            Die.transform.rotation = Quaternion.Euler(dieEuler);

            StartCoroutine(Drawer());

            Loader.SetActive(false);
        }
    }

    void Awake()
    {

    }

    void OnDestroy()
    {
        StopAllCoroutines();
        DiceMaterialManager.Instance.Dispose(diceMaterial);
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

        SaveButton.GetComponentInChildren<TMP_Text>().text = "Saving...";

        var user = AuthManager.Instance.CurrentUser;
        user.Dice = diceMaterial.userDice;
        try
        {
            await user.Save();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            ErrorText.text = e.Message;
            SaveButton.GetComponentInChildren<TMP_Text>().text = "Save";
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
            diceMaterial.userDice.MainColor = Color.HSVToRGB(H1.value, S1.value, V1.value);

            var secondary = Color.HSVToRGB(H2.value, S2.value, V2.value);
            secondary.a = A2.value;
            diceMaterial.userDice.SecondaryColor = secondary;

            diceMaterial.userDice.NumbersColor = Color.HSVToRGB(H3.value, S3.value, V3.value);

            diceMaterial.userDice.Smoothness = Smoothness.value;
            diceMaterial.userDice.Metallic = Metallic.value > 0.5f;

            diceMaterial.Draw();
            yield return null;
            yield return null;
        }
    }
}
