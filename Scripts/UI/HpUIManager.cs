using UnityEngine;
using UnityEngine.UI;

public class HpUIManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text hpText;

    private PlayerStats playerStats;

    public void SetTarget(GameObject player)
    {
        playerStats = player.GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("❌ PlayerStats를 'Player' 오브젝트에서 찾을 수 없습니다!");
            return;
        }

        //Debug.Log($"✅ PlayerStats 연결 완료: {playerStats.name}");

            //완전히 초기화된 시점에만 UI를 갱신
        playerStats.OnStatsInitialized += UpdateHpUI;
    }

    public void UpdateHpUI()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("⚠️ UpdateHpUI 호출됨 - playerStats가 설정되지 않음");
            return;
        }

        int currentHp = Mathf.Max(playerStats.health, 0); // 체력이 0 아래로 내려가지 않게
        int maxHp = playerStats.gameStatsData.GetPlayerLevelStats(playerStats.level).Item1;

        hpSlider.maxValue = maxHp;
        hpSlider.value = currentHp;
        hpText.text = $"{currentHp} / {maxHp}";

        Debug.Log($"💚 HP UI 업데이트됨: {currentHp} / {maxHp}");
    }
}
