using UnityEngine;

public class PistolCharge : MonoBehaviour
{

    [Tooltip("Number of shots that the pistol currently can fire. One click of the trigger consumes 1.0 shots.")]
    public float currentShots;
    [Tooltip("Maximum number of shots that the pistol can store for immediate fire.")]
    public float maxCharge;
    [Tooltip("Unit: Shots per second")]
    public float rechargeRate;

    public GameObject dischargePrefab;

    void Start()
    {
        currentShots = maxCharge;

        PlayerHitVolume playerHitVolume = GetComponent<PlayerHitVolume>();
        if (playerHitVolume != null)
        {
            playerHitVolume.OnHit += OnHit;
        }
    }

    void Update()
    {
        currentShots += Time.deltaTime * rechargeRate;
        currentShots = Mathf.Min(currentShots, maxCharge);
    }

    private void OnHit(PlayerHitVolume hitVolume, GameObject hittingGameObject)
    {
        currentShots = -maxCharge;

        if (dischargePrefab != null)
        {
            GameObject dischargeEffect = Instantiate(dischargePrefab, transform.position, transform.rotation);
            dischargeEffect.transform.localScale *= 0.33f;
        }
    }
}
