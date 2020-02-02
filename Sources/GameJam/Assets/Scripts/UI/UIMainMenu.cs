using System.Collections;
using System.Collections.Generic;
using OneP.Samples;
using UnityEngine;
using UnityEngine.UI;
public class UIMainMenu : SingletonMono<UIMainMenu>
{
    // Start is called before the first frame update
    public GameObject[] listGroupDialog;
    public Slider volumeSlider;
    public GameObject rootMainMenu;
    public Slider soundEffectSlider;

    public Toggle toggeGraphicMode;
    void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDialogOpenMainMenu()
    {
        OpenDialog(0);
    }

    public void OnDialogOpenSelectLevel()
    {
        OpenDialog(1);
    }
    
    
    public void OnDialogOpenGuideLine()
    {
        OpenDialog(2);
    }
    
    public void OnDialogSetting()
    {
        OpenDialog(3);
        if (toggeGraphicMode != null)
        {
            bool isActive = true;
            int mode = PlayerPrefs.GetInt("GRAPHICS_MODE", 1);
            if (mode == 0)
            {
                isActive = false;
            }
            toggeGraphicMode.isOn = isActive;
            
        }
    }
    
    public void OnDialogAbout()
    {
        OpenDialog(4);
    }

    public void OnBack()
    {
        OnDialogOpenMainMenu();
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void SelectLevel1()
    {
        GameplayController.Instance.LoadGameLevel(100);
        Show(false);
    }
    
    public void SelectLevel2()
    {
        
        GameplayController.Instance.LoadGameLevel(101);
        Show(false);
    }
    
    public void SelectLevel3()
    {
        
        GameplayController.Instance.LoadGameLevel(102);
        Show(false);
    }

    public void Show(bool isOpen)
    {
        rootMainMenu.SetActive(isOpen);
        if (isOpen)
        {
            UIInGame.Instance.Show(false);
            OpenDialog(0);
        }
    }

    public void OnVolumeChange()
    {
        AudioManager.Instance.SetVolume(soundEffectSlider.value);
    }

    public void OnSoundEffectChange()
    {
        AudioManager.Instance.SetSoundEffectVolume(volumeSlider.value);
    }

    public void OpenDialog(int index)
    {
        for (int i = 0; i < listGroupDialog.Length; i++)
        {
            if (i != index)
            {
                listGroupDialog[i].SetActive(false);
            }
            else
            {
                listGroupDialog[i].SetActive(true);
            }
        }
    }
    
    public void OnGraphicMode(bool value)
    {
       // Debug.LogError("AAAAA:"+toggeGraphicMode.isOn);
        int mode = 1;
        if(!toggeGraphicMode.isOn)
        {
            mode = 0;
        }
        PlayerPrefs.SetInt("GRAPHICS_MODE", mode);
        
    }
}
