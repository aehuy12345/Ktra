using UnityEngine;
using UnityEngine.InputSystem;

// Điều khiển nhân vật bằng W, A, S, D. CharacterController giúp nhân vật không đi xuyên vật cản.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform cameraTransform;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Thắng hoặc thua thì không nhận phím nữa, nhân vật dừng lại.
        bool canMove = GameManager.Instance == null || !GameManager.Instance.IsFinished;
        Vector3 direction = canMove ? ToWorldDirection(ReadInput()) : Vector3.zero;

        // Tăng/giảm tốc dần để chuyển động mượt.
        velocity = Vector3.MoveTowards(velocity, direction * moveSpeed, acceleration * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        controller.Move((velocity + Vector3.up * verticalVelocity) * Time.deltaTime);

        if (direction.sqrMagnitude > 0.001f)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        if (animator != null)
            animator.SetFloat(SpeedHash, velocity.magnitude / moveSpeed, 0.1f, Time.deltaTime);
    }

    private static Vector2 ReadInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        float x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
        float y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
        return Vector2.ClampMagnitude(new Vector2(x, y), 1f);
    }

    // Đổi hướng phím bấm sang hướng trong thế giới theo góc nhìn camera (W = đi ra xa camera).
    private Vector3 ToWorldDirection(Vector2 input)
    {
        if (cameraTransform == null)
            return new Vector3(input.x, 0f, input.y);

        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        return forward * input.y + right * input.x;
    }
}
