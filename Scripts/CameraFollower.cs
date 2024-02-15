using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform playerTransform;
    public Vector3 offset;

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    void LateUpdate()
    {
        Vector3 newPosition = playerTransform.position + offset;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}
