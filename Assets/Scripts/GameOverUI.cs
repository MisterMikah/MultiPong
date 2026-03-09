using Unity.Netcode;
using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI winnerText;

    GameManager gm;

    void Start()
    {
        gm = FindFirstObjectByType<GameManager>(); // grab manager from scene

        gameOverPanel.SetActive(false); // hidden until GameOver true

        gm.GameOver.OnValueChanged += OnGameOverChanged;
        gm.Winner.OnValueChanged += (_, __) => RefreshText(); // update label if winner changes

        OnGameOverChanged(false, gm.GameOver.Value); // if late-join
    }

    new void OnDestroy()
    {
        if (gm == null) return;
        gm.GameOver.OnValueChanged -= OnGameOverChanged;
    }

    void OnGameOverChanged(bool oldVal, bool newVal)
    {
        gameOverPanel.SetActive(newVal); // show/hide overlay
        if (newVal) RefreshText();
    }

    void RefreshText()
    {
        if (gm.Winner.Value == 1)
            winnerText.text = "Player 1 (Left) Wins!";
        else if (gm.Winner.Value == 2)
            winnerText.text = "Player 2 (Right) Wins!";
        else
            winnerText.text = "Game Over!"; // just in case
    }
}