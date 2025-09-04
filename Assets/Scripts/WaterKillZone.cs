using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterKillZone : MonoBehaviour
{
    [SerializeField] private RoundManager roundManager;

    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cherche un PlayerBehaviour dans l’objet ou ses parents
        var player = other.GetComponentInParent<PlayerBehaviour>() ?? other.GetComponent<PlayerBehaviour>();
        if (player != null)
        {
            roundManager.RegisterDrown(player);
        }
    }
}