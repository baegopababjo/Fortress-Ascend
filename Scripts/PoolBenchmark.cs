using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Profiling;

public class PoolBenchmark : MonoBehaviour
{
    public GameObject prefab;
    public int testCount = 1000;

    private List<GameObject> pooledObjects = new List<GameObject>();
    private Queue<GameObject> pool = new Queue<GameObject>();

    // EMA 관련 변수
    private float emaPoolSize = 0f;
    private const float smoothingFactor = 0.7f;

    void Start()
    {
        StartCoroutine(RunBenchmark());
    }

    IEnumerator RunBenchmark()
    {
        yield return new WaitForSeconds(1f);

        UnityEngine.Debug.Log("🔽 Instantiate/Destroy 테스트 시작");
        yield return StartCoroutine(TestInstantiateAndDestroy());

        yield return new WaitForSeconds(1f);

        UnityEngine.Debug.Log("🔼 Object Pooling 테스트 시작");
        yield return StartCoroutine(TestObjectPooling());

        yield return new WaitForSeconds(1f);

        UnityEngine.Debug.Log("📈 EMA 기반 Object Pooling 테스트 시작");
        yield return StartCoroutine(TestEMABasedPooling());
    }

    IEnumerator TestInstantiateAndDestroy()
    {
        long memoryBefore = Profiler.GetTotalAllocatedMemoryLong();

        Stopwatch sw = new Stopwatch();
        sw.Start();

        for (int i = 0; i < testCount; i++)
        {
            GameObject go = Instantiate(prefab);
            Destroy(go);
        }

        sw.Stop();

        long memoryAfter = Profiler.GetTotalAllocatedMemoryLong();
        long memoryUsed = memoryAfter - memoryBefore;

        UnityEngine.Debug.Log($"🛠 Instantiate + Destroy: {sw.ElapsedMilliseconds} ms, Memory Used: {memoryUsed / 1024f / 1024f:F2} MB");

        yield return null;
    }

    IEnumerator TestObjectPooling()
    {
        // 초기 풀 생성
        for (int i = 0; i < testCount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        long memoryBefore = Profiler.GetTotalAllocatedMemoryLong();

        Stopwatch sw = new Stopwatch();
        sw.Start();

        // 사용
        for (int i = 0; i < testCount; i++)
        {
            GameObject go = pool.Dequeue();
            go.SetActive(true);
            pooledObjects.Add(go);
        }

        // 반환
        foreach (var obj in pooledObjects)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        sw.Stop();

        long memoryAfter = Profiler.GetTotalAllocatedMemoryLong();
        long memoryUsed = memoryAfter - memoryBefore;

        UnityEngine.Debug.Log($"🚀 Object Pooling (Activate/Deactivate): {sw.ElapsedMilliseconds} ms, Memory Used: {memoryUsed / 1024f / 1024f:F2} MB");

        yield return null;
    }

    IEnumerator TestEMABasedPooling()
    {
        // EMA 초기화
        emaPoolSize = testCount;

        // 초기 풀 생성
        Queue<GameObject> emaPool = new Queue<GameObject>();
        for (int i = 0; i < testCount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            emaPool.Enqueue(obj);
        }

        long memoryBefore = Profiler.GetTotalAllocatedMemoryLong();

        Stopwatch sw = new Stopwatch();
        sw.Start();

        // 사용 및 EMA 업데이트
        List<GameObject> activeObjects = new List<GameObject>();
        for (int i = 0; i < testCount; i++)
        {
            // EMA 예측값 계산
            emaPoolSize = smoothingFactor * testCount + (1 - smoothingFactor) * emaPoolSize;
            int predictedSize = Mathf.CeilToInt(emaPoolSize * 1.1f);

            // 풀 크기 보장
            while (emaPool.Count < predictedSize)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                emaPool.Enqueue(obj);
            }

            GameObject go = emaPool.Dequeue();
            go.SetActive(true);
            activeObjects.Add(go);
        }

        // 반환
        foreach (var obj in activeObjects)
        {
            obj.SetActive(false);
            emaPool.Enqueue(obj);
        }

        sw.Stop();

        long memoryAfter = Profiler.GetTotalAllocatedMemoryLong();
        long memoryUsed = memoryAfter - memoryBefore;

        UnityEngine.Debug.Log($"📈 EMA 기반 Object Pooling: {sw.ElapsedMilliseconds} ms, Memory Used: {memoryUsed / 1024f / 1024f:F2} MB");

        yield return null;
    }
}
