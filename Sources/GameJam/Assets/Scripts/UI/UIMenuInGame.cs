using System;
using System.Collections;
using System.Collections.Generic;
using OneP.Samples;
using UnityEngine;

public class UIMenuInGame : SingletonMono<UIMenuInGame>
{
    public GameObject root;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnResume()
    {
        Show(false);
        GameplayController.Instance.OnPauseGame(false);
    }

    public void OnRestart()
    {
        GameplayController.Instance.RestartGame();
        Show(false);

    }

    public void OnComeBackMenu()
    {
        Show(false);
        UIMainMenu.Instance.Show(true);
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayBackgroundMusic();
        
    }

    public void Show(bool isShow)
    {
        root.SetActive(isShow);
    }

}
