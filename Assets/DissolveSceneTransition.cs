using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DissolveSceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private Material dissolveMaterial;  // Assign the Dissolve Shader Material
    [SerializeField] private RawImage transitionImage;   // UI Overlay for Transition
    [SerializeField] private float transitionDuration = 2.0f;
    [SerializeField] private string nextSceneName;

    private bool isTransitioning = false;

    void Start()
    {
        transitionImage.gameObject.SetActive(false);
    }

    public void StartSceneTransition()
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionToScene());
        }
    }

    private IEnumerator TransitionToScene()
    {
        isTransitioning = true;
        transitionImage.gameObject.SetActive(true);
        transitionImage.material = dissolveMaterial;

        // Capture Screenshot of Current Scene
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();
        transitionImage.texture = screenTexture;

        // Fade in Dissolve Effect
        float elapsed = 0;
        while (elapsed < transitionDuration)
        {
            float dissolveAmount = elapsed / transitionDuration;
            dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Load Next Scene
        yield return SceneManager.LoadSceneAsync(nextSceneName);

        // Capture New Scene and Fade Out
        yield return new WaitForSeconds(0.5f);
        elapsed = transitionDuration;
        while (elapsed > 0)
        {
            float dissolveAmount = elapsed / transitionDuration;
            dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            elapsed -= Time.deltaTime;
            yield return null;
        }

        transitionImage.gameObject.SetActive(false);
        isTransitioning = false;
    }
}
