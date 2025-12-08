using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplayerScript : MonoBehaviour
{
    public int playerIndex;
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text playerScore;

    public void Initialize(int index, string name, int startingScore)
    {
        playerIndex = index;
        playerName.text = name;
        playerScore.text = startingScore.ToString();
    }

    public void UpdateScores(int newScore)
    {
        playerScore.text = newScore.ToString();
    }
}
