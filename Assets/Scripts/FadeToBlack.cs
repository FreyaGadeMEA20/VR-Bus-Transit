using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeToBlack : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Duration of the fade in seconds
    [SerializeField] Image fadeImage;
    
    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOutAndReloadScene()
    {
        StartCoroutine(FadeOut());

        yield return new WaitForSeconds(fadeDuration*2);

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(currentSceneName);
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration+1)
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
        yield return new WaitForSeconds(fadeDuration);
        Debug.Log("Fading in");
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration+1)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            var temp = fadeImage.color;
            temp.a = alpha;
            fadeImage.color = temp;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeDuration = 1.0f;
    }
}
