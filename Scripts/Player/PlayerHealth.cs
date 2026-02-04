using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 3;     // 최대 체력
    public int MaxHp => maxHp;
    public int Hp { get; private set; }

    public event Action OnDied; // HP가 0이 되었을 때 호출되는 사망 이벤트.

    void Awake()
    {
        ResetHealth();  // 시작 시 체력을 최대치로 초기화.
    }

    public void TakeDamage(int amount)      // 데미지를 입히는 메서드
    {
        if (amount <= 0 || Hp <= 0) return;
        SetHp(Hp - amount);
        if (Hp <= 0) Die();
    }
    public void ResetHealth() { SetHp(maxHp); }
    void SetHp(int value)    // 내부적으로 체력을 설정하는 메서드
    {
        Hp = Mathf.Clamp(value, 0, maxHp);  // HP를 0~최대 체력 사이로 강제 제한.        
    }

    void Die()
    {
        OnDied?.Invoke();     // 사망 이벤트 발생.
    }
}
