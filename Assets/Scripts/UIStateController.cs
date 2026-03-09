using Unity.Netcode;
using UnityEngine;

public class UIStateController : MonoBehaviour
{
    [SerializeField] GameObject networkPanel;
    [SerializeField] GameObject scorePanel;

    void Start()
    {
        networkPanel.SetActive(true);  // show network UI first
        scorePanel.SetActive(false);   // hide gameplay UI

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected; // change UI when both players exist
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2) // both joined
        {
            networkPanel.SetActive(false); // hide host/client buttons
            scorePanel.SetActive(true);    // show score
        }
    }
}