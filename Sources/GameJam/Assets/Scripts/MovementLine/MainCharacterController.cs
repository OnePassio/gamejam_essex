using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterMovementStatus{
    LEFT,
    RIGHT
}

public class MainCharacterController : MonoBehaviour
{
    private CharacterMovementStatus status;
    [SerializeField]
    private Transform movementTrans;

    [SerializeField] 
    private Animator jumpAnim;

    private GameObject lastCollisionObject;
    
    [SerializeField]
    private GameObject prefabRay;
    
    
    
    [SerializeField]
    private GameObject rayStart;
    [SerializeField]
    private GameObject rayEnd;

    [SerializeField]
    private Transform mainLaser;
    
    [SerializeField]
    private List<GameObject> laserPrefabs;
   
    private EGA_Laser laserEffect;

    private float timeEffectLaser = 0;

    [SerializeField]
    private Transform endLand;
    
    public float MIN_DISTANCE = 0.1f;

    public GameObject prefebEffectExplo;
    public void SetStatus(CharacterMovementStatus status)
    {
        this.status = status;
    }

    public void UpdateLocation(MovementCenterTrack center)
    {
        Transform target = center.GetLeft();
        if (status == CharacterMovementStatus.RIGHT)
        {
            target=center.GetRight();
        }
        float distance= Vector3.Distance(target.position,movementTrans.position);
        if (distance < MIN_DISTANCE)
        {
            movementTrans.position = target.position;
            movementTrans.LookAt(target);
        }
        else
        {
            float move = 1.0f;//distance / 10;
            if (move < MIN_DISTANCE/3)
            {
                move = distance;
            }

            float percent = move / distance;
            Vector3 pos = Vector3.Lerp(movementTrans.position, target.position,percent);
            
            movementTrans.position = pos;
            //movementTrans.position = target.position;
            movementTrans.localEulerAngles = target.localEulerAngles;
        }

        if (timeEffectLaser > 0)
        {
            timeEffectLaser -= Time.smoothDeltaTime;
            if (timeEffectLaser <= 0)
            {
                timeEffectLaser = 0;
                laserEffect.DisablePrepare();
            }
        }
    }

    public void TurnOnEffectLaser()
    {
        if (laserEffect != null)
        {
            GameObject.Destroy(laserEffect.gameObject);
        }

        GameObject obj = GameObject.Instantiate(laserPrefabs[0],mainLaser.transform.position,mainLaser.transform.rotation);
        obj.transform.SetParent(mainLaser.transform.parent);
        laserEffect = obj.GetComponent<EGA_Laser>();
        laserEffect.lookatObj = endLand;
        //laserEffect.gameObject.SetActiveRecursively(true);
        timeEffectLaser = 0.3f;
    }

    public void Jump()
    {
        jumpAnim.SetTrigger("Jump");
    }

    // Start is called before the first frame update
    void Start()
    {
        //CreateLaser();
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.localPosition=Vector3.zero;
        //this.transform.localEulerAngles=Vector3.zero;
    }

    public void CreateLaser()
    {
        GameObject rayObject = Instantiate(prefabRay, rayEnd.transform.position, rayEnd.transform.rotation);
        rayObject.transform.parent = transform;
        rayObject.transform.localEulerAngles=new Vector3(90,0,0);
        rayObject.transform.localPosition = rayStart.transform.position;
        //LaserScript = Instance.GetComponent<EGA_Laser>()
    }

    public void OnSpaceToCollectItem()
    {
        TurnOnEffectLaser();
        bool isHasItem = false;
        for (int i = 0; i < GameplayController.Instance.GetListItemActive().Count; i++)
        {
            ItemInGame item = GameplayController.Instance.GetListItemActive()[i];
            if (item.GetStatus() == status)// same direction
            {
                if (Mathf.Abs(GameplayController.Instance.GetTimeInGame() - item.GetTimeAppear()) < 0.12f)
                {
                    GameObject objEffect = GameObject.Instantiate(prefebEffectExplo);
                    objEffect.transform.position = item.transform.position;
                    GameObject.Destroy(objEffect,3f);
                    item.CallSafeDestroy();
                    isHasItem = true;
                    //only apply for 1 item in each space
                    break;
                }
            }
        }

        if (isHasItem)
        {
            AudioManager.Instance.PlaySoundEffect(0);
            GameplayData.Instance.OnPressSuccess();
            UIInGame.Instance.UpdateUI();
        }
        else
        {
            AudioManager.Instance.PlaySoundEffect(2);
            GameplayData.Instance.OnMiss();
            UIInGame.Instance.UpdateUI();
        }


        /*
        if (lastCollisionObject != null)
        {
            float dist = Vector3.Distance(this.transform.position, lastCollisionObject.transform.position);
            if(dist<1.0f)
            {
                GameObject objEffect = GameObject.Instantiate(prefebEffectExplo);
                objEffect.transform.position = lastCollisionObject.transform.position;
                GameObject.Destroy(objEffect,3f);
                GameObject.Destroy(lastCollisionObject);
            }
        }*/
    }

    public void OnCollisionEnter(Collision other)
    {
        lastCollisionObject = other.gameObject;

        ItemInGame item = lastCollisionObject.GetComponent<ItemInGame>();
        if (item != null)
        {
            //item.ChangeColorWhenHaveCollision();
        }
        //GameObject.Destroy(other.gameObject);

    }
}
