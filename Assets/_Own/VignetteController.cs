using UnityEngine;
using Wilberforce.FinalVignette;

[RequireComponent(typeof(FinalVignetteCommandBuffer))]
public class VignetteController : MonoBehaviour
{

    public float currentHits = 0;
    public float hitDecayPerSecond = 0.33f;
    private FinalVignetteCommandBuffer vignette;
    private readonly Color colorNoVignette = new Color(0, 0, 0, 0);
    private readonly Color colorFullVignette = new Color(1, 0, 0, 0.7f);

    public float damagePointsPerHitOnHead = 1.0f;
    public float damagePointsPerHitOnPistol = 0.1f;

    private void OnHit(PlayerHitVolume hitVolume, GameObject hittingGameObject)
    {
        if (hitVolume.GetComponent<PistolControl>() != null)
        {
            currentHits += damagePointsPerHitOnPistol;
        }

        if (hitVolume.GetComponent<Camera>() != null)
        {
            currentHits += damagePointsPerHitOnHead;
        }
    }

    private void OnEnable()
    {
        PlayerHitVolume.OnHitAnyInstance += OnHit;
    }

    private void OnDisable()
    {
        PlayerHitVolume.OnHitAnyInstance -= OnHit;
    }

    void Start()
    {
        vignette = GetComponent<FinalVignetteCommandBuffer>();
        UpdateVignetteParameters(0.0f);
    }

    void Update()
    {
        float oldHits = currentHits;
        currentHits = Mathf.Max(0, currentHits - Time.deltaTime * hitDecayPerSecond);
        if (oldHits != currentHits)
        {
            UpdateVignetteParameters(1.0f - Mathf.Exp(-currentHits * 0.2f));
        }
    }

    private void UpdateVignetteParameters(float effectStrength)
    {
        effectStrength = Mathf.Clamp01(effectStrength);

        vignette.enabled = effectStrength > 0;
        vignette.VignetteInnerValueDistance = 0.6f - 0.6f * effectStrength;
        vignette.VignetteOuterValueDistance = Mathf.Max(0.4f, 0.8f - 1.0f * effectStrength);
        vignette.VignetteInnerColor = Color.Lerp(colorNoVignette, colorFullVignette, Mathf.Pow(effectStrength, 3));

    }
}
