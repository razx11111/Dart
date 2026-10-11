using UnityEngine;

public class DartTarget : MonoBehaviour
{
    public Transform centerPoint;
    public AudioSource hitAudio;
    public ParticleSystem confetti;
    public Renderer targetRenderer;

    // raze sectoare oficiale darts (în metri)
    public float rInnerBull = 0.016f;
    public float rOuterBull = 0.038f;
    public float rTripleInner = 0.185f;
    public float rTripleOuter = 0.215f;
    public float rDoubleInner = 0.310f;
    public float rDoubleOuter = 0.340f;

    // ordinea sectoarelor la darts
    private static readonly int[] Sectors = { 20,1,18,4,13,6,10,15,2,17,3,19,7,16,8,11,14,9,12,5 };

    private Color originalColor;
    private Material mat;

    void Start()
    {
        if (centerPoint == null) centerPoint = transform;
        if (targetRenderer != null)
        {
            mat = targetRenderer.material;
            originalColor = mat.HasProperty("_BaseColor") ? mat.GetColor("_BaseColor") : mat.color;
        }
    }

    public void ProcessHit(Vector3 hitPoint)
    {
        Vector3 local = transform.InverseTransformPoint(hitPoint) - transform.InverseTransformPoint(centerPoint.position);
        float dist = new Vector2(local.x, local.y).magnitude;

        int score; string label;
        if (dist <= rInnerBull)           { score = 50; label = "DOUBLE BULL! +50"; }
        else if (dist <= rOuterBull)      { score = 25; label = "BULL! +25"; }
        else if (dist > rDoubleOuter)     { score = 0;  label = "MISS!"; }
        else
        {
            float angle = Mathf.Atan2(local.x, local.y) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;
            int sector = Sectors[Mathf.FloorToInt((angle + 9f) / 18f) % 20];

            if (dist >= rTripleInner && dist <= rTripleOuter)
                { score = sector * 3; label = $"TRIPLE {sector}! +{score}"; }
            else if (dist >= rDoubleInner && dist <= rDoubleOuter)
                { score = sector * 2; label = $"DOUBLE {sector}! +{score}"; }
            else
                { score = sector; label = $"{sector}! +{score}"; }
        }

        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(score, label);

        // feedback vizual: flash culoare
        if (mat != null) { CancelInvoke(); SetColor(Color.yellow); Invoke("ResetColor", 0.3f); }

        // feedback audio
        if (hitAudio != null) hitAudio.Play();

        // confetti la bullseye
        if (score >= 50 && confetti != null) { confetti.transform.position = hitPoint; confetti.Play(); }
    }

    void SetColor(Color c) { if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c); else mat.color = c; }
    void ResetColor() { SetColor(originalColor); }
}
