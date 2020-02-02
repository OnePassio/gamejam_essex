using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    public GameObject[] parts;

    //public GameFinished gameFinished;

    public int percentage;

    public int numberOfHittedObjects = 0;

    void Start()
    {
        //gameFinished = GetComponent<GameFinished>();
    }

    public void BuildPiece(int numberOfPartObject)
    {
        Debug.Log("Part Number: " + numberOfPartObject);

        GameObject part = parts[numberOfPartObject - 1].gameObject;

        Debug.Log("Part Name: " + part.gameObject.name);


        part.gameObject.SetActive(true);

        CompletedPercentage();

        Debug.Log("Completed: " + percentage + "%");

       // GetComponent<UpdateScore>().UpdateScoreInScreen(percentage);

        CheckGameStatus();
    }

    public void ObstacleHitted()
    {
        numberOfHittedObjects++;
        CheckGameStatus();
    }

    public void CheckGameStatus()
    {
        if (percentage == 100)
        {
            //gameFinished.GameWin();
        }
        else if (numberOfHittedObjects >= 3)
        {
            //gameFinished.GameOver();
        }
    }


    public int CompletedPercentage()
    {
        int counter = GetComponent<Collect>().GetCount();

        percentage = counter * 100 / parts.Length;

        return percentage;
    }
}