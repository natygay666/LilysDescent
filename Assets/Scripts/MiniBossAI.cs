using UnityEngine;
using UnityEngine.AI;

public class MiniBossAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Walking,
        PlayerAggro,
        Attacking
    }

    public EnemyState currentState = EnemyState.Idle;

    [Header("References")]
    public Transform player;
    public GameObject attackHitboxPrefab;
    public GameObject projectilePrefab;

    [Header("Stats")]
    public float detectionRange = 12f;
    public float attackRange = 3f;
    public float moveSpeed = 3.5f;

    [Header("Timers")]
    public float idleTime = 2f;
    public float walkTime = 4f;
    public float attackCooldown = 2f;

    [Header("Attack Settings")]
    public float attackOffset = 1.5f;
    public float projectileForce = 10f;

    private NavMeshAgent agent;
    private Animator HunterAnimator;

    private float stateTimer;
    private float attackTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        HunterAnimator = GetComponent<Animator>();

        stateTimer = idleTime;
        attackTimer = 0f;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;

            case EnemyState.Walking:
                HandleWalkingState();
                break;

            case EnemyState.PlayerAggro:
                HandlePlayerAggroState();
                break;

            case EnemyState.Attacking:
                HandleAttackingState();
                break;
        }
    }

    // ================= STATES =================

    void HandleIdleState()
    {
        SetAnimation("Idle");

        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            currentState = EnemyState.Walking;
            stateTimer = walkTime;
        }

        DetectPlayer();
    }

    void HandleWalkingState()
    {
        SetAnimation("Walk");

        if (!agent.hasPath)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 5f + transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                agent.speed = moveSpeed;
            }
        }

        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            currentState = EnemyState.Idle;
            stateTimer = idleTime;
            agent.ResetPath();
        }

        DetectPlayer();
    }

    void HandlePlayerAggroState()
    {
        
        SetAnimation("Walk");
        HunterAnimator.SetBool("Walking", true);
        HunterAnimator.SetBool("Attacking", false);
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && attackTimer <= 0f)
        {
            currentState = EnemyState.Attacking;
            agent.ResetPath();
            return;
        }

        agent.SetDestination(player.position);
    }

    void HandleAttackingState()
    {
        transform.LookAt(player);
        HunterAnimator.SetBool("Attacking", true);
        HunterAnimator.SetBool("Walking", false);
        if (Random.value < 0.5f)
        {
            AttackType1();
        }
        else
        {
            AttackType2();
        }

        attackTimer = attackCooldown;
        currentState = EnemyState.PlayerAggro;
    }

    

    void DetectPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            currentState = EnemyState.PlayerAggro;
        }
    }

    

    void AttackType1()
    {
        Debug.Log("MiniBoss Melee Attack");
        SetAnimation("AxeAttack");

        Vector3 spawnPos = transform.position + transform.forward * attackOffset;
        GameObject hitbox = Instantiate(attackHitboxPrefab, spawnPos, transform.rotation);

        hitbox.tag = "EnemyAttack";
    }

    void AttackType2()
    {
        Debug.Log("MiniBoss Projectile Attack");
        SetAnimation("GunAttack");

        GameObject projectile = Instantiate(projectilePrefab, transform.position + Vector3.up, Quaternion.identity);

        projectile.tag = "EnemyAttack";

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * projectileForce;
        }
    }
    

    void SetAnimation(string triggerName)
    {
        HunterAnimator.ResetTrigger("Idle");
        HunterAnimator.ResetTrigger("Walk");
        HunterAnimator.ResetTrigger("AxeAttack");
        HunterAnimator.ResetTrigger("GunAttack");

        HunterAnimator.SetTrigger(triggerName);
    }
}