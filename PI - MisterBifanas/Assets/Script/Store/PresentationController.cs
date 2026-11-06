using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PresentationController: MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private AspectRatioFitter screenFrame;
    
    [SerializeField] private CanvasGroup tvWorldCanvasGroup;

    [Header("Side Bars")]
    [SerializeField] private RectTransform leftBar;
    [SerializeField] private RectTransform rightBar;
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;
    
    [Header("Aspect Ratios")]
    [SerializeField] private float storeAspectRatio = 1f;
    [SerializeField] private float tvAspectRatio = 1.63664f;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 1.2f;

    private Coroutine transitionCoroutine;
    
    private Coroutine fadeCoroutine;
    
    private void Start()
    {
        SetPresentation(0f);
    }

    public void EnterTV()
    {
        StartPresentationTransition(1f);
    }

    public void ExitTV()
    {
        StartPresentationTransition(0f);
    }

    private void StartPresentationTransition(float target)
    {
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(AnimatePresentation(target));
    }

    private IEnumerator AnimatePresentation(float target)
    {
        float start = GetCurrentProgress();
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(
                0f, 1f, elapsed / transitionDuration
            );

            SetPresentation(Mathf.Lerp(start, target, t));

            yield return null;
        }

        SetPresentation(target);
        transitionCoroutine = null;
    }

    private float GetCurrentProgress()
    {
        return Mathf.InverseLerp(
            storeAspectRatio,
            tvAspectRatio,
            screenFrame.aspectRatio
        );
    }

    private void SetPresentation(float progress)
    {
        // Alteração da proporção da apresentação
        screenFrame.aspectRatio = Mathf.Lerp(
            storeAspectRatio,
            tvAspectRatio,
            progress
        );

        // Primeira fase: desaparecimento das barras laterais
        float sideProgress = 1f - Mathf.Clamp01(progress / 0.5f);

        leftBar.anchorMin = new Vector2(0f, 0f);
        leftBar.anchorMax = new Vector2(
            0.125f * sideProgress, 1f
        );

        rightBar.anchorMin = new Vector2(
            1f - (0.125f * sideProgress), 0f
        );
        rightBar.anchorMax = new Vector2(1f, 1f);

        // Segunda fase: aparecimento das barras horizontais
        float horizontalProgress = Mathf.Clamp01(
            (progress - 0.5f) / 0.5f
        );

        float viewportAspect = 4f / 3f;

        float barFraction = (
            1f - (viewportAspect / tvAspectRatio)
        ) * 0.5f;

        topBar.anchorMin = new Vector2(
            0f, 1f - (barFraction * horizontalProgress)
        );
        topBar.anchorMax = new Vector2(1f, 1f);

        bottomBar.anchorMin = new Vector2(0f, 0f);
        bottomBar.anchorMax = new Vector2(
            1f, barFraction * horizontalProgress
        );
    }
    
    public void ShowTVWorld()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTVWorld(1f));
    }

    public void HideTVWorld()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTVWorld(0f));
    }

    private IEnumerator FadeTVWorld(float targetAlpha)
    {
        float startAlpha = tvWorldCanvasGroup.alpha;
        float elapsed = 0f;
        float duration = 0.8f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            tvWorldCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t
            );

            yield return null;
        }

        tvWorldCanvasGroup.alpha = targetAlpha;
        fadeCoroutine = null;
    }
    
}
