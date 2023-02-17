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
    
    protected void GenerateTargetPosition()
    {
        const float range = ConstValuesAndUtility.RandomMoveTargetRange;
        TargetPosition = MapCenterPosition + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
    }
    
    protected void CalculateRotationAndInclination(float angle)
    {
        switch (angle)
        {
            case >= -ConstValuesAndUtility.AngleSnapRange and <= ConstValuesAndUtility.AngleSnapRange:
                RotationAngle = 0;
                Inclination = Inclination switch
                {
                    > ConstValuesAndUtility.AngleSnapRange => Mathf.Clamp(Inclination + -ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime, -ConstValuesAndUtility.MaximumInclination, ConstValuesAndUtility.MaximumInclination),
                    < -ConstValuesAndUtility.AngleSnapRange => Mathf.Clamp(Inclination + ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime, -ConstValuesAndUtility.MaximumInclination, ConstValuesAndUtility.MaximumInclination),
                    _ => 0
                };
                break;
            case > ConstValuesAndUtility.AngleSnapRange:
                RotationAngle = -ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime;
                Inclination = Mathf.Clamp(Inclination + ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime, -ConstValuesAndUtility.MaximumInclination, ConstValuesAndUtility.MaximumInclination);
                break;
            case < -ConstValuesAndUtility.AngleSnapRange:
                RotationAngle = ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime;
                Inclination = Mathf.Clamp(Inclination + -ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime, -ConstValuesAndUtility.MaximumInclination, ConstValuesAndUtility.MaximumInclination);
                break;
            default:
                return;
        }
    }
}
