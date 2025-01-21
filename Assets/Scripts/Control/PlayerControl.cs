using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(TrailRenderer))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 8f;
    [SerializeField] private float _jumpForce = 16f;
    [SerializeField] private float _dashSpeed = 16f;
    [SerializeField] private float _dashTime = 0.2f;
    [SerializeField] private float _dashCooldown = 2f;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _jumpCooldown = 0.2f;
    [SerializeField] private float _lastMoveDirection = 1;

    [Header("Ground Sense")]
    [SerializeField] private Transform _feetPos;
    [SerializeField] private float _checkRadius;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _rb;
    private TrailRenderer _tr;
    private float _moveDirection;
    private bool _isDashing = false;
    private bool _canDash = true;
    private float _lastGroundedTime;
    private float _lastJumpTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _tr = GetComponent<TrailRenderer>();
    }

    private void Start()
    {
        // Initialize input manager to set this object as controllable player
        InputManager.Instance.Initialize(this);
    }

    private void FixedUpdate()
    {
        if (!_isDashing)
        {
            Move();
        }

        if (IsGrounded())
        {
            _lastGroundedTime = Time.time;
        }

        FlipCharacter();
    }

    private void Move()
    {
        _rb.velocity = new Vector2(_moveDirection * _moveSpeed, _rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>().x;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && !_isDashing && _canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && (IsGrounded() || Time.time - _lastGroundedTime <= _coyoteTime) && Time.time - _lastJumpTime >= _jumpCooldown)
        {
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _lastJumpTime = Time.time;
        }

        if (context.canceled)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
        }
    }

    private IEnumerator PerformDash()
    {
        _isDashing = true;
        _canDash = false;
        float originalGravity = _rb.gravityScale;

        _rb.gravityScale = 0;
        _rb.velocity = new Vector2(_lastMoveDirection * _dashSpeed, 0f);
        _tr.emitting = true;

        yield return new WaitForSeconds(_dashTime);

        _tr.emitting = false;
        _rb.gravityScale = originalGravity;
        _isDashing = false;

        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(_feetPos.position, new Vector2(0.8f, 0.2f), 0f, _groundLayer);
    }

    private void FlipCharacter()
    {
        if (_moveDirection > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            _lastMoveDirection = 1;
        }
        else if (_moveDirection < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            _lastMoveDirection = -1;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_feetPos.position, new Vector2(0.8f, 0.2f));
    }
}
