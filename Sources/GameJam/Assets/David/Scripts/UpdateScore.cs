using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateScore : MonoBehaviour
{

    public TextMeshProUGUI tmpScore;
   
    public void UpdateScoreInScreen(int score)
    {
        Debug.Log("UpdateScoreInScreen");
        tmpScore.SetText("Completed: " + score + "%");
    }
}
