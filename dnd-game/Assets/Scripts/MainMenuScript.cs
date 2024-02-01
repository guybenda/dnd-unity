using System.Collections;
using System.Collections.Generic;
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
    const ushort port = 42069;

    static readonly Regex ipRegex = new(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$");

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(AuthManager.Instance.CurrentUser.Email.ToString());

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
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true;

        var gameManager = new GameObject("GameManager").AddComponent<NetworkObject>().gameObject.AddComponent<GameManager>();

        if (!NetworkManager.Singleton.StartHost())
        {
            GameObject.Find("Error").GetComponent<TMP_Text>().text = "Failed to start host";
            Destroy(gameManager.gameObject);
            return;
        }

        gameManager.ConnectPlayer(NetworkManager.Singleton.LocalClientId, AuthManager.Instance.CurrentUser.Email.ToString(), isAllowedToRoll: true);
        DontDestroyOnLoad(gameManager.gameObject);
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);

        // gameManager.GetComponent<NetworkObject>().Spawn();
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
