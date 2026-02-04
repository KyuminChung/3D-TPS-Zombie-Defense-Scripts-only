using System.Collections.Generic;
using UnityEngine;

public sealed class ZombiePool : MonoBehaviour
{
    public static ZombiePool Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int initialSize = 10;  // 초기 풀 크기

    private readonly Queue<GameObject> pool = new();    // 좀비 오브젝트 풀

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;    // 싱글톤 인스턴스 설정
        Initialize();
    }

    void Initialize()
    {
        for (int i = 0; i < initialSize; i++)   // 초기 풀 크기만큼 좀비 생성
            CreateZombie();
    }

    GameObject CreateZombie()
    {
        GameObject zombie = Instantiate(zombiePrefab, transform);   
        zombie.SetActive(false);
        pool.Enqueue(zombie);   // 풀에 추가
        return zombie;
    }

    public GameObject GetZombie()   // 좀비 요청
    {
        if (pool.Count == 0)    // 풀이 비어있으면 새 좀비 생성
            CreateZombie();

        GameObject zombie = pool.Dequeue(); // 풀에서 좀비 꺼내기
        zombie.SetActive(true);
        return zombie;
    }

    public void ReturnZombie(GameObject zombie)     // 좀비 반환
    {
        zombie.SetActive(false);
        pool.Enqueue(zombie);   // 풀에 다시 추가
    }
}
