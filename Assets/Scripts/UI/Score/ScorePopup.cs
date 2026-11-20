using TMPro;
using UnityEngine;
/// <summary>
/// 적 처치 시 획득한 점수를 화면에 표시
/// </summary>
public class ScorePopup : MonoBehaviour
{
    [Header("Score Text")]
    [SerializeField] private TMP_Text scoreText = null;

    [Header("Popup Settings")]
    [SerializeField] private float displayDuration = 1f;
    [SerializeField] private float moveDistance = 50f;

    private RectTransform rectTransform = null;
    private Vector2 startPosition = Vector2.zero;
    private float elapsedTime = 0f;

    public void SetScore(int InScore, Vector2 InStartPosition)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        startPosition = InStartPosition;
        rectTransform.anchoredPosition = startPosition;
        elapsedTime = 0f;

        if (scoreText != null)
        {
            scoreText.text = "+" + InScore.ToString();
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (scoreText == null)
        {
            scoreText = GetComponentInChildren<TMP_Text>();
        }

        if (rectTransform != null)
        {
            startPosition = rectTransform.anchoredPosition;
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float progress = Mathf.Clamp01(
            elapsedTime / displayDuration
        );

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPosition 
                + Vector2.up * moveDistance * progress;
        }

        if (scoreText != null)
        {
            Color textColor = scoreText.color;
            textColor.a = 1f - progress;
            scoreText.color = textColor; 
        }

        if (elapsedTime >= displayDuration)
        {
            Destroy(gameObject);
        }
    }
}
