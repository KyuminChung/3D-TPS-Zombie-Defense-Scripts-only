using UnityEngine;
using UnityEngine.AI;   // NavMeshAgent 사용 위해 필요

public sealed class ZombieAI : MonoBehaviour
{
    [Header("Attack")]  
    [SerializeField] private float attackDistance = 1.6f;   // 공격 사거리
    [SerializeField] private float attackCooldown = 1.2f;   // 공격 쿨타임
    [SerializeField] private float attackAnimDuration = 0.8f;   // 공격 애니메이션 지속 시간

    private NavMeshAgent agent; // 네브메시 에이전트
    private Animator animator;  // 애니메이터
    private PlayerHealth target;    // 플레이어 체력 컴포넌트
    private ZombieHealth health;    // 좀비 체력 컴포넌트

    private bool isAttacking;   // 공격 중인지 여부
    private bool isDead;    // 사망 여부
    private float lastAttackTime;   // 마지막 공격 시점

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        health = GetComponent<ZombieHealth>();
    }

    // =======================   풀링 초기화 (Spawner 호출)  ======================= 
    public void OnSpawned(PlayerHealth player)
    {
        target = player;

        isDead = false;
        isAttacking = false;
        lastAttackTime = 0f;

        agent.enabled = true;
        agent.isStopped = false;
        agent.ResetPath();  // 경로 초기화

        if (health != null)
            health.ResetHealth();

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        animator.Rebind();  // 애니메이터 상태 초기화
        animator.Update(0f);    // 애니메이터 즉시 업데이트
    }

    void Update()
    {
        if (isDead || target == null || isAttacking)    //죽었거나, 타겟이 없거나, 공격 중이면 아무 것도 하지 않고 종료.
            return;
        // 좀비와 플레이어 사이의 거리 계산.
        float dist = Vector3.Distance(transform.position, target.transform.position);
        // 거리가 공격 사거리보다 멀면 플레이어 쪽으로 이동.
        if (dist > attackDistance)
        {
            // NavMeshAgent를 움직이게 하고, 목적지를 플레이어 위치로 설정.
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }
        else
        {
            // 공격 거리 안이면 “공격 시도”
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        agent.isStopped = true;
        animator.SetTrigger(AttackHash);

        // 공격 즉시 1회 데미지
        //target.TakeDamage(damage);   -> ZombieTouchDamage.cs에서 처리하도록 변경.

        Invoke(nameof(EndAttack), attackAnimDuration);
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    // =======================  사망   =======================
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;

        agent.isStopped = true;
        agent.enabled = false;

        Collider col = GetComponent<Collider>();    
        if (col != null)
            col.enabled = false;    // 죽은 동안 총알/접촉 판정이 다시 일어나지 않도록 콜라이더 비활성화.

        animator.ResetTrigger(AttackHash);  // 공격 애니메이션 트리거 리셋
        animator.SetTrigger(DieHash);   // 사망 애니메이션 재생

        GameManager.Instance.AddScore(1);   // 점수 추가

        Invoke(nameof(ReturnToPool), 3f);   // 3초 후에 풀로 반환
    }

    void ReturnToPool()
    {
        ZombiePool.Instance.ReturnZombie(gameObject);
    }
}
