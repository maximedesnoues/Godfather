using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour, IFighter
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float _jumpForce = 100f;
    [SerializeField] private bool _isFacingRight;

    [Header("Jump")]
    [SerializeField] private float _groundCheckGap = 0.4f;
    [SerializeField] private float _groundCheckRadius = 3f;
    [SerializeField] private LayerMask _groundCheckLayers;

    [Header("Attack")]
    [SerializeField] private float _attackForce = 60f;
    [SerializeField] private float _attackGap = 0.4f;
    [SerializeField] private float _attackRadius = 3f;
    [SerializeField] private LayerMask _attackLayers;
    [SerializeField] private Animator _attackAnimator;

    [Header("Damage")]
    [SerializeField] private float _damageDuration = .5f;
    [SerializeField] private float _bounceForce = 100f;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private bool _isAttacking = false;
    private Coroutine _attackCoroutine;
    private bool _isBeingDamaged = false;
    private Coroutine _damageCoroutine;

    public int PlayerIndex {  get; set; }
    public PlayerInputManager PlayerInputs {  get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, _isFacingRight ? 0 : 180, transform.eulerAngles.z);

    }

    public void OnConnectController(PlayerInputManager inputs)
    {
        PlayerInputs = inputs;
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (Physics2D.OverlapCircle(transform.position - Vector3.up * _groundCheckGap, _groundCheckRadius, _groundCheckLayers))
        {
            _rb.AddForce(Vector2.up * _jumpForce);
        }
    }

    public void OnInteract(InputValue value)
    {
        Attack();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position - Vector3.up * _groundCheckGap, _groundCheckRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - (Vector3.right * (_isFacingRight ? 1f : -1f)) * _attackGap, _attackRadius);
    }

    private void FixedUpdate()
    {
        if(_isBeingDamaged)
            return;
        _rb.linearVelocity = new Vector2(_moveInput.normalized.x * moveSpeed * Time.deltaTime,  _rb.linearVelocity.y);
        if ((_moveInput.normalized.x < 0 && _isFacingRight) || (_moveInput.normalized.x > 0 && !_isFacingRight))
        {
            _isFacingRight = !_isFacingRight;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _isFacingRight ?  0 : 180, transform.eulerAngles.z);
        }
    }

    public void Attack()
    {
        if (_isAttacking)
            return;
        _attackAnimator.SetTrigger("Attack");

        Collider2D[] enemys = Physics2D.OverlapCircleAll(transform.position - (Vector3.right * (_isFacingRight ? 1f : -1f)) * _attackGap , _attackRadius, _attackLayers);

        foreach(Collider2D enemy in enemys)
        {
            if (enemy == null || enemy.gameObject == gameObject)
                return;

            IFighter enemyInterface;
            if (enemy.TryGetComponent<IFighter>(out enemyInterface))
            {
                enemyInterface.Damage(enemy.transform.position - transform.position, _attackForce); // S'il y a un enemi à range, l'attaquer

                if(_isAttacking) continue;
                if (_attackCoroutine != null)
                    StopCoroutine(_attackCoroutine);
                _attackCoroutine = StartCoroutine(Attacking());
            }
        }
    }
    private IEnumerator Attacking()
    {
        _isAttacking = true;
        yield return new WaitForSeconds(_damageDuration);
        _isAttacking = false;
        _attackCoroutine = null;
    }

    public void Damage(Vector2 dir, float force)
    {
        _rb.AddForce(dir.normalized * force, ForceMode2D.Impulse); // rebondir selon la position de l'autre joueur

        if (_damageCoroutine != null)
            StopCoroutine(_damageCoroutine);
        _damageCoroutine = StartCoroutine(WaitDamage());
    }

    private IEnumerator WaitDamage()
    {
        _isBeingDamaged = true;
        yield return new WaitForSeconds(_damageDuration);
        _isBeingDamaged = false;
        _damageCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isBeingDamaged)
            return;
        if (collision != null)
        {
            Vector2 bounceDirection = Vector3.Reflect(_rb.linearVelocity, collision.contacts[0].normal);
            _rb.AddForce(bounceDirection.normalized * _bounceForce, ForceMode2D.Impulse);
        }
    }
}
