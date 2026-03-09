using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform leftGoal;
    [SerializeField] Transform rightGoal;
    [SerializeField] float readyDelay = 3f;

    Coroutine serveRoutine;     
    bool awaitingServe = false; 
    BallServer ball;            

    public NetworkVariable<int> LeftScore =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // server writes, everyone reads

    public NetworkVariable<int> RightScore =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> GameOver =
        new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // game over state

    public NetworkVariable<int> Winner =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);  // 1 left, 2 right

    [SerializeField] int pointsToWin = 3;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // only server spawns shared objects

        var go = Instantiate(ballPrefab);           // spawn locally on server
        go.GetComponent<NetworkObject>().Spawn();   // replicate to clients
        ball = go.GetComponent<BallServer>();       // cache script ref

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected; // start game once both players join
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected; 
    }

    void OnClientConnected(ulong _)
    {
        if (!IsServer) return;

        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2 && ball != null && !awaitingServe)
        {
            awaitingServe = true; // block scoring until serve starts
            ball.StartRoundAfterDelay(readyDelay, serveRight: Random.value < 0.5f); // first serve
            Invoke(nameof(ClearAwaitingServe), readyDelay + 0.1f); //  buffer
        }
    }

    void ClearAwaitingServe()
    {
        awaitingServe = false; // allow scoring again
    }

    void Update()
    {
        if (!IsServer) return;     
        if (ball == null) return;
        if (GameOver.Value) return; 
        if (awaitingServe) return;  

        float x = ball.transform.position.x;

        if (x < leftGoal.position.x)
        {
            RightScore.Value += 1;              // right player scored
            CheckWinOrServe(serveRight: true);  // serve toward left next
        }
        else if (x > rightGoal.position.x)
        {
            LeftScore.Value += 1;               // left player scored
            CheckWinOrServe(serveRight: false); // serve toward right next
        }
    }

    public void RestartGame()
    {
        if (!IsServer) return; // only host can restart

        LeftScore.Value = 0;
        RightScore.Value = 0;

        Winner.Value = 0;
        GameOver.Value = false;

        ball.StartRoundAfterDelay(readyDelay, serveRight: Random.value < 0.5f); // new match serve
    }

    void CheckWinOrServe(bool serveRight)
    {
        if (LeftScore.Value >= pointsToWin)
        {
            Winner.Value = 1;             // left wins
            GameOver.Value = true;        // trigger UI on both clients
            ball.FreezeNow();             // stop ball
            return;
        }

        if (RightScore.Value >= pointsToWin)
        {
            Winner.Value = 2;             // right wins
            GameOver.Value = true;
            ball.FreezeNow();
            return;
        }

        if (serveRoutine != null) StopCoroutine(serveRoutine); // prevent stacked delays
        serveRoutine = StartCoroutine(ServeAfterDelay(serveRight));
    }

    System.Collections.IEnumerator ServeAfterDelay(bool serveRight)
    {
        awaitingServe = true;

        ball.FreezeNow(); // show ball centered during “get ready”

        yield return new WaitForSeconds(readyDelay);

        ball.StartRoundAfterDelay(0f, serveRight);

        awaitingServe = false;
    }
}