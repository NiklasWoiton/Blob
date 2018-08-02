using System.Collections.Generic;
using UnityEngine;

public class PistolTarget : MonoBehaviour
{
    public delegate void PistolHitHandler(GameObject hitGameObject);

    public event PistolHitHandler OnHit;

    // needed by PistolControl
    public SphereCollider sphereCollider;

    public void Start()
    {
        if (sphereCollider == null)
        {
            sphereCollider = transform.GetComponentInChildren<SphereCollider>();
        }
    }

    public void Hit()
    {
        if (OnHit != null)
        {
            OnHit(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region tracking all instances

    public static List<PistolTarget> instances = new List<PistolTarget>();

    private void OnEnable()
    {
        instances.Add(this);
    }

    private void OnDisable()
    {
        instances.Remove(this);
    }

    #endregion
}
