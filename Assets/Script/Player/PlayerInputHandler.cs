using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Players intent
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool DashPressed { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            MoveInput = context.ReadValue<Vector2>();
        }           
        else if (context.canceled)
        {
            MoveInput = Vector2.zero;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpPressed = true;
            JumpHeld = true;
        }
        if (context.canceled)
        {
            JumpHeld = false;
        }
    }

    public void ConsumeJumpInput()
    {
        JumpPressed = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            AttackPressed = true;
        }
    }

    public void ConsumeAttack()
    {
        AttackPressed = false;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DashPressed = true;
        }
    }
    public void ConsumeDash()
    {
        DashPressed = false;
    }
}
