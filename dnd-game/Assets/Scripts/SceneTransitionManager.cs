
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

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
    }



    public void RegisterDisconnectCallback()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }



    void OnClientDisconnect(ulong _)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        MainMenu();
    }


    public void MainMenu(Action<AsyncOperation> onComplete = null)
    {
        NetworkManager.Singleton.Shutdown();
        var loader = SceneManager.LoadSceneAsync("MenuScene");

        if (onComplete != null)
        {
            loader.completed += onComplete;
        }
    }

    public void DiceCustomizer()
    {
        SceneManager.LoadScene("DiceCustomizerScene");
    }

    public void LoginScreen()
    {
        SceneManager.LoadScene("LoginScene");
    }
}