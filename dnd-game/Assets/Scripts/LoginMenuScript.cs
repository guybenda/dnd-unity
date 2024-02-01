using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DndFirebase;
using TMPro;
using UnityEngine;

public class LoginMenuScript : MonoBehaviour
{
    public GameObject LoginMenu;
    public GameObject SignUpMenu;
    public GameObject SelectionMenu;
    public GameObject Loader;
    public TMP_Text ErrorText;


    void Start()
    {
        ActivateSelectionMenu();
        SetLoader(false);
    }

    void Update()
    {

    }

    void Awake()
    {
        if (AuthManager.Instance.IsLoggedIn())
        {
            SceneTransitionManager.Instance.MainMenu();
        }
    }

    void ActivateLoginMenu()
    {
        LoginMenu.SetActive(true);
        SignUpMenu.SetActive(false);
        SelectionMenu.SetActive(false);
    }

    void ActivateSignUpMenu()
    {
        LoginMenu.SetActive(false);
        SignUpMenu.SetActive(true);
        SelectionMenu.SetActive(false);
    }

    void ActivateSelectionMenu()
    {
        LoginMenu.SetActive(false);
        SignUpMenu.SetActive(false);
        SelectionMenu.SetActive(true);
    }

    public void OnClickLoginMenu()
    {
        ActivateLoginMenu();
    }

    public void OnClickSignUpMenu()
    {
        ActivateSignUpMenu();
    }

    public async void OnClickLogin()
    {
        SetLoader(true);

        var email = InputText(LoginMenu, "Email");
        var password = InputText(LoginMenu, "Password");

        var (result, error) = await AuthManager.Instance.Login(email, password);

        SetLoader(false);

        if (!result)
        {
            SetErrorText("Failed to sign in: " + error);
        }
        else
        {
            SceneTransitionManager.Instance.MainMenu();
        }
    }

    public async void OnClickSignUp()
    {
        SetLoader(true);

        var email = InputText(SignUpMenu, "Email");
        var password = InputText(SignUpMenu, "Password");
        var displayName = InputText(SignUpMenu, "Name");

        var (result, error) = await AuthManager.Instance.SignUp(displayName, email, password);

        SetLoader(false);

        if (!result)
        {
            SetErrorText("Failed to sign up: " + error);
        }
        else
        {
            SceneTransitionManager.Instance.MainMenu();
        }
    }

    public void OnClickBack()
    {
        ActivateSelectionMenu();
    }

    string InputText(GameObject parent, string name)
    {
        return parent.transform.Find(name).GetComponent<TMP_InputField>().text;
    }

    void SetErrorText(string text)
    {
        ErrorText.text = text;
    }

    void SetLoader(bool active)
    {
        Loader.SetActive(active);
        if (active)
        {
            ErrorText.text = "";
        }
    }
}
