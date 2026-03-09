using System.Text.RegularExpressions;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] TMP_InputField ipInputField; // client target IP input
    [SerializeField] ushort port = 7777;          // shared port

    UnityTransport transport;

    void Awake()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>(); // transport component
    }

    public void StartHost()
    {
        transport.SetConnectionData("0.0.0.0", port);
        bool ok = NetworkManager.Singleton.StartHost();
        Debug.Log(ok ? $"HOST started on port {port}" : "Failed to start HOST");
    }

    public void StartClient()
    {
        string raw = ipInputField != null ? ipInputField.text : "";
        string ip = SanitizeIp(raw);                                // strip bad chars

        if (string.IsNullOrWhiteSpace(ip))
            ip = "127.0.0.1"; // local testing fallback

        Debug.Log($"CLIENT trying to connect to '{ip}' (raw input was '{raw}') on port {port}");

        transport.SetConnectionData(ip, port); // point transport at host
        bool ok = NetworkManager.Singleton.StartClient();
        Debug.Log(ok ? "CLIENT started" : "Failed to start CLIENT");
    }

    static string SanitizeIp(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";

        s = s.Trim();
        s = Regex.Replace(s, @"[^\d\.]", "");

        return s;
    }
}