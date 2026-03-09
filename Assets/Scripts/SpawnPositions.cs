using Unity.Netcode;
using UnityEngine;

public class SpawnPositions : MonoBehaviour
{
    [SerializeField] Transform leftSpawn;
    [SerializeField] Transform rightSpawn;

    void Awake()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += Approval; // spawn pos on connect
        NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true; // enable approval path
    }

    void Approval(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        res.Approved = true;
        res.CreatePlayerObject = true; // spawn player prefab

        int count = NetworkManager.Singleton.ConnectedClientsList.Count; // host is first
        Transform spawn = (count == 0) ? leftSpawn : rightSpawn;        // left for host, right for client

        res.Position = spawn.position;
        res.Rotation = spawn.rotation;

        res.Pending = false; // finalize approval
    }
}