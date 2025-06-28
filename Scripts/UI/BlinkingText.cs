using UnityEngine;
using TMPro;
using System.Collections;

public class BlinkingText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // 텍스트 오브젝트
    public float fadeDuration = 1.5f; // 페이드 지속 시간
    private Coroutine blinkCoroutine;

    private void OnEnable()
    {
        if (textMeshPro != null)
        {
            Debug.Log("🔵 OnEnable 호출됨");

            // 이전 깜빡이기 코루틴 정리
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
            }

            // 새로 코루틴 시작
            blinkCoroutine = StartCoroutine(BlinkTextEffect());
        }
    }

    private void OnDisable()
    {
        if (blinkCoroutine != null)
        {
            Debug.Log("🔴 OnDisable 호출됨 - 코루틴 정지");
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }

    IEnumerator BlinkTextEffect()
    {
        while (true)
        {
            // 점점 밝아짐
            yield return StartCoroutine(FadeText(0f, 1f, fadeDuration));
            // 점점 어두워짐
            yield return StartCoroutine(FadeText(1f, 0.1f, fadeDuration));
        }
    }

    IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);

            Color textColor = textMeshPro.color; // 매 프레임마다 새로 읽기
            textColor.a = newAlpha;
            textMeshPro.color = textColor;

            yield return null;
        }

        Color finalColor = textMeshPro.color;
        finalColor.a = endAlpha;
        textMeshPro.color = finalColor;
    }
}
