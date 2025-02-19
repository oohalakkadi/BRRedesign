using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject lockHinge;  // The object that rotates to show lock status
    [SerializeField] GameObject locker;     // The clickable lock element
    [SerializeField] GameObject doorHinge;    // The pivot for the door rotation
    [SerializeField] GameObject door;         // The clickable door element
    [SerializeField] GameObject vacant;       // A sign that is active when the door is unlocked

    [Header("Door Rotations (Euler Angles)")]
    [Tooltip("Rotation of the door hinge when closed.")]
    public Vector3 doorClosedEuler = new Vector3(0f, -90f, 0f);
    [Tooltip("Rotation of the door hinge when open.")]
    public Vector3 doorOpenEuler = new Vector3(0f, -286.319f, 0f);
    
    [Header("Lock Rotations (Euler Angles)")]
    [Tooltip("Rotation of the lock hinge when unlocked.")]
    public Vector3 lockUnlockedEuler = new Vector3(90f, 0f, 0f);
    [Tooltip("Rotation of the lock hinge when locked.")]
    public Vector3 lockLockedEuler = new Vector3(0f, 0f, 0f);

    [Header("Animation Durations")]
    [Tooltip("Duration of the door open/close animation (in seconds).")]
    public float doorAnimationDuration = 1.0f;
    [Tooltip("Duration of the lock/unlock animation (in seconds).")]
    public float lockAnimationDuration = 0.5f;

    private bool locked;
    private bool closed;
    private bool isAnimatingDoor;
    private bool isAnimatingLock;

    void Start()
    {
        // Set the door hinge and lock hinge to their initial (closed/unlocked) rotations.
        doorHinge.transform.rotation = Quaternion.Euler(doorClosedEuler);
        lockHinge.transform.rotation = Quaternion.Euler(lockUnlockedEuler);
        vacant.SetActive(true);

        locked = false;
        closed = true;

        // Keep the mouse visible during gameplay.
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        // On left mouse click, cast a ray to see if a clickable element was hit.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // If the click hit the locker, run the lock click behavior.
                if (hit.collider.gameObject == locker)
                {
                    if (!isAnimatingLock)
                        lockClick();
                }
                // If the click hit the door, run the door click behavior.
                else if (hit.collider.gameObject == door)
                {
                    if (!isAnimatingDoor)
                        doorClick();
                }
            }
        }
    }

    void lockClick()
    {
        // If the door is closed and not locked, then lock it.
        if (closed && !locked)
        {
            StartCoroutine(AnimateRotation(lockHinge, Quaternion.Euler(lockLockedEuler), lockAnimationDuration));
            vacant.SetActive(false);
            locked = true;
        }
        // If the door is locked, unlock it.
        else if (locked)
        {
            StartCoroutine(AnimateRotation(lockHinge, Quaternion.Euler(lockUnlockedEuler), lockAnimationDuration));
            vacant.SetActive(true);
            locked = false;
        }
    }

    void doorClick()
    {
        // Only allow the door to open if it’s not locked.
        if (!locked && closed)
        {
            StartCoroutine(AnimateRotation(doorHinge, Quaternion.Euler(doorOpenEuler), doorAnimationDuration));
            closed = false;
        }
        // If the door is open, close it.
        else if (!closed)
        {
            StartCoroutine(AnimateRotation(doorHinge, Quaternion.Euler(doorClosedEuler), doorAnimationDuration));
            closed = true;
        }
    }

    // A general coroutine to animate an object's rotation from its current rotation to a target rotation.
    IEnumerator AnimateRotation(GameObject obj, Quaternion targetRotation, float duration)
    {
        // Determine which object is being animated.
        if (obj == doorHinge)
            isAnimatingDoor = true;
        else if (obj == lockHinge)
            isAnimatingLock = true;

        Quaternion startRotation = obj.transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            obj.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.rotation = targetRotation;

        // Animation is complete.
        if (obj == doorHinge)
            isAnimatingDoor = false;
        else if (obj == lockHinge)
            isAnimatingLock = false;
    }
}
