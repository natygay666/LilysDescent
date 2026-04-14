using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;
public class MoveAndJump : MonoBehaviour
{
[UnitTitle("Move And Jump Smooth")]
[UnitCategory("Custom/Player")]
public class MoveJumpSmoothNode : Unit
{
    [DoNotSerialize] public ControlInput input;
    [DoNotSerialize] public ControlOutput output;
    [DoNotSerialize] public ValueInput navMeshAgent;
    [DoNotSerialize] public ValueInput moveSpeed;
    [DoNotSerialize] public ValueInput jumpForce;
    [DoNotSerialize] public ValueInput airControl;
    
    private NavMeshAgent agent;
    private float speed;
    private float jumpPower;
    private float airControlFactor;
    private bool jumping = false;
    private Rigidbody rb;
    private Vector3 lastAgentPosition;
    
    protected override void Definition()
    {
        input = ControlInput("Update", (flow) =>
        {
            agent = flow.GetValue<NavMeshAgent>(navMeshAgent);
            speed = flow.GetValue<float>(moveSpeed);
            jumpPower = flow.GetValue<float>(jumpForce);
            airControlFactor = flow.GetValue<float>(airControl);
            
            if (rb == null)
            {
                rb = agent.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    Debug.LogError("❌ No hay Rigidbody en el Player!");
                    return output;
                }
                
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.useGravity = true;
                rb.mass = 1f;
                rb.linearDamping = 0f;
                rb.angularDamping = 0.05f;
                
                // IMPORTANTE: Sincronizar posición inicial
                lastAgentPosition = agent.transform.position;
            }
            
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard == null) return output;
            
            // Obtener input
            float h = 0, v = 0;
            if (keyboard.dKey.isPressed) h++;
            if (keyboard.aKey.isPressed) h--;
            if (keyboard.wKey.isPressed) v++;
            if (keyboard.sKey.isPressed) v--;
            
            Vector3 inputDirection = new Vector3(h, 0, v).normalized;
            
            // Detectar suelo (más preciso)
            bool grounded = Physics.CheckSphere(agent.transform.position - Vector3.up * 0.5f, 0.4f);
            
            if (!jumping)
            {
                // === EN SUELO ===
                if (grounded)
                {
                    // Mantener NavMeshAgent actualizado con la posición real
                    if (!agent.enabled)
                    {
                        agent.enabled = true;
                    }
                    
                    // Movimiento con NavMeshAgent
                    if (inputDirection.magnitude > 0.1f)
                    {
                        Vector3 move = agent.transform.TransformDirection(inputDirection);
                        agent.Move(move * speed * Time.deltaTime);
                    }
                    else
                    {
                        // Detener el agente cuando no hay input
                        agent.velocity = Vector3.zero;
                    }
                    
                    // Guardar posición para referencia
                    lastAgentPosition = agent.transform.position;
                    
                    // Verificar salto
                    if (keyboard.spaceKey.wasPressedThisFrame)
                    {
                        StartJump();
                    }
                }
                else
                {
                    // Cayó del borde sin saltar
                    StartJump();
                }
            }
            else
            {
                // === EN AIRE ===
                
                // Movimiento en el aire
                if (inputDirection.magnitude > 0.1f)
                {
                    Vector3 airMove = agent.transform.TransformDirection(inputDirection);
                    Vector3 targetVelocity = airMove * speed * airControlFactor;
                    Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                    
                    Vector3 newVelocity = Vector3.Lerp(currentHorizontalVelocity, targetVelocity, Time.deltaTime * 5f);
                    rb.linearVelocity = new Vector3(newVelocity.x, rb.linearVelocity.y, newVelocity.z);
                }
                
                // Gravedad adicional
                if (rb.linearVelocity.y < 0)
                {
                    rb.linearVelocity += Vector3.up * Physics.gravity.y * 1.5f * Time.deltaTime;
                }
                
                // Verificar aterrizaje
                if (grounded && rb.linearVelocity.y <= 0.1f)
                {
                    Land();
                }
            }
            
            // Debug visual
            Debug.DrawRay(agent.transform.position, Vector3.down * 1f, grounded ? Color.green : Color.red);
            
            return output;
        });
        
        output = ControlOutput("OnUpdated");
        
        navMeshAgent = ValueInput<NavMeshAgent>("NavMesh Agent", null);
        moveSpeed = ValueInput<float>("Move Speed", 5f);
        jumpForce = ValueInput<float>("Jump Force", 8f);
        airControl = ValueInput<float>("Air Control", 0.5f);
        
        Requirement(navMeshAgent, input);
        Requirement(moveSpeed, input);
        Requirement(jumpForce, input);
        Requirement(airControl, input);
        Succession(input, output);
    }
    
    private void StartJump()
    {
        // Guardar velocidad actual
        Vector3 currentVelocity = agent.velocity;
        
        // DESACTIVAR COMPLETAMENTE el NavMeshAgent
        agent.enabled = false;
        
        // Activar Rigidbody
        rb.isKinematic = false;
        
        // Transferir velocidad horizontal
        rb.linearVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        
        // Aplicar salto
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        
        jumping = true;
        Debug.Log($"🟢 SALTO - Pos: {rb.position}");
    }
    
    private void Land()
    {
        // GUARDAR posición actual del Rigidbody
        Vector3 landingPosition = rb.position;
        
        // Desactivar física primero
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        
        // Reactivar NavMeshAgent SIN CAMBIAR SU POSICIÓN
        agent.enabled = true;
        
        // FORZAR la posición del NavMeshAgent a la posición de aterrizaje
        agent.transform.position = landingPosition;
        
        // Resetear el destino del agente para evitar que se mueva solo
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        
        jumping = false;
        Debug.Log($"🟢 ATERRIZAJE - Pos: {landingPosition}");
    }
}
}
