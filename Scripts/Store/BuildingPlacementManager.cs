using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Collections;
using System.Collections.Generic;

public class BuildingPlacementManager : MonoBehaviour
{
    public static BuildingPlacementManager Instance { get; private set; }

    [Header("Building Prefabs")]
    public List<BuildingPrefabData> buildingPrefabs;

    private GameObject previewObject;
    private bool isPlacingBuilding = false;
    private Camera mainCamera;
    private GameObject selectedPrefab;
    private bool canPlaceBuilding = false;

    [Header("NavMesh")]
    public NavMeshSurface navMeshSurface;

    private Transform currentSurface;
    private Transform selectedSlot = null;

    [System.Serializable]
    public class BuildingPrefabData
    {
        public string buildingName;
        public GameObject prefab;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (navMeshSurface == null)
            Debug.LogError("❌ [BuildingPlacementManager] NavMeshSurface가 연결되지 않았습니다! Unity Inspector에서 설정해주세요.");
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isPlacingBuilding && previewObject != null)
        {
            MovePreviewObject();

            if (Input.GetMouseButtonDown(0) && canPlaceBuilding)
            {
                PlaceBuilding();
                CursorUtils.HideCursor();
            }
        }
    }

    public bool IsPlacingBuilding() => isPlacingBuilding;

    public bool StartBuildingPlacement(string buildingName, int level, int health)
    {
        selectedPrefab = GetBuildingPrefab(buildingName);
        if (selectedPrefab == null)
        {
            Debug.LogError($"❌ {buildingName}에 해당하는 프리팹을 찾을 수 없습니다!");
            return false;
        }
        string prefabName = selectedPrefab.name;

        // 프리뷰 생성 (수동 설치는 성공으로 간주)
        previewObject = Instantiate(selectedPrefab);
        previewObject.GetComponent<Collider>().enabled = false;
        previewObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.5f);
        isPlacingBuilding = true;

        var damageHandler = previewObject.GetComponent<BuildingDamageHandler>();
        if (damageHandler != null)
        {
            damageHandler.InitializeBuilding(buildingName, level, health);
            //Debug.Log($"📦 [프리뷰 생성] {buildingName} | 레벨: {level} | 체력: {health}");
        }

        return true;
    }

    public bool PlaceBuildingInSlot(string buildingName, int level, int health, Transform slotTransform)
    {
        if (slotTransform.childCount > 0)
        {
            Debug.LogWarning("❌ 슬롯에 이미 건물이 존재합니다!");
            return false;
        }

        GameObject prefab = GetBuildingPrefab(buildingName);
        if (prefab == null) return false;

        GameObject building = Instantiate(prefab);
        float yOffset = GetPrefabHeight(building);

        building.transform.position = slotTransform.position + Vector3.up * yOffset;
        building.transform.rotation = slotTransform.rotation;
        building.transform.SetParent(slotTransform);

        var handler = building.GetComponent<BuildingDamageHandler>();
        if (handler != null)
            handler.InitializeBuilding(buildingName, level, health);

        var ai = building.GetComponent<Tower_NPC_AI>();
        if (ai != null)
            ai.overrideLevel = level;

        navMeshSurface.BuildNavMesh();

        return true;
    }




    private GameObject GetBuildingPrefab(string buildingName)
    {
        foreach (var building in buildingPrefabs)
        {
            //Debug.Log($"🔍 비교 중: {building.buildingName} vs {buildingName}");
            if (building.buildingName == buildingName)
                return building.prefab;
        }
        Debug.LogError($"❌ GetBuildingPrefab: {buildingName} 프리팹을 찾을 수 없음");
        return null;
    }

    //private bool TryAutoPlaceOnSlot(string buildingName, int level, int health)
    //{
    //    string tagToSearch = buildingName.Contains("Building C") ? "Wall" : "Tower";
    //    GameObject[] targets = GameObject.FindGameObjectsWithTag(tagToSearch);

    //    foreach (var target in targets)
    //    {
    //        foreach (Transform child in target.transform)
    //        {
    //            if (!child.name.StartsWith("Slot")) continue;
    //            if (child.childCount > 0) continue;

    //            // 🔍 디버그용 Slot 정보 출력
    //            //Debug.Log($"📌 슬롯 정보 → 이름: {child.name} | 로컬 위치: {child.localPosition} | 월드 위치: {child.position} | 부모: {target.name}");

    //            GameObject newBuilding = Instantiate(GetBuildingPrefab(buildingName));
    //            float yOffset = GetPrefabHeight(newBuilding);

    //            newBuilding.transform.position = child.position + Vector3.up * yOffset;
    //            newBuilding.transform.rotation = child.rotation;
    //            newBuilding.transform.SetParent(child); // 이거 문제!

    //            // 💥 레벨 정보 반영!
    //            var ai = newBuilding.GetComponent<Tower_NPC_AI>();
    //            if (ai != null)
    //            {
    //                ai.overrideLevel = level; // ✅ 여기서 레벨 넘겨줌
    //            }

    //            // 💥 체력 정보 반영!

    //            var handler = newBuilding.GetComponent<BuildingDamageHandler>();
    //            if (handler != null)
    //                handler.InitializeBuilding(buildingName, level, health);

    //            //Debug.Log($"✅ {buildingName} 자동 설치됨 → {child.name} on {target.name}");
    //            navMeshSurface.BuildNavMesh();
    //            return true;
    //        }
    //    }

    //    Debug.Log($"❌ {buildingName} 설치 실패: 빈 슬롯 없음 ({tagToSearch} 대상)");
    //    return false;
    //}



    private void MovePreviewObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        int layerMask = ~LayerMask.GetMask("Building"); // 🔥 이 레이어는 무시!

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            //Debug.Log($"🎯 Ray hit: {hit.collider.name} | Tag: {hit.collider.tag}");

            if (hit.collider.CompareTag("Ground"))
            {
                previewObject.transform.position = hit.point;
                previewObject.transform.SetParent(null);
                canPlaceBuilding = true;
                currentSurface = hit.collider.transform;
                selectedSlot = null;
                //Debug.Log($"🌍 일반 건물 위치: {hit.point}");
            }
            else
            {
                canPlaceBuilding = false;
                //Debug.Log("❌ 설치 불가: Ground 외 지역");
            }
        }
        else
        {
            //Debug.Log("❌ Raycast 실패 - 아무것도 맞추지 못함");
        }
    }



    private void PlaceBuilding()
    {
        if (!canPlaceBuilding || previewObject == null)
        {
            Debug.Log("❌ 건물을 배치할 수 없습니다!");
            return;
        }

        previewObject.GetComponent<Collider>().enabled = true;
        previewObject.GetComponent<Renderer>().material.color = Color.white;
        previewObject = null;
        selectedSlot = null;

        Debug.Log("✅ 건물 설치 완료!");

        //UpdateNavMesh();
        StartCoroutine(DisablePlacingBuilding());
    }

    private float GetPrefabHeight(GameObject obj)
    {
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        return renderer != null ? renderer.bounds.size.y / 2f : 0f;
    }

    private IEnumerator DisablePlacingBuilding()
    {
        yield return new WaitForSeconds(0.2f);
        isPlacingBuilding = false;
        //Debug.Log("🏗 건물 배치 완료!");
    }

    private void UpdateNavMesh()
    {
        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
        else
            Debug.LogError("❌ NavMeshSurface가 연결되지 않았습니다!");
    }
}
