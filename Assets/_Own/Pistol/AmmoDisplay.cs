using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AmmoDisplay : MonoBehaviour
{
    [Tooltip("A reference to the script that provides the current charge level.")]
    public PistolCharge pistolCharge;

    public Color[] displayColorsForAmmoLevel;
    private Text ammoDisplayText;

    void Start()
    {
        ammoDisplayText = GetComponent<Text>();
        if (pistolCharge == null)
        {
            pistolCharge = transform.GetComponentInParent<PistolCharge>();
        }
    }

    void Update()
    {
        int fullShots = Mathf.Max(0, (int)Mathf.Floor(pistolCharge.currentShots));
        if (fullShots.ToString() != ammoDisplayText.text)
        {
            ammoDisplayText.text = fullShots.ToString();
            ammoDisplayText.color = displayColorsForAmmoLevel[Mathf.Min(fullShots, displayColorsForAmmoLevel.Length-1)];
        }
    }
}
