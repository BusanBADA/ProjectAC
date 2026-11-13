using System.Collections;
using TMPro;
using UnityEngine;

public class MechStatusUIController : MonoBehaviour
{
    [Header("Mech Status Text")]
    [SerializeField] private TMP_Text speedText = null;
    [SerializeField] private TMP_Text turnText = null;
    [SerializeField] private TMP_Text weightText = null;
    [SerializeField] private TMP_Text accelText = null;
    [SerializeField] private TMP_Text fcsText = null;
   

    [Header("Panel Animation")]
    [SerializeField] private RectTransform mechInfoRect = null;
    [SerializeField] private CanvasGroup canvasGroup = null;
    [SerializeField] private Vector2 openPosition = new Vector2(20f, 20f);
    [SerializeField] private Vector2 closedPosition = new Vector2(-300f, -170f);
    [SerializeField] private float animationDuration = 0.5f;

    [Header("Panel State")]
    [SerializeField] private bool isOpen = false;

    private Coroutine animationCoroutine = null;

    public void SetMechStatus(
        float inSpeed,
        float inTurnSpeed,
        float inCurrentWeight,
        float inMaxWeight,
        float inAccleration)
    {
        if (speedText != null)
        {
            speedText.text = "SPD : " + inSpeed.ToString("0.0");
        }

        if (turnText != null)
        {
            turnText.text = "TURN : -";
        }

        if (weightText != null)
        {
            weightText.text = "WEIGHT : "
                + inCurrentWeight.ToString("0.0")
                + " / "
                + inMaxWeight.ToString("0.0");
        }

        if (fcsText != null)
        {
            fcsText.text = "FCS : -";
        }

        if (accelText != null)
        {
            accelText.text = "ACCEL : " + inAccleration.ToString("0.0");
        }
    }

    private void UpdateMechStatus()
    {
        if (PlayerInfo.Instance == null)
        {
            return;
        }

        SetMechStatus(
            PlayerInfo.Instance.maxSpeed,
            0f,
            PlayerInfo.Instance.curWeight,
            PlayerInfo.Instance.limitWeight,
            PlayerInfo.Instance.axel
            );
    }

    private void Awake()
    {
        if (mechInfoRect == null)
        {
            mechInfoRect = GetComponent<RectTransform>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (mechInfoRect != null)
        {
            mechInfoRect.anchoredPosition = closedPosition;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void Update()
    {
        UpdateMechStatus();

        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleMechInfo();
        }
    }

    private void ToggleMechInfo()
    {
        if (mechInfoRect == null || canvasGroup == null)
        {
            return;
        }

        isOpen = !isOpen;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(AnimateMechInfo());
    }

    private IEnumerator AnimateMechInfo()
    {
        Vector2 currentPosition = mechInfoRect.anchoredPosition;
        float currentAlpha = canvasGroup.alpha;

        float middleAlpha = isOpen
            ? Mathf.Lerp(currentAlpha, 1f, 0.5f)
            : Mathf.Lerp(currentAlpha, 0f, 0.5f);

        if (isOpen)
        {
            Vector2 verticalPosition = new Vector2(
                closedPosition.x,
                openPosition.y
            );

            yield return AnimateSegment(
                currentPosition,
                verticalPosition,
                currentAlpha,
                middleAlpha
            );

            yield return AnimateSegment(
                verticalPosition,
                openPosition,
                middleAlpha,
                1f
            );
        }
        else
        {
            Vector2 verticalPosition = new Vector2(
                openPosition.x,
                closedPosition.y
            );

            yield return AnimateSegment(
                currentPosition,
                verticalPosition,
                currentAlpha,
                middleAlpha
            );

            yield return AnimateSegment(
                verticalPosition,
                closedPosition,
                middleAlpha,
                0f
            );
        }

        animationCoroutine = null;
    }

    private IEnumerator AnimateSegment(
        Vector2 inStartPosition,
        Vector2 inTargetPosition,
        float inStartAlpha,
        float inTargetAlpha)
    {
        float elapsedTime = 0f;
        float segmentDuration = animationDuration / 2f;

        while (elapsedTime < segmentDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / segmentDuration
            );

            float easedProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            mechInfoRect.anchoredPosition = Vector2.Lerp(
                inStartPosition,
                inTargetPosition,
                easedProgress
            );

            canvasGroup.alpha = Mathf.Lerp(
                inStartAlpha,
                inTargetAlpha,
                easedProgress
            );

            yield return null;
        }

        mechInfoRect.anchoredPosition = inTargetPosition;
        canvasGroup.alpha = inTargetAlpha;
    }
}
