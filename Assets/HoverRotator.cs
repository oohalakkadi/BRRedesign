using UnityEngine;
using System.Collections;

public class HoverRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Starting rotation (Euler angles)")]
    [SerializeField] private Vector3 startRotation = new Vector3(0f, 0f, -90f);
    
    [Tooltip("Target rotation when hovered (Euler angles)")]
    [SerializeField] private Vector3 hoverRotation = new Vector3(0f, 0f, 0f);
    
    [Tooltip("How fast the object rotates to target rotation")]
    [SerializeField] private float rotationSpeed = 5.0f;
    
    [Tooltip("Optional: If true, will use direct rotation instead of smooth rotation")]
    [SerializeField] private bool snapToRotation = false;

    private bool isHovered = false;
    private Quaternion currentTargetRotation;
    private Collider objectCollider;

    void Start()
    {
        // Set initial rotation
        transform.rotation = Quaternion.Euler(startRotation);
        currentTargetRotation = Quaternion.Euler(startRotation);
        
        // Get collider (add one if it doesn't exist)
        objectCollider = GetComponent<Collider>();
        if (objectCollider == null)
        {
            Debug.LogWarning("No collider found on " + gameObject.name + ". Adding BoxCollider for hover detection.");
            objectCollider = gameObject.AddComponent<BoxCollider>();
        }
    }

    void Update()
    {
        // Check for mouse hover using raycast
        CheckForHover();
        
        // Handle rotation
        if (snapToRotation)
        {
            transform.rotation = currentTargetRotation;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, currentTargetRotation, Time.deltaTime * rotationSpeed);
        }
    }
    
    private void CheckForHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Check if mouse is hovering over this object
        if (Physics.Raycast(ray, out hit) && hit.collider == objectCollider)
        {
            if (!isHovered)
            {
                isHovered = true;
                currentTargetRotation = Quaternion.Euler(hoverRotation);
            }
        }
        else
        {
            if (isHovered)
            {
                isHovered = false;
                currentTargetRotation = Quaternion.Euler(startRotation);
            }
        }
    }

    // Optional: public methods to manually trigger hover state
    public void SetHovered(bool hover)
    {
        isHovered = hover;
        currentTargetRotation = isHovered ? Quaternion.Euler(hoverRotation) : Quaternion.Euler(startRotation);
    }
    
    // Method to reset to initial rotation
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(startRotation);
        currentTargetRotation = Quaternion.Euler(startRotation);
        isHovered = false;
    }
}