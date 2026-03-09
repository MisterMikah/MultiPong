using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class ShowLocalIP : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ipText;

    void Awake()
    {
        if (ipText == null)
            ipText = GetComponent<TextMeshProUGUI>(); // if script is on the text object
    }

    void Start()
    {
        if (ipText == null)
        {
            Debug.LogError("ShowLocalIP: ipText missing"); // inspector/wiring issue
            return;
        }

        ipText.text = "IP: " + GetLocalIPv4(); // display LAN IP
    }

    string GetLocalIPv4()
    {
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                return ip.ToString(); // IPv4 only
        }
        return "Unknown";
    }
}