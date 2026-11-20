using UnityEngine;
/// <summary>
/// 적의 사망 여부를 확인 및 점수
/// </summary>
public class EnemyScoreHandler : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private MechHull2D mechHull = null;

    [Header("Score")]
    [SerializeField] private int killScore = 100;
    [SerializeField] private ScorePopup scorePopupPrefab = null;
    [SerializeField] private Canvas targetCanvas = null;
    [SerializeField] private Vector3 popupOffset = new Vector3(0f, 1f, 0f);

    private bool isScoreAdded = false;

    private void Awake()
    {
        if (mechHull == null)
        {
            mechHull = GetComponent<MechHull2D>();
        }

        if (targetCanvas == null)
        {
            GameObject canvasObject = GameObject.Find("ScoreCanvas");

            if (canvasObject != null)
            {
                targetCanvas = canvasObject.GetComponent<Canvas>();
            }
        }
    }

    private void Update()
    {
        if (mechHull == null || !mechHull.IsDead || isScoreAdded)
        {
            return;
        }

        isScoreAdded = true;

        ScoreManager.AddScore(killScore);
        ShowScorePopup();
    }

    private void ShowScorePopup()
    {
        if (scorePopupPrefab == null || targetCanvas == null)
        {
            return;
        }

        Vector3 worldPosition = transform.position + popupOffset;

        Vector2 screenPosition =
            RectTransformUtility.WorldToScreenPoint(
                Camera.main,
                worldPosition
            );

        RectTransform canvasRect =
            targetCanvas.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            null,
            out Vector2 localPosition
        );

        ScorePopup popup = Instantiate(
            scorePopupPrefab,
            targetCanvas.transform
        );

        popup.SetScore(killScore, localPosition);
    }
}
