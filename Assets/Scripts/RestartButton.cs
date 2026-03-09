using UnityEngine;
using Unity.Netcode;

public class RestartButton : MonoBehaviour
{
    public void OnRestartClicked()
    {
        var gm = FindFirstObjectByType<GameManager>(); // grab manager

        if (NetworkManager.Singleton.IsServer) // host-only restart
            gm.RestartGame();
    }
}