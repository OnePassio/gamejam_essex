using System.Collections;
using System.Collections.Generic;
using OneP.Samples;
using UnityEngine;
using UnityEngine.UI;
public class UIInGame : SingletonMono<UIInGame>
{
    public GameObject root;
    public Text txtScore;

    public Text textCombo;

    public Text textCollect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPause()
    {
        GameplayController.Instance.OnPauseGame(true);
    }

    public void UpdateUI()
    {
        txtScore.text = GameplayData.Instance.score.ToString();
        textCombo.text = GameplayData.Instance.countCombo.ToString();
        textCollect.text = GameplayData.Instance.countItemCollect.ToString();

        
    }

    public void Show(bool isShow)
    {
        root.SetActive(isShow);
    }
}
