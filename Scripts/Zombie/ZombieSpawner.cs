using UnityEngine;
using UnityEngine.AI;

public sealed class ZombieSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth; // 플레이어 체력 참조
    [SerializeField] private Transform[] spawnPoints;   // 스폰 위치 배열

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;  //  스폰 간격
    [SerializeField] private float navMeshSampleRadius = 2f;    //  NavMesh 샘플링 반경

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, spawnInterval);  // 1초 후부터 spawnInterval 간격으로 Spawn() 반복 호출
    }

    void Spawn()
    {
        if (playerHealth == null || spawnPoints == null || spawnPoints.Length == 0) // 유효성 검사
            return;

        Vector3 spawnPos =  
            spawnPoints[Random.Range(0, spawnPoints.Length)].position;  // 랜덤 스폰 위치 선택

        if (NavMesh.SamplePosition(       // NavMesh 상의 유효한 위치로 보정
                spawnPos,
                out NavMeshHit hit,
                navMeshSampleRadius,
                NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }

        GameObject zombie = ZombiePool.Instance.GetZombie();    // 풀에서 좀비 오브젝트 가져오기
        if (zombie == null) return;

        // 위치 세팅
        NavMeshAgent agent = zombie.GetComponent<NavMeshAgent>();   
        if (agent != null)      // NavMeshAgent가 있으면 위치 설정 시 비활성화 후 재활성화
        {
            agent.enabled = false;
            zombie.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
            agent.enabled = true;
            agent.Warp(spawnPos);
        }
        else
        {
            zombie.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
        }

        //  풀링 초기화 호출 
        ZombieAI ai = zombie.GetComponent<ZombieAI>();  
        if (ai != null)
        {
            ai.OnSpawned(playerHealth); 
        }
    }
}
