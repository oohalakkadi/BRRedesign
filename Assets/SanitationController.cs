using System.Collections;
using UnityEngine;

public class SanitationController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Collider used to detect when the cursor is hovering over the box.")]
    [SerializeField] private Collider hoverCollider;
    
    [Tooltip("The hinged lid GameObject that should open and close.")]
    [SerializeField] private GameObject lid;

    [Header("Lid Rotations (Euler Angles)")]
    [Tooltip("The rotation of the lid when it is closed.")]
    public Vector3 lidClosedEuler = new Vector3(0f, 0f, 0f);
    
    [Tooltip("The rotation of the lid when it is open.")]
    public Vector3 lidOpenEuler = new Vector3(60f, 0f, 0f);

    [Header("Animation Settings")]
    [Tooltip("The duration (in seconds) of the lid open/close animation.")]
    public float lidAnimationDuration = 0.5f;

    // Private variables for tracking hover state and animation
    private Coroutine currentAnimation;
    private bool isHovered = false;

    void Start()
    {
        // Ensure the lid starts in the closed rotation.
        if (lid != null)
        {
            lid.transform.rotation = Quaternion.Euler(lidClosedEuler);
        }
    }

    void Update()
    {
        if (Camera.main == null || hoverCollider == null)
            return;

        // Create a ray from the camera through the current mouse position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // Check if the ray intersects with our designated hover collider.
        bool hit = hoverCollider.Raycast(ray, out hitInfo, Mathf.Infinity);

        // If the mouse is over the collider and wasn't before, open the lid.
        if (hit && !isHovered)
        {
            isHovered = true;
            OpenLid();
        }
        // If the mouse is no longer over the collider and it was previously hovering, close the lid.
        else if (!hit && isHovered)
        {
            isHovered = false;
            CloseLid();
        }
    }

    /// <summary>
    /// Initiates the animation to open the lid.
    /// </summary>
    private void OpenLid()
    {
        if (lid != null)
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }
            currentAnimation = StartCoroutine(AnimateRotation(lid, Quaternion.Euler(lidOpenEuler), lidAnimationDuration));
        }
    }

    /// <summary>
    /// Initiates the animation to close the lid.
    /// </summary>
    private void CloseLid()
    {
        if (lid != null)
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }
            currentAnimation = StartCoroutine(AnimateRotation(lid, Quaternion.Euler(lidClosedEuler), lidAnimationDuration));
        }
    }

    /// <summary>
    /// Smoothly animates the rotation of the specified GameObject from its current rotation to a target rotation over a given duration.
    /// </summary>
    /// <param name="obj">The GameObject to animate.</param>
    /// <param name="targetRotation">The target rotation (as a Quaternion).</param>
    /// <param name="duration">The duration of the animation in seconds.</param>
    /// <returns>An IEnumerator for coroutine execution.</returns>
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
        currentAnimation = null;
    }
}
