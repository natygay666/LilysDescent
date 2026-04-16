using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovementF : MonoBehaviour
{
  
[SerializeField]
    private float speed = 5f;

[SerializeField]
private float mouseSensitivity = 5f;

[SerializeField] private InputAction jump;

[SerializeField]
float jumpForce = 10f;

private Rigidbody rb;

private Vector3 moveDirection;

private float rotationY;

private void Start()
{
    rb = GetComponent<Rigidbody>();
}

private void OnEnable()
{
    jump.Enable();
    
}

private void OnFixedUpdate()
{
    if (jump.IsPressed())
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
}
    void Update()
    {
        void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

        void HandleRotation()
    {
        float moauseX = Input.GetAxis("Mouse X");
        rotationY += moauseX * mouseSensitivity;
        
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
     }
    }
}
