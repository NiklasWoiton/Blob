using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DroneBehavior : MonoBehaviour
{

    public GameObject explosionPrefab;
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public Transform muzzleTransform;

    public BoxCollider selectedFiringPoint;
    public Vector3 currentTarget = Vector3.positiveInfinity;

    public float maxSpeed = 1;
    public MovementDerivativesTracker movementDerivativesTracker;

    [HideInInspector]
    private float initialDistanceToCurrentTarget = 10000;
    [HideInInspector]
    private float myRadius;

    private float distanceToConsiderTargetReached;

    public enum MovementMode { MoveTowardsTarget, RotateTowardsTarget, RotateTowardsPlayer, FireOnPlayer };
    public MovementMode movementMode = MovementMode.RotateTowardsTarget;
    private float timestampOfLastMovementModeChange;

    // Use this for initialization
    void Start()
    {
        myRadius = GetComponentInChildren<SphereCollider>().radius;
        distanceToConsiderTargetReached = myRadius / 2;
        Debug.Assert(movementDerivativesTracker != null);
        timestampOfLastMovementModeChange = Time.time;

        if (muzzleTransform == null)
        {
            muzzleTransform = transform.Find("Muzzle");
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (movementMode)
        {
            case MovementMode.RotateTowardsTarget:
                {
                    if (selectedFiringPoint == null)
                    {
                        SelectRandomFiringPointAndCurrentTarget();
                    }

                    // this causes rotation
                    VerySlowlyMovePositionTowardsCurrentTarget();

                    // rotate for 1 second
                    if (timestampOfLastMovementModeChange + 1 < Time.time)
                    {
                        movementMode = MovementMode.MoveTowardsTarget;
                        timestampOfLastMovementModeChange = Time.time;
                        break;
                    }

                    break;
                }

            case MovementMode.MoveTowardsTarget:
                {
                    MovePositionTowardsCurrentTarget();

                    float distanceToTarget = (currentTarget - transform.position).magnitude;
                    if (distanceToTarget < distanceToConsiderTargetReached)
                    {
                        movementMode = MovementMode.RotateTowardsPlayer;
                        timestampOfLastMovementModeChange = Time.time;
                        break;
                    }

                    break;
                }

            case MovementMode.RotateTowardsPlayer:
                {
                    currentTarget = Camera.main.transform.position;

                    // this causes rotation
                    VerySlowlyMovePositionTowardsCurrentTarget();

                    // rotate for 1 second
                    if (timestampOfLastMovementModeChange + 1 < Time.time)
                    {
                        movementMode = MovementMode.FireOnPlayer;
                        timestampOfLastMovementModeChange = Time.time;
                        break;
                    }

                    break;
                }

            case MovementMode.FireOnPlayer:
                {
                    if (muzzleFlashPrefab != null)
                    {
                        Instantiate(muzzleFlashPrefab, muzzleTransform.position, muzzleTransform.rotation);
                    }

                    if (projectilePrefab != null)
                    {
                        Instantiate(projectilePrefab, muzzleTransform.position, muzzleTransform.rotation);
                    }

                    // go to new target
                    selectedFiringPoint = null;
                    movementMode = MovementMode.RotateTowardsTarget;
                    timestampOfLastMovementModeChange = Time.time;
                    break;
                }
        }

        if (selectedFiringPoint != null)
        {
            Debug.DrawLine(transform.position, currentTarget, Color.gray);
        }
    }

    private void SelectRandomFiringPointAndCurrentTarget()
    {
        // look up all firing points, sort them near to far
        var firingPointsNearToFar = FindObjectsOfType<DroneFiringPoint>()
            .OrderBy(droneFiringPoint => (droneFiringPoint.transform.position - transform.position).magnitude);

        // select one randomly, nearest with highest probability
        int i = 0;
        while (Random.value > 0.5f)
        {
            i = (i + 1) % firingPointsNearToFar.Count();
        }

        // every firing point must have a box collider
        selectedFiringPoint = firingPointsNearToFar.ElementAt(i).GetComponent<BoxCollider>();

        // pick random point within firing point's box collider as currentTarget
        currentTarget = selectedFiringPoint.transform.TransformPoint(new Vector3(
            selectedFiringPoint.center.x + (Random.value - 0.5f) * selectedFiringPoint.size.x,
            selectedFiringPoint.center.y + (Random.value - 0.5f) * selectedFiringPoint.size.y,
            selectedFiringPoint.center.z + (Random.value - 0.5f) * selectedFiringPoint.size.z
            ));
    }

    private void MovePositionTowardsCurrentTarget()
    {
        float currentdistanceToCurrentTarget = (transform.position - currentTarget).magnitude;
        float speedFactor = 1;
        speedFactor = Mathf.Min(speedFactor, currentdistanceToCurrentTarget);
        speedFactor = Mathf.Min(speedFactor, initialDistanceToCurrentTarget - currentdistanceToCurrentTarget);
        speedFactor = Mathf.Max(speedFactor, 0.01f);
        float speed = speedFactor * maxSpeed;
        var direction = (currentTarget - transform.position).normalized;
        var droneRepellingforce = QueryDroneRepellingForce();
        if (droneRepellingforce.magnitude > 0.01f)
        {
            direction = Vector3.Slerp(direction, droneRepellingforce.normalized, droneRepellingforce.magnitude);
            Debug.DrawRay(transform.position, droneRepellingforce, Color.red);
        }
        transform.position += direction * Time.deltaTime * speed;
    }

    private void VerySlowlyMovePositionTowardsCurrentTarget()
    {
        float currentdistanceToCurrentTarget = (transform.position - currentTarget).magnitude;
        float speedFactor = 0.01f;
        float speed = speedFactor * maxSpeed;
        var direction = (currentTarget - transform.position).normalized;
        transform.position += direction * Time.deltaTime * speed;
    }

    private Vector3 QueryDroneRepellingForce()
    {
        Vector3 totalForce = Vector3.zero;
        foreach (DroneRepellingSphere droneRepellingSphere in DroneRepellingSphere.instances)
        {
            totalForce += droneRepellingSphere.QueryNormalizedRepellingVector(transform.position);
        }
        return totalForce;
    }

    private void PickNewRandomTarget()
    {
        //Debug.Log("PickNewRandomTarget");
        RaycastHit raycastHit;
        if (Physics.SphereCast(transform.position, myRadius * 2, Random.onUnitSphere, out raycastHit))
        {
            currentTarget = raycastHit.point;
            initialDistanceToCurrentTarget = (transform.position - currentTarget).magnitude;
            //Debug.Log(currentTarget);
        }
    }

    private bool applicationIsQuitting = false;

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    private void OnDestroy()
    {
        if (!applicationIsQuitting && explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
    }
}
