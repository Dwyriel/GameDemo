using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementBase : MonoBehaviour
{
    protected Rigidbody ComponentRigidbody;
    protected Vector3 MapCenterPosition;
    protected Vector3 TargetPosition;
    protected Vector3 CurrentPosition;
    protected Vector3 ForwardDirection;
    protected Vector3 DirectionToTarget;
    protected float RotationAngle;
    protected float Inclination;
}
