using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Walking,
        PlayerAggro,
        Attacking
    }

    public EnemyState currentState = EnemyState.Idle;

    public GameObject attackHitboxPrefab;
    public float attackOffset = 1.5f;
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float idleTime = 3f;
    public float walkTime = 5f;
    public float moveSpeed = 3f;

    private NavMeshAgent agent;
    private Animator WolfAnimator;
    private float timer;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        WolfAnimator = GetComponent<Animator>();
        timer = idleTime;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        timer = idleTime;
    }

    void Update()
    {
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
        UpdateAnimation();
    }

    private void HandleIdleState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            currentState = EnemyState.Walking;
            timer = walkTime;
        }
        DetectPlayer();
    }
    
    

    private bool hasDestination = false;

    private void HandleWalkingState()
    {
        if (!hasDestination)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 5f + transform.position;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas);
            agent.SetDestination(hit.position);

            hasDestination = true;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            currentState = EnemyState.Idle;
            timer = idleTime;
            hasDestination = false;
        }

        DetectPlayer();
    }

    private void HandlePlayerAggroState()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = EnemyState.Attacking;
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    private void HandleAttackingState()
    {
       
        Debug.Log("attacking player");
        
        if (Random.value < 0.5f)
        {
            AttackType1();
        }
        else
        {
            AttackType2();
        }
        currentState = EnemyState.PlayerAggro;
    }

    private void DetectPlayer()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            currentState = EnemyState.PlayerAggro;
        }
    }

    private void UpdateAnimation()
    {
        WolfAnimator.SetBool("IsIdle", currentState == EnemyState.Idle);
        WolfAnimator.SetBool("IsWalking", currentState == EnemyState.Walking);
        WolfAnimator.SetBool("IsPlayerAggro", currentState == EnemyState.PlayerAggro);
        WolfAnimator.SetBool("IsAttacking", currentState == EnemyState.Attacking);
    }

    void SpawnHitbox()
    {
        Vector3 spawnPos = transform.position + transform.forward * attackOffset;

        GameObject hitbox = Instantiate(attackHitboxPrefab, spawnPos, transform.rotation);

        hitbox.tag = "EnemyAttack";
    }
    private void AttackType1()
    {
        Debug.Log("Bite attack");
        SpawnHitbox();
    }

    private void AttackType2()
    {
        Debug.Log("Scratch attack");
        SpawnHitbox();
        audioSource.Play();
    }
    
}