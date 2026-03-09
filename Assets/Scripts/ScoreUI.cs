using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI leftText;
    [SerializeField] TextMeshProUGUI rightText;

    void Start()
    {
        var gm = FindFirstObjectByType<GameManager>(); // grab manager

        gm.LeftScore.OnValueChanged += (_, newVal) =>
            leftText.text = newVal.ToString(); // update from NetworkVariable

        gm.RightScore.OnValueChanged += (_, newVal) =>
            rightText.text = newVal.ToString();

        leftText.text = gm.LeftScore.Value.ToString(); 
        rightText.text = gm.RightScore.Value.ToString();
    }
}