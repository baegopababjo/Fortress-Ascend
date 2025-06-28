using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RepairUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Rubble_RepairHandler _repairHandler;
    bool IsNear;

    void Start()
    {
        _repairHandler = GetComponentInParent<Rubble_RepairHandler>();
        //Debug.Log($"🛠️ RepairUI 초기화됨 - 연결된 Rubble: {_repairHandler?.name}");
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.T) && IsNear)
        {
            //Debug.Log("🔨 T 키 입력됨 & IsNear = true → Repair 호출");
            _repairHandler.Repair();
        }
    }

    void Interact(GameObject obj)
    {
        float dist = Vector3.Distance(transform.position, obj.transform.position);

        if (IsNear && dist > 7f)
        {
            IsNear = false;
            Debug.Log($"🚫 플레이어 거리 벗어남 ({dist:F2}m) → IsNear = false");
        }
        else if (!IsNear && dist < 7f)
        {
            IsNear = true;
            Debug.Log($"✅ 플레이어 근접 ({dist:F2}m) → IsNear = true");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("🖱️ 마우스 잔해물 UI에 진입");
        Interact(eventData.selectedObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("👋 마우스 잔해물 UI에서 벗어남");
        Interact(eventData.selectedObject);
    }
}
