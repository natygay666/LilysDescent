using UnityEngine;
using Unity.VisualScripting;

public class MovePlayerWithInput : MonoBehaviour
{
[UnitTitle("Move Player With Input")]
[UnitCategory("Custom/Player")]
public class MovePlayerNode : Unit
{
    [DoNotSerialize]
    public ControlInput input;
    
    [DoNotSerialize]
    public ControlOutput output;
    
    [DoNotSerialize]
    public ValueInput playerTransform;
    
    [DoNotSerialize]
    public ValueInput moveSpeed;
    
    [DoNotSerialize]
    public ValueInput horizontalAxis; // Nombre del eje horizontal
    
    [DoNotSerialize]
    public ValueInput verticalAxis;   // Nombre del eje vertical
    
    [DoNotSerialize]
    public ValueOutput isMoving;
    
    [DoNotSerialize]
    public ValueOutput currentDirection;
    
    private Transform player;
    private float speed;
    private string horizontal;
    private string vertical;
    
    private bool moving;
    private Vector3 moveDirection;
    
    protected override void Definition()
    {
        input = ControlInput("Update", (flow) =>
        {
            player = flow.GetValue<Transform>(playerTransform);
            speed = flow.GetValue<float>(moveSpeed);
            horizontal = flow.GetValue<string>(horizontalAxis);
            vertical = flow.GetValue<string>(verticalAxis);
            
            // Usar el sistema de Input que esté activo
            float h = 0;
            float v = 0;
            
            #if ENABLE_INPUT_SYSTEM
                // Nuevo Input System
                var keyboard = UnityEngine.InputSystem.Keyboard.current;
                if (keyboard != null)
                {
                    if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h += 1;
                    if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) h -= 1;
                    if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) v += 1;
                    if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) v -= 1;
                }
            #else
                // Legacy Input Manager
                h = Input.GetAxis(horizontal);
                v = Input.GetAxis(vertical);
            #endif
            
            moveDirection = new Vector3(h, 0, v);
            moving = moveDirection.magnitude > 0.1f;
            
            if (moving)
            {
                moveDirection.Normalize();
            }
            
            Vector3 finalMovement = player.TransformDirection(moveDirection) * speed * Time.deltaTime;
            player.position += finalMovement;
            
            return output;
        });
        
        output = ControlOutput("OnUpdated");
        
        playerTransform = ValueInput<Transform>("Player", null);
        moveSpeed = ValueInput<float>("Speed", 5f);
        horizontalAxis = ValueInput<string>("Horizontal Axis", "Horizontal");
        verticalAxis = ValueInput<string>("Vertical Axis", "Vertical");
        
        isMoving = ValueOutput<bool>("Is Moving", (flow) => moving);
        currentDirection = ValueOutput<Vector3>("Direction", (flow) => moveDirection);
        
        Requirement(playerTransform, input);
        Requirement(moveSpeed, input);
        Requirement(horizontalAxis, input);
        Requirement(verticalAxis, input);
        
        Succession(input, output);
        
        Assignment(input, isMoving);
        Assignment(input, currentDirection);
    }
}
}
