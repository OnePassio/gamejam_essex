using System.Collections;
using System.Collections.Generic;
using OneP.Samples;
using UnityEngine;

public enum GameStage{
    None,
    Initial,
    InGameplay,
    Pause,
    GameOver

}
public class GameplayController : SingletonMono<GameplayController>
{
    [SerializeField] private bool isFreezeTime=false;

    private GameStage gameStage= GameStage.None;
    public List<CacheInitialPosition> listCache;
    #region initial for pool
    
    //All alive Note in Scene
    List<ItemInGame> listNoteActive = new List<ItemInGame>();

    //All note need Clear
    List<ItemInGame> listNoteNeedClear = new List<ItemInGame>();
    #endregion

    public List<ItemInGame> GetListItemActive()
    {
        return listNoteActive;
    }

    // Item Prefab
    [SerializeField]
    private List<GameObject> listPrefabItems;

    private float timeInGame=0;

    //public Transform testFollowObject;
    
    public MovementCenterTrack movementCenter;
    public TrackMovement mainTrack;
    public MainCharacterController mainCharacter;

    public Camera mainCamera;
    [SerializeField]
    private MovementCenterTrack movementLocationForItem;

    private int curentDesignlevel=101;

    public int GetCurrentDesignLevel()
    {
        return curentDesignlevel;
    }

    public MovementCenterTrack GetLocationMovement()
    {
        return movementLocationForItem;
    }

    public float GetTimeInGame()
    {
        return timeInGame;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void LoadGameLevel(int index)
    {
        curentDesignlevel = index;
        
        GameplayData.Instance.Setup(index);
        OnResetGame();
        UICountDown.Instance.ShowCountDown();
        //StartGame();
    }

    public void StartGame()
    {
        gameStage = GameStage.InGameplay;
        AudioManager.Instance.PlayMusic();
        UIInGame.Instance.Show(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(gameStage!=GameStage.InGameplay)
            return;
        OnKeyProcess();
        UpdateNotes();
        if (!isFreezeTime)
        {
            timeInGame += Time.smoothDeltaTime;
        }

        float timePos = timeInGame;
        while (timePos > mainTrack.GetTimeMove())
        {
            timePos -= mainTrack.GetTimeMove();
        }

        if (GameplayData.Instance.IsMoveFinish())
        {
            gameStage = GameStage.GameOver;
            AudioManager.Instance.StopMusic();
            UIResult.Instance.Show(true);
            UIResult.Instance.UpdateUI();
        }

        movementCenter.SetMovement(mainTrack,timePos);
        mainCharacter.UpdateLocation(movementCenter);
        
    }

    public void OnKeyProcess()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            mainCharacter.SetStatus(CharacterMovementStatus.LEFT);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            mainCharacter.SetStatus(CharacterMovementStatus.RIGHT);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            mainCharacter.Jump();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameStage == GameStage.Pause)
            {
                OnPauseGame(false);
            }
            else if (gameStage == GameStage.InGameplay)
            {
                OnPauseGame(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mainCharacter.OnSpaceToCollectItem();
        }
    }

    public void RemoveItemInGame(ItemInGame item)
    {
        listNoteNeedClear.Add(item);
    }

    // Control for generate Object In Screen
    private void UpdateNotes() {
        // clear when begin the function
        for (int i = 0; i < listNoteNeedClear.Count; i++) {
            int index = listNoteActive.IndexOf (listNoteNeedClear[i]);
            if (index > -1) {
                listNoteActive.RemoveAt (index);
            }

            if (listNoteNeedClear[i].gameObject != null)
            {
                GameObject.Destroy(listNoteNeedClear[i].gameObject);
                
            }
        }
        listNoteNeedClear.Clear ();

        float TIMESHOWAHEAD = 3;
        float visibleLimit = timeInGame + TIMESHOWAHEAD;
      
        int countLimit = 0;
      
        for (int i =  0; i < GameplayData.Instance.noteDatas.Count; i++) {
            ConfigLevelDesignItem note = GameplayData.Instance.noteDatas [i];
            if (note.time > visibleLimit) { 
                countLimit++;
                if (countLimit > 9) {
                    break;
                };
            } 
            else { 
                countLimit = 0;
                // sinh ra note moi o day
                GameplayData.Instance.LowestNoteIndex++;
                GameObject newObject=GameObject.Instantiate(listPrefabItems[0]) as GameObject;
                ItemInGame noteSimple = newObject.GetComponent<ItemInGame>();
                noteSimple.Setup (note);
                listNoteActive.Add (noteSimple);
                GameplayData.Instance.noteDatas.RemoveAt (i);
                i--;
            }
        }
    }

    public void OnPauseGame(bool isPause)
    {
        if(isPause)
        {
            if (gameStage == GameStage.InGameplay)
            {
                gameStage = GameStage.Pause;
                AudioManager.Instance.PauseMusic(true);
                UIMenuInGame.Instance.Show(true);
                UIInGame.instance.Show(false);
            }
        }
        else
        {
            
            if (gameStage == GameStage.Pause)
            {
                gameStage = GameStage.InGameplay;
                AudioManager.Instance.PauseMusic(false);
                UIMenuInGame.Instance.Show(false);
                UIInGame.instance.Show(true);
            }
        }
    }

    public void RestartGame()
    {
        
        LoadGameLevel(curentDesignlevel);
    }

    public void OnResetGame()
    {
        GameGraphicsManager.Instance.ShowGraphics();
        for (int i = 0; i < listNoteActive.Count; i++)
        {
            if (listNoteActive[i] != null)
            {
                GameObject.Destroy(listNoteActive[i]);
            }
        }
        listNoteActive.Clear();
        listNoteNeedClear.Clear();
        gameStage=GameStage.None;
        timeInGame = 0;
        for (int i = 0; i < listCache.Count; i++)
        {
            listCache[i].ResetBeginning();
        }
        UIInGame.Instance.UpdateUI();
    }
}
