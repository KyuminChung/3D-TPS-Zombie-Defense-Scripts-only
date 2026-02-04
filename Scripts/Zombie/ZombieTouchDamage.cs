using UnityEngine;

public class ZombieTouchDamage : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;    // 닿을 때마다 데미지
    [SerializeField] private float damageInterval = 1.0f; // 데미지 간격

    [Header("Target Tag")]
    [SerializeField] private string playerTag = "Player";   // 플레이어 태그

    private float nextDamageTime = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;   // 플레이어 태그가 아니면 무시
        if (Time.time < nextDamageTime) return;     // 데미지 간격 체크

        // PlayerHealth 찾기 (플레이어 루트에 있든, 자식에 있든 대응)
        var health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        health.TakeDamage(damage);  // 데미지 적용
        nextDamageTime = Time.time + damageInterval;    // 다음 데미지 시간 갱신
    }
}
