using UnityEngine;

// Kẻ địch đi tuần qua lại giữa hai điểm A và B. Nhân vật chạm vào thì Game Over.
// Rigidbody có trọng lực + CapsuleCollider đặc để kẻ địch đứng trên mặt đất, không rơi xuyên xuống;
// một CapsuleCollider Trigger lớn hơn một chút dùng để phát hiện va chạm với nhân vật.
[RequireComponent(typeof(Rigidbody))]
public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float turnSpeed = 360f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float arriveDistance = 0.2f;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private Rigidbody body;
    private Animator animator;
    private Transform target;
    private float waitTimer;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        target = pointB;
    }

    private void FixedUpdate()
    {
        bool stopped = pointA == null || pointB == null ||
                       (GameManager.Instance != null && GameManager.Instance.IsFinished);

        Vector3 move = Vector3.zero;
        if (!stopped)
        {
            if (waitTimer > 0f)
            {
                waitTimer -= Time.fixedDeltaTime;
            }
            else
            {
                Vector3 toTarget = Vector3.ProjectOnPlane(target.position - body.position, Vector3.up);
                if (toTarget.magnitude <= arriveDistance)
                {
                    // Tới nơi: nghỉ một chút rồi quay đầu đi về điểm còn lại.
                    target = target == pointA ? pointB : pointA;
                    waitTimer = waitTime;
                }
                else
                {
                    move = toTarget.normalized * moveSpeed;
                    var targetRotation = Quaternion.LookRotation(toTarget);
                    body.rotation = Quaternion.RotateTowards(body.rotation, targetRotation,
                        turnSpeed * Time.fixedDeltaTime);
                }
            }
        }

        // Chỉ điều khiển vận tốc ngang, giữ vận tốc rơi để trọng lực + collider giữ kẻ địch trên mặt đất.
        body.linearVelocity = new Vector3(move.x, body.linearVelocity.y, move.z);

        if (animator != null)
            animator.SetFloat(SpeedHash, move.magnitude / moveSpeed, 0.1f, Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance != null)
            GameManager.Instance.TriggerGameOver();
    }

    private void OnDrawGizmos()
    {
        if (pointA == null || pointB == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawWireSphere(pointA.position, 0.3f);
        Gizmos.DrawWireSphere(pointB.position, 0.3f);
    }
}
