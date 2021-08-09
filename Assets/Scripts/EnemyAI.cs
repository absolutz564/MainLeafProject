using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    public bool IsStaticEnemy;
    
    private void Awake()
    {
        EnemyAnim = GetComponent<Animator>();
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet && !IsStaticEnemy)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        EnemyAnim.SetBool("attacking", false);
        EnemyAnim.SetBool("following", false);

        if (!IsStaticEnemy)
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
        Rigidbody rb = Instantiate(projectile, BaseArrow.position, BaseArrow.rotation).GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * 15f, ForceMode.Impulse);
        rb.AddForce(transform.up * 4f, ForceMode.Impulse);
        Destroy(rb.gameObject, 4f);
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
