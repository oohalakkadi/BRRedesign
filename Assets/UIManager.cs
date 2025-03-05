using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SceneLoader sceneLoader;
    
    [Header("Key Configuration")]
    [SerializeField] private KeyCode sceneLoadKey = KeyCode.C;
    [SerializeField] private bool enableKeyTrigger = true;
    
    private void Update()
    {
        if (enableKeyTrigger && Input.GetKeyDown(sceneLoadKey))
        {
            TriggerSceneLoad();
        }
    }
    
    public void TriggerSceneLoad()
    {
        if (sceneLoader != null)
        {
            sceneLoader.LoadScene();
        }
        else
        {
            Debug.LogError("Scene Loader reference is missing in UIManager");
        }
    }
    
    // Method to enable/disable the key trigger
    public void SetKeyTriggerEnabled(bool isEnabled)
    {
        enableKeyTrigger = isEnabled;
    }
}