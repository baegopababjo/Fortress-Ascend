using UnityEngine;
using TMPro;

public class GoldUIManager : MonoBehaviour
{
    public TextMeshProUGUI goldText; // 돈을 표시할 UI 텍스트
    private PlayerStats playerStats; // 🔥 플레이어 스탯 자동 할당

    void Start()
    {
        FindPlayerStats();
        UpdateGoldUI(); // 🎯 Start에서 한 번 실행 (null 체크 포함)
    }

    void Update()
    {
        // ⏳ playerStats가 null이면 계속 찾아서 할당
        if (playerStats == null)
        {
            FindPlayerStats();
        }

        // 🎯 Gold UI 업데이트 (null 체크 추가)
        if (playerStats != null)
        {
            UpdateGoldUI();
        }
    }

    void FindPlayerStats()
    {
        int playerLayer = LayerMask.NameToLayer("Player"); // 🎯 "Player" 레이어 가져오기
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None); // 🔥 최적화된 방식 사용

        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == playerLayer)
            {
                playerStats = obj.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    //Debug.Log($"✅ PlayerStats 할당 완료! {obj.name}에서 찾음.");
                    return;
                }
            }
        }

        //Debug.LogWarning("⚠️ 아직 PlayerStats를 찾지 못했습니다. Update에서 다시 확인합니다.");
    }

    void UpdateGoldUI()
    {
        if (playerStats == null || goldText == null) return; // 🔥 예외 방지
        goldText.text = playerStats.gold.ToString("N0"); // 1,000 형태로 숫자 포맷
    }
}
