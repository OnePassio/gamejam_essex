using System.Collections;
using System.Collections.Generic;
using OneP.Samples;
using UnityEngine;

public class GameplayData : Singleton<GameplayData>
{
    
    public List<ConfigLevelDesignItem> noteDatas;

    public int LowestNoteIndex = 0;
    public float totalTime=5;
    private float LIMIT_TIME_PLAY = 180;

    public int score;
    public int countCombo;
    public int countMaxCombo;
    public int countItemCollect;
    public static int SCORE_ADD = 10;
    public static int SCORE_COMBOX = 3;

    public void OnMiss()
    {
        score -= SCORE_ADD;
        if (score < 0)
        {
            score = 0;
        }
        countCombo = 0;
    }
    public float Percent(){
        float percent=0;
        if(noteDatas!=null&&noteDatas.Count>0){
            percent=countItemCollect/noteDatas.Count;
        }
        return percent;
    }

    public void OnPressSuccess()
    {
        countItemCollect++;
        countCombo++;
        if (countMaxCombo < countCombo)
        {
            countMaxCombo = countCombo;
        }

        int coff = countCombo / 5;
        if (coff > 10)
        {
            coff = 10;
        }
        score += SCORE_ADD + coff * SCORE_COMBOX;
    }

    public bool IsMoveFinish()
    {
        if (GameplayController.Instance.GetTimeInGame() > totalTime)
        {
            return true;
        }

        return false;
    }

    public void Setup(int levelIndex)
    {
        score=0;
        countCombo=0;
        countItemCollect=0;
        if (levelIndex < 0)
            levelIndex = 0;
        string datafile = levelIndex.ToString();
        string musicFileName = levelIndex.ToString()+"_music";
        
        AudioManager.Instance.LoadLocalMusic(musicFileName);
        TextAsset textAsset = (TextAsset) Resources.Load(datafile);
        if (textAsset != null)
        {
            ConfigLevelDesign design = ConfigService.LoadLevelDesign(textAsset.text);
            noteDatas = design.records;
            for (int i = 0; i < noteDatas.Count-1; i++)
            {
                ConfigLevelDesignItem item1=noteDatas[i];
                
                ConfigLevelDesignItem item2=noteDatas[i+1];
                bool isRemove = false;
                if (Mathf.Abs(item1.time - item2.time) < 0.1f)//remove 2 item in same time
                {
                    Debug.Log("Remove Error Design level at:"+item1.time);
                    noteDatas.RemoveAt(i);
                    i = i - 1;
                    isRemove = true;
                }
                if (!isRemove)
                {
                    float timeMove = LIMIT_TIME_PLAY;//GameplayController.Instance.mainTrack.GetTimeMove();
                    if (item2.time > timeMove-2) // remove note all of Track
                    {
                        noteDatas.RemoveAt(i);
                        i = i - 1;
                    }
                }

            }
        }

        totalTime = noteDatas[noteDatas.Count - 2].time +1;
        if (totalTime > LIMIT_TIME_PLAY)
        {
            totalTime = LIMIT_TIME_PLAY;
        }

        LowestNoteIndex = 0;
    }

    public void Reset(int index)
    {
        Setup(index);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
