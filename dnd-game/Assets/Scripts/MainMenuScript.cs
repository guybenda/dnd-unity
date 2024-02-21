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
    public TMP_Text ErrorText;
    public GameObject Loader;

    SyncExecutor exec = new();
    const ushort port = 42069;

    static readonly Regex ipRegex = new(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$");

    void Start()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.NetworkConfig.ConnectionData =
            Encoding.ASCII.GetBytes(AuthManager.Instance.CurrentUser.Email.ToString());

        SetLoading(false);
        SetError();
    }

    void Update()
    {
        exec.Execute(1);
    }

    void Awake()
    {

    }

    public void OnClickConnect()
    {
        SetError();

        var ip = GetComponentInChildren<TMP_InputField>().text.Trim();
        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = "127.0.0.1";
        }

        if (!ipRegex.IsMatch(ip))
        {
            SetError("Invalid IP address");
            return;
        }

        SetLoading(true);

        exec.Enqueue(() =>
        {
            Connect(ip);
        });
    }

    void Connect(string ip)
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            ip,
            port
        );

        if (!NetworkManager.Singleton.StartClient())
        {
            SetError("Failed to connect to server");
            SetLoading(false);
            return;
        }

        NetworkManager.Singleton.OnConnectionEvent += ConnectEvent;
    }

    void ConnectEvent(NetworkManager source, ConnectionEventData data)
    {
        NetworkManager.Singleton.OnConnectionEvent -= ConnectEvent;

        if (data.EventType == ConnectionEvent.ClientConnected)
        {
            SceneTransitionManager.Instance.RegisterDisconnectCallback();
            return;
        }

        SetLoading(false);
        SetError("Failed to connect to server");

    }

    public void OnClickHost()
    {
        SetLoading(true);
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

    void SetError(string error = "")
    {
        ErrorText.text = error;
    }

    void SetLoading(bool isLoading)
    {
        Loader.SetActive(isLoading);
    }
}
