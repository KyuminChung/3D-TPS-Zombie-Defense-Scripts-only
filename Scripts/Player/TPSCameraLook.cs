using UnityEngine;

public sealed class TPSCameraLook : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Sensitivity")]
    [SerializeField] private float sensitivity = 120f;
    [SerializeField] private float minPitch = -40f;
    [SerializeField] private float maxPitch = 60f;

    private PlayerInput input;
    private float pitch;

    void Awake()
    {
        input = player.GetComponent<PlayerInput>();
    }

    void Update()
    {
        RotateView();
    }

    void RotateView()
    {
        // input.LookInput.x → 마우스를 좌/우로 얼마나 움직였는지
        //  sensitivity(감도) 값이 클수록 더 많이 회전
        float mouseX = input.LookInput.x * sensitivity * Time.deltaTime;
        // input.LookInput.y → 마우스를 상/하로 얼마나 움직였는지
        float mouseY = input.LookInput.y * sensitivity * Time.deltaTime;

        // 좌우 회전 (Player)
        player.Rotate(Vector3.up * mouseX); // 플레이어 오브젝트 회전

        // 상하 회전 (Camera Pivot)
        pitch -= mouseY;    // 마우스를 위로 밀면 mouseY가 양수 , pitch 감소 → 아래로 고개 숙임
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);     // pitch 제한
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);      // 위아래만 카메라가 움직임
    }
}
