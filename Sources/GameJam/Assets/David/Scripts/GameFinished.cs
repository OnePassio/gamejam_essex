using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameFinished : MonoBehaviour
{
    public TextMeshProUGUI finalTextMessage;

    public void GameOver()
    {
        ShowFinalMessage("GAME OVER!");
    }

    public void GameWin()
    {
        ShowFinalMessage("WINNER!");
    }

    public void ShowFinalMessage(string message)
    {
        finalTextMessage.SetText(message);
        finalTextMessage.gameObject.SetActive(true);
    }
}
