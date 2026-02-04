using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public sealed class TPSPlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float runSpeed = 6f;   // 달리기 속도(Shift 안 누를 때).
    [SerializeField] private float walkSpeed = 3f;  // 걷기 속도(Shift 누를 때)

    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;    // 중력 가속도

    [Header("Animation")]
    [SerializeField] private Animator animator; 

    private CharacterController controller;
    private PlayerInput input;
    private float verticalVelocity;     //  현재 Y방향 속도(중력 누적용).
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        Move();
        UpdateAnimation();
    }

    void Move()     //  XZ 이동 + 중력 적용 + 컨트롤러 이동을 담당하는 함수.
    {
        Vector3 inputDir = new Vector3(input.MoveInput.x, 0f, input.MoveInput.y);

        // Shift가 눌렸으면 걷기 속도, 아니면 달리기 속도 선택.
        float speed = input.IsWalkPressed ? walkSpeed : runSpeed;       
        Vector3 move = transform.TransformDirection(inputDir.normalized) * speed;

        ApplyGravity(ref move);     // 중력 적용, ref라서 함수 안에서 move.y를 직접 바꿀 수 있음.
        controller.Move(move * Time.deltaTime);     
    }

    void ApplyGravity(ref Vector3 move)     // 점프는 추가 안해서 아직..
    {
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;
    }

    void UpdateAnimation()
    {
        bool isMoving = input.MoveInput.sqrMagnitude > 0.01f;
        float speedParam = isMoving
            ? (input.IsWalkPressed ? 0.5f : 1f)
            : 0f;

        animator.SetFloat(SpeedHash, speedParam);
    }
}
