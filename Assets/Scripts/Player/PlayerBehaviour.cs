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
    [SerializeField] private Animator _attackAnimator;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private GameObject _enemyInRange;

    public int PlayerIndex {  get; set; }
    public PlayerInputManager PlayerInputs {  get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.freezeRotation = true;
        if (!_isFacingRight)
            transform.localScale = Vector2.Scale(transform.localScale, new Vector2(-1, 1));
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
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_moveInput.normalized.x * moveSpeed * Time.deltaTime,  _rb.linearVelocity.y);
        if ((_moveInput.normalized.x < 0 && !_isFacingRight) || (_moveInput.normalized.x > 0 && _isFacingRight))
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _isFacingRight ?  0 : 180, transform.eulerAngles.z);
            _isFacingRight = !_isFacingRight;
        }
    }

    public void Attack()
    {
        _attackAnimator.SetTrigger("Attack");

        if(_enemyInRange == null)
            return;
        IFighter enemy;
        if (_enemyInRange.TryGetComponent<IFighter>(out enemy))
        {
            enemy.Damage(_enemyInRange.transform.position - transform.position, _attackForce); // S'il y a un enemi à range, l'attaquer
        }
    }

    public void Damage(Vector2 dir, float force)
    {
        Debug.Log(PlayerIndex + " Damage");
        _rb.AddForce(dir * force); // rebondir selon la position de l'autre joueur
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != gameObject)
            _enemyInRange = collision.gameObject;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject != gameObject && _enemyInRange == collision.gameObject)
            _enemyInRange = null;
    }
}
