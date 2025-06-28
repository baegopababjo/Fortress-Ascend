using UnityEngine;
using System.Collections;

public class NPC_Manager : MonoBehaviour
{
    public GameStatsData gameStatsData; // ScriptableObject 연결
    public GameObject Warrior, Archer;
    public GameObject SpawnPoint;
    BoxCollider col;
    Vector3 zeroPos, randomPos;
    Vector3 pos;
    float rand_X, rand_Z;
    int NPCnum, Level, MaxNowNPCNum;

    void Awake()
    {
        if (SpawnPoint == null)
        {
            Debug.LogError("SpawnPoint가 연결되지 않았습니다!");
            return;
        }

        col = SpawnPoint.GetComponent<BoxCollider>();
        zeroPos = SpawnPoint.transform.position;
        rand_X = col.bounds.size.x; rand_Z = col.bounds.size.z;
        MaxNowNPCNum = 15; // 필드에 존재할 수 있는 최대 적 NPC 수
    }

    public void WaveNPC(int wave)
    {
        if (gameStatsData == null)
        {
            Debug.LogError("NPC_Manager의 gameStatsData가 null입니다!");
            return;
        }

        var waveStats = gameStatsData.GetWaveStats(wave);
        if (waveStats == null)
        {
            Debug.LogError($"Wave {wave}에 해당하는 waveStats를 찾을 수 없습니다!");
            return;
        }

        Level = waveStats.enemyLevel;
        NPCnum = waveStats.enemyCount;

        // ✅ EMA 기반 예측으로 풀 사이즈 확장
        EnemyPoolManager.Instance.EnsurePoolSizeWithPrediction(Warrior, NPCnum);
        EnemyPoolManager.Instance.EnsurePoolSizeWithPrediction(Archer, NPCnum);


        StartCoroutine(SpawnWaveEnemies(NPCnum));
    }

    IEnumerator SpawnWaveEnemies(int num)
    {
        for (int i = 0; i < num; i++)
        {
            RandomClass(); // Warrior/Archer 중 랜덤 생성
            yield return new WaitForSeconds(0.3f);
        }
    }


    // 클래스 종류를 일정 확률에 따라 결정
    void RandomClass()
    {
        float randomClass = Random.value;
        if (randomClass < 0.7f)
        {
            CreateClass(Warrior);
        }
        else
        {
            CreateClass(Archer);
        }
    }

    // 클래스 관련 NPC 생성
    void CreateClass(GameObject prefab)
    {
        Vector3 spawnPosition = RandomPos();
        Quaternion spawnRotation = Quaternion.identity;
        //Debug.Log($"zeropoint = {zeroPos}");
        GameObject enemy = EnemyPoolManager.Instance.SpawnFromPool(prefab, spawnPosition, spawnRotation);
        if (enemy != null)
        {
            enemy.GetComponent<NPC_Stats>().SetLevel(Level);
        }
    }

    Vector3 RandomPos() // NPC의 포지션을 스폰 지역 안에서 무작위로 설정
    {
        rand_X = Random.Range((rand_X / 2) * -1, rand_X / 2); 
        rand_Z = Random.Range((rand_Z / 2) * -1, rand_Z / 2);
        randomPos = new Vector3(rand_X, 0f, rand_Z);

        Vector3 result = zeroPos + randomPos;

        return result;
    }
}
