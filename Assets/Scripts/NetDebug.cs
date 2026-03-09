using Unity.Netcode;
using UnityEngine;

public class NetDebug : MonoBehaviour
{
    void Start()
    {
        var nm = NetworkManager.Singleton;

        nm.OnServerStarted += () =>
            Debug.Log("[NET] Server started");

        nm.OnClientConnectedCallback += (id) =>
            Debug.Log($"[NET] Client connected: {id}");

        nm.OnClientDisconnectCallback += (id) =>
            Debug.Log($"[NET] Client disconnected: {id}");

        nm.OnTransportFailure += () =>
            Debug.Log("[NET] Transport failure");
    }
}