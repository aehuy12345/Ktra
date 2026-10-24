using UnityEngine;

// Camera góc nhìn thứ ba bám theo nhân vật với độ lệch cố định.
public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 6f, -8f);
    [SerializeField] private float lookHeight = 1.5f;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private LayerMask obstacleMask = ~0;

    private Vector3 currentVelocity;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 focus = target.position + Vector3.up * lookHeight;
        Vector3 desired = target.position + offset;

        // Kéo camera lại gần nếu có vật cản che giữa camera và nhân vật.
        Vector3 toCamera = desired - focus;
        if (Physics.SphereCast(focus, collisionRadius, toCamera.normalized, out var hit, toCamera.magnitude,
                obstacleMask, QueryTriggerInteraction.Ignore))
            desired = focus + toCamera.normalized * hit.distance;

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref currentVelocity, smoothTime);
        transform.LookAt(focus);
    }
}
