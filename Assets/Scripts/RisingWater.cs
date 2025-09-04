using UnityEngine;

public class RisingWater : MonoBehaviour
{
    [SerializeField] private float riseSpeed = 2f;
    [SerializeField] private bool rising;

    private RoundManager manager;

    public void Initialize(RoundManager rm, float speed, float startY)
    {
        manager = rm;
        riseSpeed = speed;
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
        transform.position += Vector3.up * (riseSpeed * Time.deltaTime);
    }
}