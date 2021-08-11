using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public int Life = 100;
    public Animator DeadCanvasAnim;
    public Animator CameraAnim;
    public MouseControl MControl;
    public PlayerMovement PMovement;
    public CapsuleCollider PlayerCollider;
    public CharacterController Controller;
    public Slider HealthBar;
    public bool IsDead = false;

    public GameObject projectile;
    public Transform BaseArrow;

    public bool CanShot = false;
    public int Arrows;
    public AudioSource ArrowSound;
    public AudioSource ItemCollect;

    public List<GameObject> ArrowsList;
    public void Hit()
    {
        Debug.Log("Atacou!");

        GameController.Instance.UpdatePlayerLife(false, 10);
        if (Life <= 0)
        {
            IsDead = true;
            Dead();
        }
    }

    private void Start()
    {
        ConfigPool();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("EnemyArrow"))
        {
            Debug.Log("Flexa do inimigo acertou player");
            Hit();
            coll.gameObject.SetActive(false);
        }
        if (coll.CompareTag("HeartItem"))
        {
            GameController.Instance.HealthPlayer();
            Destroy(coll.gameObject);
            ItemCollect.Play();
        }
        if (coll.CompareTag("ArrowItem"))
        {
            GameController.Instance.UpdateArrows(true);
            Destroy(coll.gameObject);
            ItemCollect.Play();
        }

    }


    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            InstantiateArrow();
            Debug.Log("Pressed left click.");
        }
    }

    public void InstantiateArrow()
    {
        ///Attack code here
        if (CanShot && Arrows > 0)
        {
            ArrowSound.Play();
            GameController.Instance.UpdateArrows(false);
            CanShot = false;
            PoolArrow();
            Invoke("WaitToCanShot", 0.5f);
        }
    }

    void ConfigPool()
    {
        Debug.Log("Criou Pool de flechas");
        ArrowsList = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject objBullet = (GameObject)Instantiate(projectile);
            objBullet.SetActive(false);
            ArrowsList.Add(objBullet);
        }
    }
    void PoolArrow()
    {
        for (int i = 0; i < ArrowsList.Count; i++)
        {
            if (!ArrowsList[i].activeInHierarchy)
            {
                ArrowsList[i].transform.position = BaseArrow.position;
                ArrowsList[i].transform.rotation = BaseArrow.rotation;
                ArrowsList[i].SetActive(true);

                Rigidbody rb = ArrowsList[i].GetComponent<Rigidbody>();
                rb.transform.tag = "PlayerArrow";
                rb.AddForce(transform.forward * 15f, ForceMode.Impulse);
                rb.AddForce(transform.up * 4f, ForceMode.Impulse);
                break;
            }
        }

    }


    void WaitToCanShot()
    {
        CanShot = true;
    }

    void DisableAllControllers()
    {
        AudioSource PlayerAudio = GetComponent<AudioSource>();
        {
            if (PlayerAudio != null)
            {
                PlayerAudio.Stop();
            }
        }
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        Controller.enabled = false;
        PlayerCollider.enabled = false;

        MControl.enabled = false;
    }

    public void Dead()
    {
        DisableAllControllers();

        //Call Camera Anim Dead;
        CameraAnim.enabled = true;
        CameraAnim.SetTrigger("Dead");
        Debug.Log("Morreu");


        GameController.Instance.SetCursorState(true);
    }

    public void SetCanvasDead()
    {
        DeadCanvasAnim.SetTrigger("Dead");
    }

}
