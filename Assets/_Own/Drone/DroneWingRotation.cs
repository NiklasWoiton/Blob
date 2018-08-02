using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneWingRotation : MonoBehaviour {

    public MovementDerivativesTracker movementDerivativesTracker;

	void Update () {
        float forwardSpeed = Vector3.Dot(movementDerivativesTracker.smoothedVelocity, transform.forward);
        float forwardAcceleration = Vector3.Dot(movementDerivativesTracker.smoothedAcceleration, transform.forward);
        transform.localRotation = Quaternion.AngleAxis(forwardSpeed * 0 + forwardAcceleration * 10, Vector3.right);
	}
}
