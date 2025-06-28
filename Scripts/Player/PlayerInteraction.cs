using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 5f;
    public LayerMask rubbleLayer;
    private Rubble_RepairHandler currentRubble;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, rubbleLayer))
        {
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green); // 시각 디버깅

            Rubble_RepairHandler rubble = hit.collider.GetComponentInParent<Rubble_RepairHandler>();

            if (rubble != null)
            {
                if (currentRubble != rubble)
                {
                    //Debug.Log($"🧭 새 잔해물 감지됨: {rubble.name}");

                    if (currentRubble != null)
                    {
                        //Debug.Log("👋 이전 잔해물 UI 숨김");
                        currentRubble.HideUI();
                    }

                    currentRubble = rubble;
                    currentRubble.ShowUI();
                    //Debug.Log("👁️ 새 잔해물 UI 표시됨");
                }

                if (Input.GetKeyDown(KeyCode.T))
                {
                    Debug.Log("🔨 T 키 입력 감지됨 - Repair 시도");
                    rubble.Repair();
                    currentRubble = null;
                    //Debug.Log("✅ Repair 완료 및 currentRubble 초기화됨");
                }
            }
            else
            {
                Debug.Log("❓ Raycast 히트함, 하지만 Rubble_RepairHandler 없음");
            }
        }
        else
        {
            if (currentRubble != null)
            {
                //Debug.Log("🚫 Raycast 미감지 - 현재 잔해물 UI 숨김 및 초기화");
                currentRubble.HideUI();
                currentRubble = null;
            }

            Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red); // 감지 실패 시 표시
        }
    }
}
