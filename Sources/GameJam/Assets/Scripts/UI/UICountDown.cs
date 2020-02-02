using System.Collections;
using System.Collections.Generic;
using OneP.Samples;
using UnityEngine;

public class UICountDown : SingletonMono<UICountDown>
{
    public GameObject root;

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCountDown()
    {
        StartCoroutine(ShowRoutineCountDown());
    }

    public IEnumerator ShowRoutineCountDown()
    {
        Show(true);
        animator.SetTrigger("Show");
        yield return  new WaitForSeconds(2);
        Show(false);
        GameplayController.Instance.StartGame();
    }

    public void Show(bool isShow)
    {
        root.SetActive(isShow);
    }
}
