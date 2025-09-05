using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour, IFighter
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float moveSpeedHolding = 2f;
    [SerializeField] private float _jumpForce = 100f;
    [SerializeField] private bool _isFacingRight;
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private AudioSource _runningAudioSource;

    [Header("Jump")]
    [SerializeField] private float _groundCheckGap = 0.4f;
    [SerializeField] private float _groundCheckRadius = 3f;
    [SerializeField] private LayerMask _groundCheckLayers;

    [Header("Attack")]
    [SerializeField] private float _attackForce = 60f;
    [SerializeField] private float _attackGap = 0.4f;
    [SerializeField] private float _attackRadius = 3f;
    [SerializeField] private float _attackCooldown = .5f;
    [SerializeField] private LayerMask _attackLayers;
    [SerializeField] private Animator _attackAnimator;

    [Header("Charge Attack")]
    [SerializeField] private float _chargeAttackForce = 120f;
    [SerializeField] private float _necessaryHoldDuration = 1f;
    [SerializeField] private GameObject _feedBack;

    [Header("Damage")]
    [SerializeField] private float _damageDuration = .5f;
    [SerializeField] private float _bounceForce = 100f;    
    
    [Header("Events")]
    [SerializeField] private UnityEvent _onJump;
    [SerializeField] private UnityEvent _onSimpleAttack;
    [SerializeField] private UnityEvent _onChargeAttack;
    [SerializeField] private UnityEvent _onMissAttack;
    [SerializeField] private UnityEvent _onCharged;
    [SerializeField] private UnityEvent _onTakeDamage;
    [SerializeField] private UnityEvent _onBounce;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private bool _isMoving;
    private Vector2 _currentVelocity;
    private bool _isAttacking = false;
    private Coroutine _attackCoroutine;
    private bool _isBeingDamaged = false;
    private Coroutine _damageCoroutine;

    private float _attackHoldDuration;
    private bool _isHolding = false;
    private bool _isCharged = false;

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

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (Physics2D.OverlapCircle(transform.position - Vector3.up * _groundCheckGap, _groundCheckRadius, _groundCheckLayers))
        {
            _rb.AddForce(Vector2.up * _jumpForce);
            _characterAnimator.SetTrigger("Jump");
            _onJump?.Invoke();
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isHolding = true;
            _characterAnimator.SetTrigger("StartAttack");
        }
        if (context.canceled)
        {
            Attack(_attackHoldDuration >= _necessaryHoldDuration);

            _isHolding = false;
            _isCharged = false;
            _attackHoldDuration = 0.0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position - Vector3.up * _groundCheckGap, _groundCheckRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - (Vector3.right * (_isFacingRight ? 1f : -1f)) * _attackGap, _attackRadius);
    }

    private void FixedUpdate()
    {
        _characterAnimator.SetBool("IsMoving", _moveInput != Vector2.zero);

        if( _moveInput != Vector2.zero && !_isMoving || _moveInput == Vector2.zero && _isMoving)
        {
            _isMoving = !_isMoving;

            if(_isMoving)
                _runningAudioSource.Play();
            else
                _runningAudioSource.Stop();
        }

         _currentVelocity = _rb.linearVelocity;
        if (_isBeingDamaged)
            return; 
        _rb.linearVelocity = new Vector2(_moveInput.normalized.x * (_isHolding? moveSpeedHolding : moveSpeed) * Time.deltaTime,  _rb.linearVelocity.y);
        if ((_moveInput.normalized.x < 0 && _isFacingRight) || (_moveInput.normalized.x > 0 && !_isFacingRight))
        {
            _isFacingRight = !_isFacingRight;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _isFacingRight ?  0 : 180, transform.eulerAngles.z);
        }
    }

    private void Update()
    {
        if (_isHolding)
        {
            _attackHoldDuration += Time.deltaTime;
            if (_attackHoldDuration >= _necessaryHoldDuration && !_isCharged)
            {
                _onCharged?.Invoke();
                _feedBack.transform.DOScale(0.15f, .25f).SetEase(Ease.InExpo).OnComplete(() =>
                {
                    _feedBack.transform.DOScale(0.15f, 0.25f).OnComplete(() =>
                    {
                        _feedBack.transform.DOScale(0f, .7f).SetEase(Ease.OutExpo);
                    });
                });
                _isCharged = true;
            }
        }
    }

    public void Attack(bool isChargeAttack)
    {
        if (_isAttacking)
            return; 
        _attackAnimator.SetTrigger("Attack");
        _characterAnimator.SetTrigger("Attack");

        Collider2D[] enemys = Physics2D.OverlapCircleAll(transform.position - (Vector3.right * (_isFacingRight ? 1f : -1f)) * _attackGap , _attackRadius, _attackLayers);

        foreach(Collider2D enemy in enemys)
        {
            if (enemy == null || enemy.gameObject == gameObject)
                return;

            IFighter enemyInterface;
            if (enemy.TryGetComponent<IFighter>(out enemyInterface))
            {
                enemyInterface.Damage(enemy.transform.position - transform.position, isChargeAttack ? _chargeAttackForce : _attackForce, PlayerIndex); // S'il y a un enemi à range, l'attaquer


                if (isChargeAttack)
                    _onChargeAttack?.Invoke();
                else
                    _onSimpleAttack?.Invoke();

                if (_isAttacking) continue;
                if (_attackCoroutine != null)
                    StopCoroutine(_attackCoroutine);
                _attackCoroutine = StartCoroutine(Attacking());
            }
        }
        if(enemys.Length == 0)
            _onMissAttack?.Invoke();
    }
    private IEnumerator Attacking()
    {
        _isAttacking = true;
        yield return new WaitForSeconds(_attackCooldown);
        _isAttacking = false;
        _attackCoroutine = null;
    }

    public void Damage(Vector2 dir, float force, int playerID = 0)
    {
        _rb.AddForce(dir.normalized * force, ForceMode2D.Impulse); // rebondir selon la position de l'autre joueur

        if (_damageCoroutine != null)
            StopCoroutine(_damageCoroutine);
        _damageCoroutine = StartCoroutine(WaitDamage());
        _onTakeDamage?.Invoke();
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
            Vector2 bounceDirection = Vector3.Reflect(_currentVelocity, collision.contacts[0].normal);
            _rb.AddForce(bounceDirection * _bounceForce, ForceMode2D.Impulse);
            _onBounce?.Invoke();
        }
    }
}
