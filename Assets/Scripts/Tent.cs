using UnityEngine;

public class Tent : MonoBehaviour, IFighter
{
    [SerializeField] int _playerIndex;
    public void Attack()
    {
        // ça attaque pas une tente
    }

    public void Damage(Vector2 dir, float force)
    {
        Debug.Log("Tent attacked : Player " + _playerIndex);
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
