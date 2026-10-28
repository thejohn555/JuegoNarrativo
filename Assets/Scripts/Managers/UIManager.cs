using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private CanvasGroup transitionCanvasGroup;
    [SerializeField] private float transitionDuration = 1f;

    [Header("Configuración de Escenas")]
    [SerializeField] private string[] levelScenesToFade;

    // 1. SUSCRIPCIÓN AL EVENTO (Crucial para que OnSceneLoaded funcione)
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        transitionCanvasGroup.alpha = 0f;
        transitionCanvasGroup.blocksRaycasts = false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (levelScenesToFade.Contains(scene.name))
        {
            StartCoroutine(FadeInRoutine());
        }
        else
        {
            transitionCanvasGroup.alpha = 0f;
            transitionCanvasGroup.blocksRaycasts = false;
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionAndLoad(sceneName));
    }

    private IEnumerator TransitionAndLoad(string sceneName)
    {
        transitionCanvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(1f));

        SceneManager.LoadSceneAsync(sceneName);
    }

    private IEnumerator FadeInRoutine()
    {
        transitionCanvasGroup.alpha = 1f;
        yield return StartCoroutine(Fade(0f));

        transitionCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = transitionCanvasGroup.alpha;
        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            transitionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / transitionDuration);
            yield return null;
        }

        transitionCanvasGroup.alpha = targetAlpha;
    }
}