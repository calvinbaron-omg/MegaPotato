using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // Drag Player here
    public Vector3 offset = new Vector3(0, 10, -10);
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;

        transform.LookAt(target);
    }
}
