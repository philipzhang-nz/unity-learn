using System.Collections;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Moves and rolls a GameObject when the 'R' key is pressed.
/// Supports both simple rotation around the center and realistic edge-pivoted rolling.
/// </summary>
public class MoveAndRollByEnteringR : MonoBehaviour
{
    public enum RollMode
    {
        [Tooltip("Translates and rotates around center simultaneously (can look like sliding).")]
        RotateAroundCenter,
        
        [Tooltip("Realistic cube roll that pivots around its bottom edges.")]
        PivotOnEdges
    }

    [Header("General Settings")]
    [SerializeField] private RollMode rollMode = RollMode.PivotOnEdges;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;
    [SerializeField] private KeyCode triggerKey = KeyCode.R;
    [Tooltip("If true, pressing R will roll exactly once. You must release and press it again to roll again. If false, holding R will roll continuously.")]
    [SerializeField] private bool requireNewPressForEachRoll = true;

    [Header("Rotate Around Center Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rollSpeed = 360f;
    [SerializeField] private Vector3 rotationAxis = Vector3.right;
    [SerializeField] private Space moveSpace = Space.World;
    [SerializeField] private Space rollSpace = Space.World;

    [Header("Pivot On Edges Settings")]
    [Tooltip("Time in seconds to complete a single 90-degree roll.")]
    [SerializeField] private float rollDuration = 0.3f;
    [Tooltip("The side length of the cube (typically 1.0 for standard Unity cubes).")]
    [SerializeField] private float cubeSize = 1f;

    [Header("Animator Handling")]
    [Tooltip("If true, the script will automatically disable the Animator component on Start to prevent it from overriding the movement/rotation.")]
    [SerializeField] private bool disableAnimatorOnStart = true;

    private bool isRolling = false;
    private Animator animator;
    private Rigidbody rb;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (disableAnimatorOnStart && animator != null)
        {
            animator.enabled = false;
            Debug.Log("MoveAndRollByEnteringR: Automatically disabled the Animator component on Start to allow manual rotation/rolling. You can disable this setting in the Inspector if needed.");
        }
    }

    private void Update()
    {
        bool isKeyPressed = false;

        if (requireNewPressForEachRoll)
        {
            // Check legacy Input System for key down
#if ENABLE_LEGACY_INPUT_MANAGER || !ENABLE_INPUT_SYSTEM
            if (Input.GetKeyDown(triggerKey))
            {
                isKeyPressed = true;
            }
#endif

            // Check new Input System for key down (wasPressedThisFrame)
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                Key newSystemKey = MapKeyCodeToKey(triggerKey);
                if (newSystemKey != Key.None && Keyboard.current[newSystemKey].wasPressedThisFrame)
                {
                    isKeyPressed = true;
                }
            }
#endif
        }
        else
        {
            // Check legacy Input System for key hold
#if ENABLE_LEGACY_INPUT_MANAGER || !ENABLE_INPUT_SYSTEM
            if (Input.GetKey(triggerKey))
            {
                isKeyPressed = true;
            }
#endif

            // Check new Input System for key hold (isPressed)
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                Key newSystemKey = MapKeyCodeToKey(triggerKey);
                if (newSystemKey != Key.None && Keyboard.current[newSystemKey].isPressed)
                {
                    isKeyPressed = true;
                }
            }
#endif
        }

        if (isKeyPressed)
        {
            if (rollMode == RollMode.RotateAroundCenter)
            {
                PerformSimpleMoveAndRoll();
            }
            else if (rollMode == RollMode.PivotOnEdges && !isRolling)
            {
                StartCoroutine(RollOnEdgeRoutine(moveDirection.normalized));
            }
        }
    }

    private void PerformSimpleMoveAndRoll()
    {
        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, moveSpace);
        transform.Rotate(rotationAxis.normalized, rollSpeed * Time.deltaTime, rollSpace);
    }

    private IEnumerator RollOnEdgeRoutine(Vector3 direction)
    {
        isRolling = true;

        // Disable Rigidbody physics temporarily to prevent physics simulation from overriding manual movement/rotation
        bool wasKinematic = true;
        if (rb != null)
        {
            wasKinematic = rb.isKinematic;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Calculate the actual size of the cube in world space (assuming uniform scale)
        float averageScale = (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;
        float half = cubeSize * averageScale * 0.5f;

        // The pivot is at the bottom (world down) and in the direction of movement (world direction)
        // This prevents the pivot from tumbling when the local axes rotate
        Vector3 worldDown = Vector3.down * half;
        Vector3 worldForward = direction * half;
        Vector3 pivotPoint = transform.position + worldDown + worldForward;

        // The rotation axis is perpendicular to world up and the direction we are moving
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction).normalized;

        float elapsed = 0f;
        float totalAngle = 0f;

        while (elapsed < rollDuration)
        {
            elapsed += Time.deltaTime;
            
            // Calculate interpolation
            float percent = Mathf.Clamp01(elapsed / rollDuration);
            float targetAngle = percent * 90f;
            float angleThisFrame = targetAngle - totalAngle;
            totalAngle = targetAngle;

            transform.RotateAround(pivotPoint, rotationAxis, angleThisFrame);
            yield return null;
        }

        // Ensure we end up at exactly 90 degrees of rotation
        float remainingAngle = 90f - totalAngle;
        if (remainingAngle > 0f)
        {
            transform.RotateAround(pivotPoint, rotationAxis, remainingAngle);
        }

        // Restore Rigidbody physics settings
        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
        }

        isRolling = false;
    }

#if ENABLE_INPUT_SYSTEM
    private Key MapKeyCodeToKey(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.R: return Key.R;
            case KeyCode.A: return Key.A;
            case KeyCode.B: return Key.B;
            case KeyCode.C: return Key.C;
            case KeyCode.D: return Key.D;
            case KeyCode.E: return Key.E;
            case KeyCode.F: return Key.F;
            case KeyCode.G: return Key.G;
            case KeyCode.H: return Key.H;
            case KeyCode.I: return Key.I;
            case KeyCode.J: return Key.J;
            case KeyCode.K: return Key.K;
            case KeyCode.L: return Key.L;
            case KeyCode.M: return Key.M;
            case KeyCode.N: return Key.N;
            case KeyCode.O: return Key.O;
            case KeyCode.P: return Key.P;
            case KeyCode.Q: return Key.Q;
            case KeyCode.S: return Key.S;
            case KeyCode.T: return Key.T;
            case KeyCode.U: return Key.U;
            case KeyCode.V: return Key.V;
            case KeyCode.W: return Key.W;
            case KeyCode.X: return Key.X;
            case KeyCode.Y: return Key.Y;
            case KeyCode.Z: return Key.Z;
            case KeyCode.Space: return Key.Space;
            default: return Key.None;
        }
    }
#endif
}
