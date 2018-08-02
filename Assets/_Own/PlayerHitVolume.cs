using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerHitVolume : MonoBehaviour {

    public delegate void HitHandler(PlayerHitVolume hitVolume, GameObject hittingGameObject);
    public static event HitHandler OnHitAnyInstance;
    public event HitHandler OnHit;

    public void Hit(GameObject other)
    {
        OnHit?.Invoke(this, other);
        OnHitAnyInstance?.Invoke(this, other);
    }
}
