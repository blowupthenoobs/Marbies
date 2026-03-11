using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinalScoreDisplayScript : MonoBehaviour
{
    [SerializeField] TMP_Text rank;
    [SerializeField] TMP_Text username;
    [SerializeField] TMP_Text finalScore;

    public void InitializeDisplay(string placement, string score, string endingScore)
    {
        rank.text = placement;
        username.text = score;
        finalScore.text = endingScore;
    }
}
