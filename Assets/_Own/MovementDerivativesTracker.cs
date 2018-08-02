using UnityEngine;

public class MovementDerivativesTracker : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 smoothedVelocity;
    public Vector3 acceleration;
    public Vector3 smoothedAcceleration;

    [HideInInspector]
    private Vector3 oldPosition;
    [HideInInspector]
    private Vector3 oldVelocity;

    void Start()
    {
        oldPosition = transform.position;
    }

    void Update()
    {
        Vector3 deltaPosition = transform.position - oldPosition;
        velocity = deltaPosition / Time.deltaTime;
        const float smoothingFactorVelocity = 10;
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, velocity, 1f - Mathf.Exp(-Time.deltaTime * smoothingFactorVelocity));

        Vector3 deltaVelocity = velocity - oldVelocity;
        acceleration = deltaVelocity / Time.deltaTime;
        const float smoothingFactorAcceleration = 1;
        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, acceleration, 1f - Mathf.Exp(-Time.deltaTime * smoothingFactorAcceleration));
        //Debug.DrawRay(transform.position, smoothedAcceleration);

        oldPosition = transform.position;
        oldVelocity = velocity;
    }
}
