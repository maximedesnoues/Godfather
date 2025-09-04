using UnityEngine;
using UnityEngine.Events;

public class Tent : MonoBehaviour, IFighter
{
    public int PlayerIndex;
    [SerializeField] int _playerIndex;
    [SerializeField] UnityEvent OnTentHit;
    public void Attack(bool isChargeAttack)
    {
        // ça attaque pas une tente
    }

    public void Damage(Vector2 dir, float force, int playerID = 0)
    {
        if(_playerIndex == PlayerIndex) 
            return;
        Debug.Log("Tent attacked : Player " + _playerIndex);
        OnTentHit?.Invoke();
        FeedbackManager.Instance.ScreenShake(.5f, 1f);
        FeedbackManager.Instance.VibrateAllControllers(.3f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
