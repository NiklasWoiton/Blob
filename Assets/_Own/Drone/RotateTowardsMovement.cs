using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsMovement : MonoBehaviour {

    public MovementDerivativesTracker movementDerivativesTracker;

    [HideInInspector]
    private Vector3 oldHorizontalVelocityNormalized;
	
	void Update () {
        Vector3 movementDirection = movementDerivativesTracker.velocity.normalized;
        float forwardSpeed = Vector3.Dot(movementDerivativesTracker.velocity, transform.forward);

        //forward.y = -forwardSpeed;
        //forward.Normalize();
        //transform.localRotation = Quaternion.FromToRotation(Vector3.forward, forward);

        Vector3 horizontalVelocityNormalized = movementDirection;
        horizontalVelocityNormalized.y = 0;
        horizontalVelocityNormalized.Normalize();

        // look towards movement direction
        Quaternion yaw = Quaternion.FromToRotation(Vector3.forward, horizontalVelocityNormalized);

        // lean forward when moving forward
        Quaternion pitch = Quaternion.AngleAxis(forwardSpeed * 10, Vector3.right);

        // roll sideways while changing direction
        float rollAngle = -Quaternion.FromToRotation(oldHorizontalVelocityNormalized, horizontalVelocityNormalized).eulerAngles.y;
        Quaternion roll = Quaternion.AngleAxis(rollAngle * 30, Vector3.forward);

        Quaternion targetRotation = yaw * pitch * roll;
        const float smoothingFactor = 2;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, 1f - Mathf.Exp(-Time.deltaTime * smoothingFactor));

        oldHorizontalVelocityNormalized = horizontalVelocityNormalized;
	}
}
