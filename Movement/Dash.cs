using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dash : MonoBehaviour
{
    private Rigidbody _RB;
    private Animator _Anim;

    [SerializeField]
    private KeyCode _DashInput = KeyCode.Space;
    private bool _IsDashing = false;
    [SerializeField]
    private bool _DashInAimDirection = false;

    [SerializeField]
    private float _Duration = 5f;
    private float _CurrentTime = 0f;
    [SerializeField]
    private float _Distance = 12f;
    [SerializeField]
    private float _StoppingDistance = 1f;

    [SerializeField]
    private LayerMask _ObstacleLayers = default;

    private Vector3 _StartPos = default;
    private Vector3 _EndPos = default;

    private void Awake()
    {
        _RB = GetComponent<Rigidbody>();
        _Anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        _IsDashing = false;
        _CurrentTime = 0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(_DashInput) && _IsDashing == false)
        {
            StartDash();
        }

        if (_IsDashing == true)
        {
            var distance = Vector3.Distance(_StartPos, _EndPos);

            if (distance < 0.5f)
            {
                EndDash();
            }
        }
    }

    private void FixedUpdate()
    {
        if (_IsDashing == true)
        {
            Move();
        }
    }

    public void StartDash()
    {
        _CurrentTime = 0;
        _StartPos = transform.position;
        _EndPos = transform.position;
        CalculateDistance();
        _RB.isKinematic = true;
        _IsDashing = true;
    }

    private void Move()
    {
        _CurrentTime += Time.fixedUnscaledDeltaTime;
        _CurrentTime = Mathf.Clamp(_CurrentTime, 0, _Duration);
        _StartPos = transform.position;

        float perc = _CurrentTime / _Duration;

        var movement = Vector3.Lerp(_StartPos, _EndPos, perc);
        _RB.MovePosition(movement);

        Debug.DrawLine(transform.position, _EndPos, Color.cyan);
    }

    private void EndDash()
    {
        _IsDashing = false;
        _RB.isKinematic = false;
    }

    private void CalculateDistance()
    {
        Vector3 direction;
        if (_DashInAimDirection == false)
        {
            direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            if (direction.x <= 0.1 && direction.z <= 0.1 && direction.x > -0.1 && direction.z > -0.1)
            {
                direction = transform.forward * 0.9f;
            }
        }
        else
        {
            direction = transform.forward;
        }

        RaycastHit hit;

        Vector3 centerOfSphere1 = transform.position + Vector3.up * (1f + Physics.defaultContactOffset);
        Vector3 centerOfSphere2 = transform.position + Vector3.down * (1f + Physics.defaultContactOffset);

        if (Physics.CapsuleCast(centerOfSphere1, centerOfSphere2, 0.5f, direction, out hit, _Distance, _ObstacleLayers))
        {
            float distanceToObstacle = Vector3.Distance(transform.position, hit.point);
            _EndPos = transform.position + (direction * (distanceToObstacle - _StoppingDistance));

            Debug.DrawLine(transform.position + Vector3.up, _EndPos + Vector3.up, Color.green, 5f);
        }
        else
        {
            Debug.DrawLine(centerOfSphere1, centerOfSphere2, Color.red, 5f);
            _EndPos = transform.position + (direction * _Distance);
        }
    }
}
