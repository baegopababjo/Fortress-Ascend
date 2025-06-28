using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
    }

    public static EnemyPoolManager Instance;
    public List<Pool> pools;

    // 📦 풀 저장소 및 EMA 예측용 캐시
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;
    private Dictionary<GameObject, float> emaPoolSizes = new(); // 프리팹별 EMA 추적
    private const float smoothingFactor = 0.3f; // EMA 가중치

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializePools(); // 👈 풀 초기화
    }

    /// 각 프리팹에 대해 초기 풀 생성
    void InitializePools()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (var pool in pools)
        {
            poolDictionary[pool.prefab] = CreatePoolForPrefab(pool.prefab, pool.size);
        }
    }

    /// 주어진 프리팹으로 오브젝트 풀 생성
    Queue<GameObject> CreatePoolForPrefab(GameObject prefab, int size)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, Vector3.one * 9999f, Quaternion.identity); // 📍 시야 밖 생성
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }

        return objectPool;
    }

    /// 오브젝트 풀에서 꺼내 위치 설정 후 반환
    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning("풀에 해당 프리팹이 없습니다: " + prefab.name);
            return null;
        }

        GameObject objectToSpawn = poolDictionary[prefab].Dequeue();
        objectToSpawn.transform.SetPositionAndRotation(position, rotation);
        objectToSpawn.SetActive(true);
        poolDictionary[prefab].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    /// 현재 필요량을 기반으로 EMA 예측 적용
    public void EnsurePoolSizeWithPrediction(GameObject prefab, int currentRequired)
    {
        int predictedSize = PredictPoolSize(prefab, currentRequired);
        EnsurePoolSize(prefab, predictedSize);
    }

    /// EMA 기반 풀 크기 예측
    private int PredictPoolSize(GameObject prefab, int currentRequired)
    {
        if (!emaPoolSizes.ContainsKey(prefab))
        {
            emaPoolSizes[prefab] = currentRequired;
        }
        else
        {
            emaPoolSizes[prefab] = smoothingFactor * currentRequired + (1 - smoothingFactor) * emaPoolSizes[prefab];
        }

        return Mathf.CeilToInt(emaPoolSizes[prefab] * 1.1f); // 예측값보다 여유 있게 확보
    }

    /// 지정된 크기만큼 풀 확장
    public void EnsurePoolSize(GameObject prefab, int requiredSize)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab] = new Queue<GameObject>();
        }

        Queue<GameObject> pool = poolDictionary[prefab];
        int currentSize = pool.Count;

        for (int i = currentSize; i < requiredSize; i++)
        {
            GameObject obj = Instantiate(prefab, Vector3.one * 9999f, Quaternion.identity);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// 모든 풀의 오브젝트 초기화 및 비활성화
    public void ResetAllPools()
    {
        foreach (var kvp in poolDictionary)
        {
            foreach (var obj in kvp.Value)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    var ai = obj.GetComponent<NPC_AI>(); // 상태 초기화 가능
                }
            }
        }
    }
}
