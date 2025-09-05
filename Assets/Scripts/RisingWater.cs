using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RisingWater : MonoBehaviour
{
    [Header("Montée par scale (bas fixe)")]
    [SerializeField] private float riseSpeed = 0.5f;
    [SerializeField] private float startScaleY = 0.05f;
    [SerializeField] private float maxScaleY = 100f;

    [Header("Décalage de départ vers le haut (en unités monde)")]
    [SerializeField] private float startYOffset = 0.5f;

    private RoundManager manager;
    private float fixedBottomY;
    private float baseScaleX, baseScaleZ;
    private SpriteRenderer sr;
    private bool rising = false;

    public void Initialize(RoundManager rm, float baseBottomY)
    {
        manager = rm;
        if (sr == null) sr = GetComponent<SpriteRenderer>();

        baseScaleX = transform.localScale.x;
        baseScaleZ = transform.localScale.z;

        // Décalage appliqué dès l'init
        fixedBottomY = baseBottomY + startYOffset;

        ApplyStartState();
    }

    public void ResetToStart(float baseBottomY)
    {
        // Décalage réappliqué à chaque reset
        fixedBottomY = baseBottomY + startYOffset;

        ApplyStartState();
    }

    public void StartRising() => rising = true;
    public void StopRising() => rising = false;

    private void Update()
    {
        if (!rising) return;

        // Augmente l’échelle Y
        float newY = Mathf.Min(transform.localScale.y + riseSpeed * Time.deltaTime, maxScaleY);
        SetScaleY(newY);

        // Recalage du centre pour garder le bas fixé
        UpdatePositionForBottom();
    }

    private void ApplyStartState()
    {
        SetScaleY(Mathf.Max(0.0001f, startScaleY));
        UpdatePositionForBottom();
        rising = false;
    }

    private void SetScaleY(float y)
    {
        var s = transform.localScale;
        s.x = baseScaleX;
        s.y = y;
        s.z = baseScaleZ;
        transform.localScale = s;
    }

    private void UpdatePositionForBottom()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float height = sr.bounds.size.y;

        var p = transform.position;
        p.y = fixedBottomY + height * 0.5f;
        transform.position = p;
    }
}