using UnityEngine;

public sealed class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 40;    // 최대 체력

    public int Hp { get; private set; }

    private ZombieAI zombieAI;

    void Awake()
    {
        zombieAI = GetComponent<ZombieAI>();
        Hp = maxHp;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || Hp <= 0) return;

        Hp = Mathf.Max(Hp - amount, 0);

        if (Hp == 0)
            zombieAI.Die();
    }

    public void ResetHealth()   // 체력을 최대치로 초기화
    {
        Hp = maxHp;
    }
}
