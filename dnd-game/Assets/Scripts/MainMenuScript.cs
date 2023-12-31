using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

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

        if (!NetworkManager.Singleton.StartClient())
        {
            GameObject.Find("Error").GetComponent<TMP_Text>().text = "Failed to connect to server";
            return;
        }
    }

    public void OnClickHost()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            "127.0.0.1",
            port,
            "0.0.0.0"
        );

        if (!NetworkManager.Singleton.StartHost())
        {
            GameObject.Find("Error").GetComponent<TMP_Text>().text = "Failed to start host";
            return;
        }

        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

}
