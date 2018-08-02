using UnityEngine;

public class EnemyProjectileBehavior : MonoBehaviour
{

    public float speed;

    private float spawnTime;

    private void Start()
    {
        Vector3 towardsPlayer = Camera.main.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(towardsPlayer, Vector3.up);

        spawnTime = Time.time;

        PistolTarget pistolTarget = GetComponent<PistolTarget>();
        if (pistolTarget != null)
        {
            pistolTarget.OnHit += (hitGameObject =>
            {
                Disappear();
                ParticleSystem.MainModule particleSystemMain = GetComponent<ParticleSystem>().main;
                particleSystemMain.simulationSpeed *= 3;
            });
        }
    }

    void Update()
    {
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        transform.position += direction * speed * Time.deltaTime;

        // failsafe: remove projectile 2 minutes after spawn
        // in case the projectile somehow managed to get through the walls
        if (spawnTime + 120 < Time.time)
        {
            Destroy(gameObject);
        }
    }

    private bool IsValidCollisionTarget(Collider other)
    {
        return other.GetComponent<DroneFiringPoint>() == null
            && other.GetComponent<DroneRepellingSphere>() == null
            && other.GetComponent<EnemyProjectileBehavior>() == null
            && other.GetComponentInParent<DroneBehavior>() == null;

        // should only leave objects that have PlayerHitVolume
        // ...and the good old walls
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidCollisionTarget(other))
        {
            PlayerHitVolume playerHitVolume = other.GetComponent<PlayerHitVolume>();
            if (playerHitVolume != null)
            {
                playerHitVolume.Hit(gameObject);
            }

            if (other.GetComponent<PistolControl>() != null)
            {
                Vector3 direction = transform.TransformDirection(Vector3.forward);
                Vector3 impactPoint = other.ClosestPoint(transform.position);
                Vector3 normal = (transform.position - impactPoint).normalized;
                Vector3 newDirection = Vector3.Reflect(direction, normal);
                transform.rotation *= Quaternion.FromToRotation(direction, newDirection);
            }
            else
            {
                Disappear();
            }
        }
    }

    public void Disappear()
    {
        // stop movement
        speed = 0;

        // don't get hit anymore
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // stop emitting new particles
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // destroy self after all already existing particles have disappeared
        float remainingLifetime = particleSystem.main.startLifetime.constantMax;
        Destroy(gameObject, remainingLifetime);
    }
}
