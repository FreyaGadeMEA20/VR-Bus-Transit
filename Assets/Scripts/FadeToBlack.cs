using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeToBlack : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Duration of the fade in seconds
    [SerializeField] Material fadeImage;
   
    private void Start()
    {
        //fadeImage = GetComponent<Image>();
        //canvasGroup = GetComponent<CanvasGroup>();
        //StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOutAndReloadScene()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            var temp = fadeImage.color;
            temp.a = alpha;
            fadeImage.color = temp;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(currentSceneName);
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            var temp = fadeImage.color;
            temp.a = alpha;
            fadeImage.color = temp;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            var temp = fadeImage.color;
            temp.a = alpha;
            fadeImage.color = temp;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
