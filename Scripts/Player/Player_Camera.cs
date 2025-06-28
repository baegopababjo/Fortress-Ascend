using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    public Transform Player;  // 플레이어
    public Vector3 Offset = new Vector3(0, 2, -4); // 카메라 거리
    public float Rotate_Speed = 3.0f;  // 회전 속도
    private bool isAttacking = false; // 🛑 공격 중인지 체크
    private bool canRotate = true; // 회전 허용 여부 (기본값 true)

    float mx = 0;  // 좌우 회전 값 (플레이어 Y축 회전)
    float my = 0;  // 상하 회전 값 (카메라 X축 회전)


    void LateUpdate()
    {
        if (Player == null || !canRotate) return;   // Player가 할당되지 않았다면 실행하지 않음

        // 🔒 마법 선택 중이면 회전 차단
        if (CharacterSelection.Instance != null && !CharacterSelection.Instance.isMagicSelected)
            return;

        // 🔥 상점이 열려 있거나 환경설정 창 열었다면 공격 입력을 차단
        if (ShopUI.IsShopOpen || (SettingsMenuManager.Instance != null && SettingsMenuManager.Instance.IsMenuOpen())) return;

        // 🛑 공격 중이면 카메라 회전 금지
        if (isAttacking)    return;

        // 마우스 입력 받기
        float mouse_x = Input.GetAxis("Mouse X") * Rotate_Speed;
        float mouse_y = Input.GetAxis("Mouse Y") * Rotate_Speed;

        mx += mouse_x;  // 플레이어 좌우 회전
        my -= mouse_y;  // 카메라 상하 회전 (반전)

        // 상하 회전 제한 (-30도 ~ 60도)
        my = Mathf.Clamp(my, -30f, 60f);

        // 플레이어 Y축 회전 적용 (좌우)
        Player.rotation = Quaternion.Euler(0, mx, 0);

        // 카메라 위치 설정
        Quaternion camRotation = Quaternion.Euler(my, mx, 0);
        transform.position = Player.position + camRotation * Offset;

        // 플레이어를 자연스럽게 바라보도록 설정
        transform.LookAt(Player.position + Vector3.up * 1.5f);
    }

    // 🏹 공격 상태 변경 함수 추가
    public void SetAttackState(bool attacking)
    {
        isAttacking = attacking;
    }

    //회전 차단 함수
    public void SetRotationEnabled(bool enable)
    {
        canRotate = enable;
    }
}
