using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; // Set this in Inspector

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
