using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Tent : MonoBehaviour, IFighter
{
    [SerializeField] int _playerIndex;
    [SerializeField] UnityEvent OnTentHit;
    public int PlayerIndex;
    RoundManager _roundManager;
    TentLives _life;

    public void Attack(bool isChargeAttack)
    {
        // ça attaque pas une tente
    }

    public void Damage(Vector2 dir, float force, int playerID = 0)
    {
        if(_playerIndex == PlayerIndex) 
            return;
        Debug.Log("Tent attacked : Player " + _playerIndex);

        _roundManager.RegisterDrown(_playerIndex);

        OnTentHit?.Invoke();
        FeedbackManager.Instance.ScreenShake(.5f, 1f);
        FeedbackManager.Instance.VibrateAllControllers(.3f);

    }

    void Start()
    {
        _roundManager = FindAnyObjectByType<RoundManager>();
        _life = GetComponent<TentLives>();
    }

}
