using System.Collections;
using System.Collections.Generic;
using OneP.Samples;
using UnityEngine;
using UnityEngine.UI;

public class UIResult : SingletonMono<UIResult>
{
    public GameObject root;

    public Text textScore;

    public Text textMaxCombo;

    public Text textCollect;

    public Text info;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMenu()
    {
        UIMainMenu.Instance.Show(true);
        Show(false);
    }

    public void Show(bool isOpen)
    {
        root.SetActive(isOpen);
        UIInGame.instance.Show(false);
    }

    public void UpdateUI()
    {
        textScore.text = GameplayData.Instance.score.ToString();
        textMaxCombo.text = GameplayData.Instance.countMaxCombo.ToString();
        textCollect.text = GameplayData.Instance.countItemCollect.ToString()+", Reached "+ string.Format("{0:0.##}", GameplayData.Instance.Percent()) +" Road"; 
        int maxScore = PlayerPrefs.GetInt("MAX_SCORE_"+GameplayController.Instance.GetCurrentDesignLevel() , 0);
        if (maxScore < GameplayData.Instance.score)
        {
            info.text = "THIS IS THE HIGHEST SCORE OF THIS LEVEL !!!";
            PlayerPrefs.SetInt("MAX_SCORE_"+GameplayController.Instance.GetCurrentDesignLevel(), GameplayData.Instance.score);
        }
        else
        {
            info.text = "THE HIGHEST SCORE OF THIS LEVEL IS: "+maxScore;
        }
        
    }
}
