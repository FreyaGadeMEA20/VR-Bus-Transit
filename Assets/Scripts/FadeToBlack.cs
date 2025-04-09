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

    // Initiatlize the singleton instance of the FadeToBlack
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(FadeIn()); // fade in at the start
    }

    // Method to fade out and load a new scene
    public IEnumerator FadeOutAndLoadScene(int scene)
    {        
        StartCoroutine(FadeOut()); // Fades out

        // Waits till it is fully faded out, plus a little, before loading the scene
        yield return new WaitForSeconds(fadeDuration*2);

        // Load the new scene
        SceneManager.LoadSceneAsync(scene);
        isFading = false;
    }

    // Method for fading out in a thread (enumerator)
    public IEnumerator FadeOut()
    {
        // Makes sure the timer starts at 0
        float elapsedTime = 0f;

        // Until it hits the fadeduration, run the while loop.
        // Legit don't ask me why the fade duration is +1, i don't wanna worry about it
        while (elapsedTime < fadeDuration+1)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration); //lerps the number between 0 and 1
            var temp = fadeImage.color; // Takes the color of the image
            temp.a = alpha; // takes the lerped number and assigns it to the alpha value of the color
            fadeImage.color = temp; // Sets the color of the image to the new color
            elapsedTime += Time.deltaTime; // increments the time
            yield return null;
        }

        // Tells the program that the fade is done
        isFading = false;
    }

    // everything as above, however it counts up instead of down
    public IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(fadeDuration); // why is this here, why did i add this. Oh well!

        // Everything as the fade out, just in reverse
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
        fadeDuration = 1.0f; // why is this here, why did i add this. Oh well!

        isFading = false;
    }
}
