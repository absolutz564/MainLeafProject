using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public Transform BaseArrow;
    public Animator EnemyAnim;

    public Enemies EnemyType;

    public Slider HealthBar;
    bool IsDead = false;

    public List<GameObject> ArrowsList;

    [System.Serializable]
    public enum Enemies
    {
        Archer,
        Warrior
    }

    public void Damage()
    {
        health -= 25;
        HealthBar.value = health;
        if (health<= 0)
        {
            Dead();
        }
    }

    void Dead()
    {
        GameController.Instance.EnemiesDied++;
        IsDead = true;
        EnemyAnim.SetBool("attacking", false);
        EnemyAnim.SetBool("following", false);
        EnemyAnim.SetBool("walking", false);
        EnemyAnim.SetBool("dead", true);
        agent.Stop();
        Destroy(this.gameObject, 4);
        IncreasePlayerPoints();

        RemoveEnemyColisor();

        //Chance to spawn items
        GameController.Instance.SpawnItems(this.transform);
    }

    void RemoveEnemyColisor()
    {
        Collider coll = this.GetComponent<Collider>();
        coll.isTrigger = true;
    }

    void IncreasePlayerPoints()
    {
        GameController.Instance.IncreasePlayerPoints();
    }

    private void Awake()
    {
        ConfigPool();
        EnemyAnim = GetComponent<Animator>();
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PlayerArrow"))
        {
            Debug.Log("Flexa do player acertou inimigo");
            coll.gameObject.SetActive(false);
            Damage();
        }
    }

    private void Update()
    {
        //Check for sight and attack range
        if (IsDead == false)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInSightRange && !playerInAttackRange) Patroling();
            if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
    }

    public void Hit()
    {
        GameController.Instance.HitPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && EnemyType != Enemies.Archer)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        EnemyAnim.SetBool("attacking", false);
        EnemyAnim.SetBool("following", false);

        if (EnemyType != Enemies.Archer)
        {
            //Anim Walking
            EnemyAnim.SetBool("walking", true);
        }

    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        
        agent.SetDestination(player.position);
        
        //Anim Follow
        EnemyAnim.SetBool("attacking", false);
        EnemyAnim.SetBool("following", true);
        EnemyAnim.SetBool("walking", false);
    }

    public void InstantiateArrow()
    {
        ///Attack code here
        PoolArrow();
    }

    void ConfigPool()
    {
        Debug.Log("Criou Pool de flecha inimiga");
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
                rb.transform.tag = "EnemyArrow";
                rb.AddForce(transform.forward * 15f, ForceMode.Impulse);
                rb.AddForce(transform.up * Random.Range(3f, 6f), ForceMode.Impulse);
                break;
            }
        }

    }

    private void AttackPlayer()
    {

            //Make sure enemy doesn't move
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            if (!alreadyAttacked)
            {
                Quaternion EnemyRot = this.GetComponent<Transform>().rotation;

                //Anim Attack
                EnemyAnim.SetBool("attacking", true);
                EnemyAnim.SetBool("following", false);
                EnemyAnim.SetBool("walking", false);


                ///End of attack code

                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
