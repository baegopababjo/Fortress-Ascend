using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class NpcPlacementManager : MonoBehaviour
{
    public static NpcPlacementManager Instance { get; private set; }

    [Header("NPC Prefabs")]
    public List<NpcPrefabData> npcPrefabs;

    private GameObject previewObject;
    private bool isPlacingNpc = false;
    private GameObject selectedPrefab;
    private Camera mainCamera;
    private bool canPlaceNpc = false;

    public bool IsPlacingNpc() => isPlacingNpc; //NPC 설치 여부함수


    [System.Serializable]
    public class NpcPrefabData
    {
        public string npcName;
        public GameObject prefab;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isPlacingNpc && previewObject != null)
        {
            MovePreviewObject();

            if (Input.GetMouseButtonDown(0) && canPlaceNpc)
            {
                PlaceNpc();
            }
        }
    }

    public bool StartNpcPlacement(string npcName, int level)
    {
        selectedPrefab = GetNpcPrefab(npcName);
        if (selectedPrefab == null)
        {
            Debug.LogError($"❌ {npcName} 프리팹을 찾을 수 없습니다!");
            return false;
        }

        previewObject = Instantiate(selectedPrefab);
        previewObject.GetComponent<Collider>().enabled = false;
        previewObject.GetComponent<Renderer>().material.color = new Color(1, 1, 0, 0.5f); // 노란색 반투명

        // ✅ Preview 상태로 전환
        var ai = previewObject.GetComponent<NPC_AI>();
        if (ai != null)
        {
            ai.SetPreviewState(); // 🎯 여기서 AI가 Chase/Attack 안 하도록 막기
        }

        isPlacingNpc = true;

        // ✅ 클래스 설정 & 레벨 적용
        var npcStats = previewObject.GetComponent<NPC_Stats>();
        if (npcStats != null)
        {
            npcStats.SetNPC(previewObject.transform, npcName.ToLower());
            npcStats.SetLevelStats(level);
            npcStats.ApplyClassStats();
        }

        return true;
    }

    private GameObject GetNpcPrefab(string npcName)
    {
        foreach (var npc in npcPrefabs)
        {
            if (npc.npcName == npcName)
                return npc.prefab;
        }

        Debug.LogError($"❌ {npcName} 프리팹을 찾을 수 없음");
        return null;
    }

    private void MovePreviewObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                previewObject.transform.position = hit.point;
                previewObject.transform.SetParent(null);
                canPlaceNpc = true;
            }
            else
            {
                canPlaceNpc = false;
            }
        }
    }

    private void PlaceNpc()
    {
        if (!canPlaceNpc || previewObject == null)
        {
            Debug.Log("❌ NPC를 배치할 수 없습니다!");
            return;
        }

        previewObject.GetComponent<Collider>().enabled = true;
        previewObject.GetComponent<Renderer>().material.color = Color.white;

        // ✅ 무기 Collider 다시 활성화
        var sword = previewObject.GetComponentInChildren<Sword>();
        if (sword != null)
        {
            Collider weaponCollider = sword.GetComponent<Collider>();
            if (weaponCollider != null)
                weaponCollider.enabled = true;
        }

        // ✅ Idle 상태 진입 (이게 '배치 완료' 의미니까 여기서!)
        var ai = previewObject.GetComponent<NPC_AI>();
        if (ai != null)
        {
            ai.batch(); // 배치 완료 → Idle 상태로 진입
        }

        previewObject = null;
        canPlaceNpc = false;

        // ✅ 배치 상태 false는 한 프레임 뒤에 처리
        StartCoroutine(DelayedPlacementEnd());

        Debug.Log("✅ NPC 설치 완료!");
    }

    private IEnumerator DelayedPlacementEnd()
    {
        yield return null; // 한 프레임 대기
        isPlacingNpc = false;
    }
}
