using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject lockHinge;
    [SerializeField] private GameObject locker;
    [SerializeField] private GameObject doorHinge;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject vacant;
    [SerializeField] private Collider hoverCollider;

    [Header("Door Rotations (Euler Angles)")]
    public Vector3 doorClosedEuler = new Vector3(0f, -90f, 0f);
    public Vector3 doorOpenEuler = new Vector3(0f, -286.319f, 0f);

    [Header("Lock Rotations (Euler Angles)")]
    public Vector3 lockUnlockedEuler = new Vector3(90f, 0f, 0f);
    public Vector3 lockLockedEuler = new Vector3(0f, 0f, 0f);

    [Header("Animation Durations")]
    public float doorAnimationDuration = 1.0f;
    public float lockAnimationDuration = 0.5f;

    [Header("Touch-Free Settings")]
    [SerializeField] private bool touchFree = false;
    [SerializeField] private float cooldownAfterClosing = 2.0f; // Cooldown time in seconds
    [SerializeField] private float gazeTimeRequired = 1.0f; // Time user must gaze at door to open it
    [SerializeField] private float doorAutoCloseTime = 5.0f; // Time before door auto-closes

    private bool locked;
    private bool closed;
    private bool isAnimatingDoor;
    private bool isAnimatingLock;
    private bool isHovered = false;
    private bool inCooldown = false;
    private float currentGazeTime = 0f;
    private float timeSinceOpened = 0f;

    void Start()
    {
        doorHinge.transform.rotation = Quaternion.Euler(doorClosedEuler);
        lockHinge.transform.rotation = Quaternion.Euler(lockUnlockedEuler);
        vacant.SetActive(true);

        locked = false;
        closed = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Camera.main == null || hoverCollider == null)
            return;

        // Toggle touch-free mode when 'T' is pressed
        if (Input.GetKeyDown(KeyCode.T))
        {
            touchFree = !touchFree;
            Debug.Log("Touch-Free Mode: " + (touchFree ? "Enabled" : "Disabled"));
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        bool hit = hoverCollider.Raycast(ray, out hitInfo, Mathf.Infinity);
        
        // Handle touch-free functionality with proper gaze timing
        if (touchFree)
        {
            if (hit && !locked && closed && !inCooldown)
            {
                if (!isHovered)
                {
                    isHovered = true;
                    currentGazeTime = 0f;
                }
                else
                {
                    // Accumulate gaze time
                    currentGazeTime += Time.deltaTime;
                    
                    // Only open door after sufficient gaze time
                    if (currentGazeTime >= gazeTimeRequired)
                    {
                        OpenDoor();
                        currentGazeTime = 0f;
                    }
                }
            }
            else if (!hit)
            {
                isHovered = false;
                currentGazeTime = 0f;
            }
            
            // Auto-close door if open for too long
            if (!closed && !isAnimatingDoor)
            {
                timeSinceOpened += Time.deltaTime;
                if (timeSinceOpened >= doorAutoCloseTime)
                {
                    CloseDoor();
                }
            }
        }

        // Clicking should always allow door to close or toggle lock
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.collider.gameObject == locker && !isAnimatingLock)
                {
                    LockClick();
                }
                else if (hitInfo.collider.gameObject == door && !isAnimatingDoor)
                {
                    CloseDoor();
                }
            }
        }
    }

    void LockClick()
    {
        if (closed && !locked)
        {
            StartCoroutine(AnimateRotation(lockHinge, Quaternion.Euler(lockLockedEuler), lockAnimationDuration));
            vacant.SetActive(false);
            locked = true;
        }
        else if (locked)
        {
            StartCoroutine(AnimateRotation(lockHinge, Quaternion.Euler(lockUnlockedEuler), lockAnimationDuration));
            vacant.SetActive(true);
            locked = false;
        }
    }

    void OpenDoor()
    {
        if (!locked && closed && !isAnimatingDoor)
        {
            StartCoroutine(AnimateRotation(doorHinge, Quaternion.Euler(doorOpenEuler), doorAnimationDuration));
            closed = false;
            timeSinceOpened = 0f;
        }
    }

    void CloseDoor()
    {
        if (!closed && !isAnimatingDoor)
        {
            StartCoroutine(AnimateRotation(doorHinge, Quaternion.Euler(doorClosedEuler), doorAnimationDuration));
            StartCoroutine(EnforceCooldown());
        }
    }

    IEnumerator EnforceCooldown()
    {
        inCooldown = true;
        closed = true;
        isHovered = false; // Reset so gaze won't immediately reopen
        
        yield return new WaitForSeconds(cooldownAfterClosing);
        
        inCooldown = false;
    }

    IEnumerator AnimateRotation(GameObject obj, Quaternion targetRotation, float duration)
    {
        if (obj == doorHinge) isAnimatingDoor = true;
        else if (obj == lockHinge) isAnimatingLock = true;

        Quaternion startRotation = obj.transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            obj.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.rotation = targetRotation;

        if (obj == doorHinge) isAnimatingDoor = false;
        else if (obj == lockHinge) isAnimatingLock = false;
    }
}