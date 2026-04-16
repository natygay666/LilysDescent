using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerStateMachine : MonoBehaviour
{
    private enum PlayerState
    {
        Idle,
        Walking,
        Jumping,
        Attacking,
        Evading,
        Blocking,
        Shooting,
        SpecialAttack
    }

    [Header("References")]
    public Animator animator; // Slot para el Animator
    public NavMeshAgent agent; // Referencia al NavMeshAgent
    public GameObject projectilePrefab; // Prefab del proyectil
    public Transform shootingPoint; // Punto de disparo
    public GameUIController uiController; // Referencia al GameUIController

    private PlayerState currentState = PlayerState.Idle;
    private bool isBlocking = false;

    // Variables de salud y ulti
    public int playerMaxHp = 100;
    public int playerHp;
    public int ultiValue;

    // Inventario
    private List<string> inventory = new List<string>(); // Lista de objetos clave
    private string goldenCarrot = "GoldenCarrot"; // Nombre del objeto
    private string specialItem = "SpecialItem"; // Otro objeto especial

    private void Start()
    {
        playerHp = playerMaxHp;
        ultiValue = 0;
        UpdateUI();
    }

    private void Update()
    {
        HandleInput();
        UpdateState();
    }

    private void HandleInput()
    {
        // Movimiento
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            Move();
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Evasión
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Evade();
        }

        // Ataques
        if (Input.GetMouseButtonDown(0)) // Light Attack
        {
            LightAttack();
        }
        if (Input.GetMouseButtonDown(1)) // Heavy Attack
        {
            HeavyAttack();
        }

        // Bloqueo
        if (Input.GetMouseButton(2)) // Block
        {
            Block();
        }

        // Disparo
        if (Input.GetKeyDown(KeyCode.F)) // Shooting
        {
            Shoot();
        }

        // Recoger objeto
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Lógica para recoger objetos, sustituye "ItemName" con el nombre del objeto
            PickUp("ItemName");
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                animator.SetTrigger("Idle");
                break;
            case PlayerState.Walking:
                animator.SetTrigger("Walk");
                break;
            case PlayerState.Jumping:
                animator.SetTrigger("Jump");
                break;
            case PlayerState.Attacking:
                animator.SetTrigger("Attack");
                break;
            case PlayerState.Evading:
                animator.SetTrigger("Evade");
                break;
            case PlayerState.Blocking:
                animator.SetTrigger("Block");
                break;
            case PlayerState.Shooting:
                animator.SetTrigger("Shoot");
                break;
            case PlayerState.SpecialAttack:
                animator.SetTrigger("SpecialAttack");
                break;
        }
    }

    private void Move()
    {
        currentState = PlayerState.Walking;
        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        agent.Move(moveDirection * Time.deltaTime);
        agent.transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    private void Jump()
    {
        currentState = PlayerState.Jumping;
        // Lógica de salto aquí
    }

    private void Evade()
    {
        currentState = PlayerState.Evading;
        // Lógica de evasión aquí
    }

    private void Block()
    {
        isBlocking = true;
        currentState = PlayerState.Blocking;
        // Lógica para reducir daño
    }

    private void Shoot()
    {
        currentState = PlayerState.Shooting;
        // Lógica de detección de enemigos y disparo
        RaycastHit hit;
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hit))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
                if (inventory.Contains(specialItem))
                {
                    Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity); // Segundo proyectil
                }
            }
        }
    }

    private void LightAttack()
    {
        currentState = PlayerState.Attacking;
        SpawnAttackCollider();
        // Lógica de ataque ligero aquí
    }

    private void HeavyAttack()
    {
        currentState = PlayerState.Attacking;
        SpawnAttackCollider();
        // Lógica de ataque pesado aquí
    }

    private void SpawnAttackCollider()
    {
        // Lógica para spawn de collider en zona de ataque
    }

    private void PickUp(string itemName)
    {
        // Lógica para recoger objetos
        if (itemName == "Item25")
        {
            ModifyPlayerHp(25);
        }
        else if (itemName == "Item50")
        {
            ModifyPlayerHp(50);
        }
        else if (itemName == goldenCarrot)
        {
            playerMaxHp = 150; // Aumenta el máximo de vida
            ModifyPlayerHp(playerMaxHp); // Restaura vida
        }
        else if (itemName == specialItem)
        {
            ultiValue = 200; // Aumenta UltiValue
        }

        // Añadir al inventario si es un objeto clave
        if (itemName == goldenCarrot || itemName == specialItem)
        {
            inventory.Add(itemName);
            UpdateUI(); // Actualiza la UI del inventario
        }
    }

    private void ModifyPlayerHp(int amount)
    {
        playerHp = Mathf.Clamp(playerHp + amount, 0, playerMaxHp);
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Actualiza los sliders en el GameUIController
        if (uiController != null)
        {
            uiController.UpdateHealthSlider(playerHp, playerMaxHp);
            uiController.UpdateUltiSlider(ultiValue);
            uiController.UpdateInventoryUI(inventory); // Asumiendo que tienes un método para actualizar el inventario
        }
    }
}