using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 2f;
    [SerializeField] private bool rising = false;

    private RoundManager manager;

    public void Initialize(RoundManager rm, float startY)
    {
        manager = rm;
        ResetToStart(startY);
    }

    public void ResetToStart(float y)
    {
        var p = transform.position;
        p.y = y;
        transform.position = p;
        rising = false;
    }

    public void StartRising() => rising = true;
    public void StopRising() => rising = false;

    private void Update()
    {
        if (!rising) return;

        // Déplacement vertical
        transform.position += Vector3.up * (riseSpeed * Time.deltaTime);
    }
}