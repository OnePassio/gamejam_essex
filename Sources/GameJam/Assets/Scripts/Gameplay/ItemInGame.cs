using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public  enum GameItemType{
    None,
    Normal,
}

public enum GameItemColor{
    Red,
    Blue
}

public class ItemInGame : MonoBehaviour
{
    private ConfigLevelDesignItem data;
    private  static  System.Random random=new System.Random();
    [SerializeField] private float fixHeight;
    private GameItemType type = GameItemType.Normal;
    [SerializeField]
    private Material redMat;
    [SerializeField]
    private Material blueMat;

    [SerializeField] private MeshRenderer mesh;
    
    private float time = 0;

    [SerializeField] private Transform rootStone;
    [SerializeField] private List<GameObject> stones;

    public GameItemType GetGameType()
    {
        return type;
    }
    private CharacterMovementStatus status=CharacterMovementStatus.LEFT;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float GetTimeAppear()
    {
        return data.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 0)
        {
            time -= Time.smoothDeltaTime;
            if (time < 0)
            {
                time = 0;
                SetupColor(GameItemColor.Red);
            }
        }

        if (data != null)
        {
            if (GameplayController.Instance.GetTimeInGame() - data.time>2.0f)// remove old Item
            {
                CallSafeDestroy();
            }

        }
    }

    public void CallSafeDestroy()
    {
        GameplayController.Instance.RemoveItemInGame(this);
    }

    public void ChangeColorWhenHaveCollision()
    {
        SetupColor(GameItemColor.Blue);
        time = 0.2f;
    }

    private void SetupColor(GameItemColor itemColor)
    {
        if (mesh != null)
        {
            if (itemColor == GameItemColor.Blue)
                mesh.material = blueMat;
            else
            {
                mesh.material = redMat;
            }
        }
    }

    public void Setup(ConfigLevelDesignItem data)
    {
        this.data = data;
        MovementCenterTrack item = GameplayController.Instance.GetLocationMovement();
        item.SetMovement(GameplayController.Instance.mainTrack,data.time);

        int rand = random.Next(2);
        if (rand == 1)
        {
            Vector3 pos= item.GetLeft().position;
            pos.y += fixHeight;
            transform.position = pos;
            status=CharacterMovementStatus.LEFT;
        }
        else
        {
            Vector3 pos = item.GetRight().position;
            pos.y += fixHeight;
            transform.position = pos;
            status=CharacterMovementStatus.RIGHT;
        }

        rand = random.Next(stones.Count);
        GameObject prefab = stones[rand];
        if (prefab != null)
        {
            GameObject obj = GameObject.Instantiate(prefab, rootStone);
            obj.transform.localPosition = Vector3.zero;
            rand = random.Next(360);
            obj.transform.localScale=new Vector3(0.6f,0.6f,0.6f);
            obj.transform.localEulerAngles=new Vector3(0,rand,0);
            
        }
    }

    public CharacterMovementStatus GetStatus()
    {
        return status;
    }
}
