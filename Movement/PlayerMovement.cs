using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _RB;

    [SerializeField]
    private Camera Cam = default;
    [SerializeField]
    private LayerMask _AimLayer = default;

    [SerializeField]
    private float _Velocity = 12f;
    [SerializeField]
    private float _Acceleration = 2f;

    private float _SpeedMult = 1f;
    [SerializeField]
    private float _DiagonalSpeedMult = 0.88f;
    [SerializeField]
    private float _BackwardsSpeedMult = 0.66f;

    [SerializeField]
    private bool UsingController = false;
    [SerializeField]
    private float _RadialDeadzone = 0.25f;

    private Vector2 _MoveDirectionInput;
    private Vector2 _ControllerRotationInput;

    private void Awake()
    {
        _RB = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CheckInputs();
        Movement();
        Rotate();
    }

    private void CheckInputs()
    {
        var hInput = Input.GetAxisRaw("Horizontal");

        var vInput = Input.GetAxisRaw("Vertical");

        float dot = Vector3.Dot(transform.forward, Vector3.forward);

        var stickInput = new Vector2(hInput, vInput);

        if (stickInput.magnitude < _RadialDeadzone)
        {
            stickInput = Vector2.zero;
        }
        else
        {
            stickInput = stickInput.normalized * ((stickInput.magnitude - _RadialDeadzone) / (1 - _RadialDeadzone));
        }

        _MoveDirectionInput = new Vector2(stickInput.x, stickInput.y);

        if (UsingController == true)
        {
            _ControllerRotationInput = new Vector2(Input.GetAxisRaw("ControllerRSH"), Input.GetAxisRaw("ControllerRSV"));
        }
    }

    private void Movement()
    {
        Vector3 direction = new Vector3(_MoveDirectionInput.x, 0, _MoveDirectionInput.y).normalized;

        if (direction == Vector3.zero)
        {
            _RB.velocity = Vector3.zero;
        }

        Vector3 vDir = _RB.transform.InverseTransformDirection(_RB.velocity);

        float dirDot = Vector3.Dot(vDir, Vector3.forward);

        if (dirDot > 0.5f)
        {
            _SpeedMult = 1f;
        }
        else
        {
            if (dirDot <= 0.5f && dirDot > -0.5f)
            {
                _SpeedMult = Mathf.Clamp(_SpeedMult, 0f, _DiagonalSpeedMult);
            }
            else
            {
                _SpeedMult = Mathf.Clamp(_SpeedMult, 0f, _BackwardsSpeedMult);
            }
        }

        var targetVel = direction * _Velocity * _SpeedMult;

        var velDif = targetVel - _RB.velocity;

        var hForce = velDif.x * _Acceleration;
        var vForce = velDif.z * _Acceleration;

        _RB.AddForce(new Vector3(hForce, 0, vForce));

        if (UsingController == true && direction != Vector3.zero)
        {
            var angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg - 90f;
            transform.eulerAngles = new Vector3(0f, -angle, 0f);
        }
    }


    private void Rotate()
    {
        if (UsingController == true)
        {
            Vector3 playerDirection = Vector3.right * _ControllerRotationInput.x + Vector3.forward * _ControllerRotationInput.y;
            if (playerDirection == Vector3.zero) return;
            if (playerDirection.sqrMagnitude > 0.0f)
            {
                transform.rotation = Quaternion.LookRotation(playerDirection * 45 * Time.deltaTime, Vector3.up);
            }
        }
        else
        {
            var Pos = Cam.WorldToViewportPoint(transform.position);
            var castPoint = Cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, _AimLayer))
            {
                var lookAtTransform = hit.point;
                lookAtTransform.y = 0f;

                var lookAtRotation = Quaternion.LookRotation(lookAtTransform - transform.position);

                var slerpedRotation = Quaternion.Slerp(transform.rotation, lookAtRotation, 45 * Time.deltaTime);

                Vector3 eulerAngle = slerpedRotation.eulerAngles;

                transform.eulerAngles = new Vector3(0f, eulerAngle.y, 0f);
            }
        }
    }
}
