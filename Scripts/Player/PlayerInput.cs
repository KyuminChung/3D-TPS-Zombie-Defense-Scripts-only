using UnityEngine;

public sealed class PlayerInput : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }      // X: Horizontal, Y: Vertical
    public Vector2 LookInput { get; private set; }      // X: Mouse X, Y: Mouse Y
    public bool IsWalkPressed { get; private set; }     // Walk action (Left Shift)

    void Update()
    {
        ReadMovementInput();
        ReadLookInput();
        ReadActionInput();
    }

    void ReadMovementInput()
    {
        MoveInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
    }

    void ReadLookInput()
    {
        LookInput = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );
    }

    void ReadActionInput()
    {
        IsWalkPressed = Input.GetKey(KeyCode.LeftShift);
    }
}
