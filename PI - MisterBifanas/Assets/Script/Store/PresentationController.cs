using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PresentationController: MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private AspectRatioFitter screenFrame;
    [SerializeField] private CanvasGroup tvWorldCanvasGroup;
    [SerializeField] private RenderTexture tvRenderTexture;
    [SerializeField] private StoreCameraLook storeCameraLook;

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

    private bool isTVSceneLoaded = false;
    private bool isLoadingTVScene = false;

    private void Start()
    {
        Debug.Log("Resolução do ecrã: " + Screen.width + " x " + Screen.height);

        tvWorldCanvasGroup.alpha = 0f;

        SetPresentation(0f);

        tvRenderTexture.Create();

        Camera[] allCameras = Resources.FindObjectsOfTypeAll<Camera>();

        foreach (Camera cam in allCameras)
        {
            if (cam.gameObject.scene.name == "TV_01")
            {
                Debug.Log("Câmara encontrada na TV_01: " + cam.name);
                Debug.Log("Ativa: " + cam.isActiveAndEnabled);
                Debug.Log("Target Texture: " + (cam.targetTexture != null ? cam.targetTexture.name : "NULL"));
            }
        }

        Debug.Log("Cenas carregadas: " + SceneManager.sceneCount);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Debug.Log("Cena " + i + ": " + SceneManager.GetSceneAt(i).name);
        }

        Debug.Log("Cena ativa: " + SceneManager.GetActiveScene().name);
    }

    public IEnumerator LoadTVScene()
    {
        if (isLoadingTVScene)
            yield break;

        Scene tvScene = SceneManager.GetSceneByName("TV_01");

        if (!tvScene.isLoaded)
        {
            isLoadingTVScene = true;

            AsyncOperation operation = SceneManager.LoadSceneAsync(
                "TV_01",
                LoadSceneMode.Additive
            );

            while (!operation.isDone)
            {
                yield return null;
            }

            isLoadingTVScene = false;
        }

        isTVSceneLoaded = true;

        yield return null;

        if (tvRenderTexture != null && !tvRenderTexture.IsCreated())
        {
            tvRenderTexture.Create();
        }
    }

    public IEnumerator UnloadTVScene()
    {
        Scene tvScene = SceneManager.GetSceneByName("TV_01");

        if (tvScene.isLoaded)
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync("TV_01");

            while (operation != null && !operation.isDone)
            {
                yield return null;
            }
        }

        isTVSceneLoaded = false;
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

        float viewportAspect = (float)Screen.width / Screen.height;

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

    public void ShowTVWorld(float duration)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTVWorld(1f, duration));
    }

    public void HideTVWorld(float duration = 0.8f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTVWorld(0f, duration));
    }

    private IEnumerator FadeTVWorld(float targetAlpha, float duration = 0.8f)
    {
        float startAlpha = tvWorldCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            tvWorldCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t
            );

            storeCameraLook.SetStaticStrength(
                Mathf.Lerp(0.3f, 0f, targetAlpha == 1f ? t : 1f - t)
            );

            yield return null;
        }

        tvWorldCanvasGroup.alpha = targetAlpha;
        fadeCoroutine = null;
    }
}
