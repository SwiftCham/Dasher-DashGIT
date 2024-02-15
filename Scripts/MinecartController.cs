using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinecartController : MonoBehaviour
{
    [SerializeField] private InputActionReference movementReference;
    [SerializeField] private float switchDuration = 1.0f;
    [SerializeField] private float liftHeight = 0.5f;
    [SerializeField] private Transform[] tracks;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float maxSpeed = 10f;

    private int currentTrackIndex = 0;
    private bool isSwitching = false;
    private float currentSpeed = 0f;
    private Vector3 targetPosition;
    
    private void OnEnable()
    {
        movementReference.action.Enable();
    }

    private void OnDisable()
    {
        movementReference.action.Disable();
    }

    private void Update()
    {
        Vector2 input = movementReference.action.ReadValue<Vector2>();
        
        if (!isSwitching)
        {
            // Left and right input for track switching
            if (input.x > 0)
            {
                StartSwitch(1);
            }
            else if (input.x < 0)
            {
                StartSwitch(-1);
            }

            // Forward movement with acceleration
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
        }
        else
        {
            // Halve the speed when switching
            currentSpeed *= 0.5f;
        }

        // Apply the forward movement
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void StartSwitch(int direction)
    {
        if ((direction == -1 && currentTrackIndex == 0) || (direction == 1 && currentTrackIndex == tracks.Length - 1))
            return;

        if (!isSwitching)
        {
            StartCoroutine(SwitchTrack(currentTrackIndex + direction));
        }
    }

    private IEnumerator SwitchTrack(int targetTrackIndex)
    {
        isSwitching = true;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = tracks[targetTrackIndex].position;
        Vector3 liftPosition = new Vector3(startPosition.x, startPosition.y + liftHeight, startPosition.z);

        float elapsedTime = 0;

        // Lift the minecart
        while (elapsedTime < switchDuration / 2)
        {
            transform.position = Vector3.Lerp(startPosition, liftPosition, (elapsedTime / (switchDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move to the adjacent track
        startPosition = transform.position;
        elapsedTime = 0;
        while (elapsedTime < switchDuration / 2)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / (switchDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Final position adjustment
        transform.position = endPosition;

        currentTrackIndex = targetTrackIndex;
        isSwitching = false;
    }
}
