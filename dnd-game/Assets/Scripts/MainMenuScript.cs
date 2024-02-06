using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DndFirebase;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    SyncExecutor exec = new();
    const ushort port = 42069;

    static readonly Regex ipRegex = new(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$");

    void Start()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.NetworkConfig.ConnectionData =
            Encoding.ASCII.GetBytes(AuthManager.Instance.CurrentUser.Email.ToString());
    }

    void Update()
    {
        exec.Execute(1);
    }

    public void OnClickConnect()
    {
        var ip = GetComponentInChildren<TMP_InputField>().text;
        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = "127.0.0.1";
        }

        if (!ipRegex.IsMatch(ip)) return;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            ip,
            port
        );

        if (!NetworkManager.Singleton.StartClient())
        {
            GameObject.Find("Error").GetComponent<TMP_Text>().text = "Failed to connect to server";
            return;
        }

        SceneTransitionManager.Instance.RegisterDisconnectCallback();
    }

    public void OnClickHost()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            "127.0.0.1",
            port,
            "0.0.0.0"
        );

        var gameManagerPrefab = Resources.Load<GameObject>("GameManager");
        if (gameManagerPrefab == null)
        {
            Debug.LogError("GameManager prefab not found");
            return;
        }

        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Single).completed += OnGameLoaded;
    }

    private void OnGameLoaded(AsyncOperation op)
    {
        if (!NetworkManager.Singleton.StartHost())
        {
            SceneTransitionManager.Instance.MainMenu((_) =>
            {
                GameObject.Find("Error").GetComponent<TMP_Text>().text = "Failed to start host";
            });
            return;
        }
    }

    public void OnClickCustomize()
    {
        SceneTransitionManager.Instance.DiceCustomizer();
    }

    public void OnClickLogout()
    {
        AuthManager.Instance.Logout();
        SceneTransitionManager.Instance.LoginScreen();
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
