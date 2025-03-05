using System.Collections;
using UnityEngine;

public class SinkController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject water;
    [SerializeField] private GameObject faucetTab;
    [SerializeField] private Collider proximityTrigger; // Trigger for proximity detection

    [Header("Faucet Tab Rotations (Euler Angles)")]
    public Vector3 tabOffEuler = new Vector3(0f, 0f, 0f);
    public Vector3 tabOnEuler = new Vector3(-15f, 0f, 0f);

    [Header("Settings")]
    [SerializeField] private bool touchFree = false;
    [SerializeField] private float autoOffTime = 5.0f;
    public float tabAnimationDuration = 0.3f;

    private bool isWaterRunning = false;
    private Coroutine tabAnimation;

    void Start()
    {
        water.SetActive(false);
        faucetTab.transform.rotation = Quaternion.Euler(tabOffEuler);
    }

    void Update()
    {
        if (!touchFree && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == faucetTab)
            {
                ToggleSink();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (touchFree && other.CompareTag("Player") && !isWaterRunning)
        {
            TurnOnWater();
            StartCoroutine(AutoTurnOff());
        }
    }

    void ToggleSink()
    {
        if (isWaterRunning)
        {
            TurnOffWater();
        }
        else
        {
            TurnOnWater();
        }
    }

    void TurnOnWater()
    {
        if (tabAnimation != null)
            StopCoroutine(tabAnimation);

        tabAnimation = StartCoroutine(AnimateRotation(faucetTab, Quaternion.Euler(tabOnEuler), tabAnimationDuration));
        water.SetActive(true);
        isWaterRunning = true;
    }

    void TurnOffWater()
    {
        if (tabAnimation != null)
            StopCoroutine(tabAnimation);

        tabAnimation = StartCoroutine(AnimateRotation(faucetTab, Quaternion.Euler(tabOffEuler), tabAnimationDuration));
        water.SetActive(false);
        isWaterRunning = false;
    }

    IEnumerator AutoTurnOff()
    {
        yield return new WaitForSeconds(autoOffTime);
        TurnOffWater();
    }

    IEnumerator AnimateRotation(GameObject obj, Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = obj.transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            obj.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.rotation = targetRotation;
    }
}
