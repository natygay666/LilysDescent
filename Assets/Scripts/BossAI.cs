using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BossAI : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        PlayerAggro,
        Attacking
    }

    public BossState currentState = BossState.Idle;

    [Header("References")]
    public Transform player;
    public GameObject attackHitboxPrefab;

    private NavMeshAgent agent;
    private Animator BossAnimator;

    [Header("Stats")]
    public float detectionRange = 15f;
    public float attackRange = 3f;
    public float moveSpeed = 4f;
    public float attackCooldown = 2f;

    private bool canAttack = true;

    [Header("Meteor Attack")]
    public int meteorCount = 10;
    public float meteorRadius = 5f;
    public float meteorHeight = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        BossAnimator = GetComponent<Animator>();
        agent.speed = moveSpeed;
    }

    void Update()
    {
        switch (currentState)
        {
            case BossState.Idle:
                HandleIdleState();
                break;

            case BossState.PlayerAggro:
                HandlePlayerAggroState();
                break;

            case BossState.Attacking:
                
                break;
        }

        UpdateAnimation();
    }

    private void HandleIdleState()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            currentState = BossState.PlayerAggro;
        }
    }

    private void HandlePlayerAggroState()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && canAttack)
        {
            StartCoroutine(AttackRoutine());
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    private IEnumerator AttackRoutine()
    {
        currentState = BossState.Attacking;
        canAttack = false;

        agent.ResetPath();
        
        if (Random.value < 0.5f)
        {
            BossAnimator.SetTrigger("Attack1");
            AttackType1();
        }
        else
        {
            BossAnimator.SetTrigger("Attack2");
            yield return StartCoroutine(AttackType2_MeteorRain());
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        currentState = BossState.PlayerAggro;
    }

    private void UpdateAnimation()
    {
        BossAnimator.SetBool("IsIdle", currentState == BossState.Idle);
        BossAnimator.SetBool("IsChasing", currentState == BossState.PlayerAggro);
        BossAnimator.SetBool("IsAttacking", currentState == BossState.Attacking);
    }
    
    private void AttackType1()
    {
        Vector3 spawnPos = transform.position + transform.forward * 2f;

        GameObject hitbox = Instantiate(attackHitboxPrefab, spawnPos, transform.rotation);
        hitbox.tag = "EnemyAttack";
    }
    
    private IEnumerator AttackType2_MeteorRain()
    {
        Debug.Log("Meteor Rain Attack");

        for (int i = 0; i < meteorCount; i++)
        {
            Vector3 randomPos = player.position + new Vector3(
                Random.Range(-meteorRadius, meteorRadius),
                0,
                Random.Range(-meteorRadius, meteorRadius)
            );

            Vector3 spawnPos = randomPos + Vector3.up * meteorHeight;

            GameObject meteor = Instantiate(attackHitboxPrefab, spawnPos, Quaternion.identity);
            meteor.tag = "EnemyAttack";
            
            Rigidbody rb = meteor.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.down * 15f;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    void OnDestroy()
    {
        SceneManager.LoadScene(6);
    }
}