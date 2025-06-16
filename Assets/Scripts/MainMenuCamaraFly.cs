using UnityEngine;

public class MainMenuCamaraFly : MonoBehaviour
{
    // Movement speed multiplier
    [SerializeField] private float speed = 0.2f;
    // Range of movement
    [SerializeField] private float movementRange = 20f;
    
    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Z position using a sine wave
        float newZ = startPosition.z + Mathf.Sin(Time.time * speed) * movementRange;
        
        // Calculate the new Y position using a sine wave with a different phase
        float newY = startPosition.y + Mathf.Sin(Time.time * speed * 0.7f) * 7f + 8f; // This will move between 1 and 15
        
        // Update the camera position
        transform.position = new Vector3(transform.position.x, newY, newZ);
    }
}
