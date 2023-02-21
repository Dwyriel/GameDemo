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

    protected virtual void SetStartAttributes(IEnumerable<Renderer> renderers, Color color)
    {
        MapCenterPosition = GameObject.FindWithTag(ConstValuesAndUtility.MapCenterPointTag).transform.position;
        ComponentRigidbody = GetComponent<Rigidbody>();
        ChangeColor(renderers, color);
        GenerateTargetPosition();
    }

    protected virtual void UpdateAttributes()
    {
        CurrentPosition = transform.position;
        ForwardDirection = transform.forward;
        ComponentRigidbody.MovePosition(CurrentPosition + ForwardDirection * ConstValuesAndUtility.MovingUnitsPerSecond * Time.fixedDeltaTime);
        DirectionToTarget = TargetPosition - CurrentPosition;
    }

    protected void ChangeColor(IEnumerable<Renderer> renderers, Color color)
    {
        foreach (var renderer in renderers)
        {
            if (renderer.gameObject.name == "Cockpit")
                continue;
            renderer.material.color = color;
        }
    }

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