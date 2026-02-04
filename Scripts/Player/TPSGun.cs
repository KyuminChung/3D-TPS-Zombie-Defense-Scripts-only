using UnityEngine;

public sealed class TPSGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;    // 메인 카메라
    [SerializeField] private Transform muzzle;      // 총구 위치

    [Header("Gun Settings")]
    [SerializeField] private float range = 100f;    // 사정거리
    [SerializeField] private int damage = 20;       // 데미지

    [Header("Effects")]
    [SerializeField] private float lineDuration = 0.05f;    // 총알 궤적 지속 시간

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

    void Update()
    {
        if (!CanShoot()) return;    // 게임 상태가 플레이 중이 아닐 때는 발사 불가
        if (Input.GetMouseButtonDown(0))    // 계속 누르고 있는 동안이 아니라 클릭 다운 1회만 반응
            Shoot();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        Vector3 dir = ray.direction;

        bool hitEnemy = false;
        Vector3 endPoint = muzzle.position + dir * range;

        if (Physics.Raycast(muzzle.position, dir, out RaycastHit hit, range))
        {
            endPoint = hit.point;

            // Enemy 판별
            if (hit.collider.transform.CompareTag("Enemy"))
            {
                ZombieHealth health =
                    hit.collider.transform.GetComponent<ZombieHealth>();

                if (health != null)
                {
                    hitEnemy = true;
                    health.TakeDamage(damage);
                }
            }
        }
        DrawLine(muzzle.position, endPoint, hitEnemy);
        Invoke(nameof(HideLine), lineDuration); // lineDuration초 뒤에 HideLine()을 호출해서 라인을 끈다.
    }


    // =======================  Internal  =======================
    bool CanShoot()
    {
        return GameManager.Instance != null &&
               GameManager.Instance.State == GameState.Playing;
    }
    void DrawLine(Vector3 start, Vector3 end, bool enemyHit)    // 적을 맞췄는지에 따라 색상 변경
    {
        Color color = enemyHit ? Color.red : Color.yellow;

        line.startColor = color;
        line.endColor = color;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        line.enabled = true;
    }

    void HideLine()
    {
        line.enabled = false;
    }
}
