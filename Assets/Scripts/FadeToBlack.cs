using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeToBlack : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Duration of the fade in seconds
    [SerializeField] Image fadeImage;

    public static FadeToBlack Instance { get; internal set; }
    public bool isFading = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOutAndLoadScene(int scene)
    {
        if (isFading)
            yield break;
        isFading = true;
        
        StartCoroutine(FadeOut());

        yield return new WaitForSeconds(fadeDuration*2);

        //string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Scene loaded");
        SceneManager.LoadSceneAsync(scene);
    }

    public IEnumerator FadeOut()
    {
        if (isFading)
            yield break;
        isFading = true;

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
        if (isFading)
            yield break;
        isFading = true;

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
