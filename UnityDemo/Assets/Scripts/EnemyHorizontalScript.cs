using UnityEngine;

public class EnemyHorizontalScript : EnemyMovementBase
{
    private enum CurrentState
    {
        AwayFromTarget,
        FinishRotating,
        FlyingStraight
    }

    private CurrentState _currentState = CurrentState.AwayFromTarget;

    private void Start()
    {
        MapCenterPosition = GameObject.FindWithTag(ConstValuesAndUtility.MapCenterPointTag).transform.position;
        ComponentRigidbody = GetComponent<Rigidbody>();
        GenerateTargetPosition();
    }

    private void FixedUpdate()
    {
        CurrentPosition = transform.position;
        ForwardDirection = transform.forward;
        ComponentRigidbody.MovePosition(CurrentPosition + ForwardDirection * ConstValuesAndUtility.MovingUnitsPerSecond * Time.fixedDeltaTime);
        DirectionToTarget = TargetPosition - CurrentPosition;
        switch (_currentState)
        {
            case CurrentState.AwayFromTarget:
                AwayFromTargetUpdate();
                break;
            case CurrentState.FinishRotating:
                FinishRotatingUpdate();
                break;
            case CurrentState.FlyingStraight:
                FlyingStraightUpdate();
                break;
            default:
                return;
        }
    }

    private void AwayFromTargetUpdate()
    {
        if (DirectionToTarget.magnitude < ConstValuesAndUtility.MaxAllowedDistanceToTarget)
            SwitchToFinishRotatingState();
        var angleToTarget = Vector3.SignedAngle(DirectionToTarget, ForwardDirection, Vector3.up);
        CalculateRotationAndInclination(angleToTarget);
        var eulerAngles = transform.eulerAngles;
        ComponentRigidbody.MoveRotation(Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + RotationAngle, Inclination)));
    }

    private void FinishRotatingUpdate()
    {
        var angleToTarget = Vector3.SignedAngle(DirectionToTarget, ForwardDirection, Vector3.up);
        if (angleToTarget is > -ConstValuesAndUtility.AngleSnapRange and < ConstValuesAndUtility.AngleSnapRange)
            SwitchToFlyingStraightState();
        CalculateRotationAndInclination(angleToTarget);
        var eulerAngles = transform.eulerAngles;
        ComponentRigidbody.MoveRotation(Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + RotationAngle, Inclination)));
    }

    private void FlyingStraightUpdate()
    {
        if (DirectionToTarget.magnitude > ConstValuesAndUtility.MaxAllowedDistanceToTarget)
            SwitchToAwayFromTargetState();
        CalculateRotationAndInclination(0);
        var eulerAngles = transform.eulerAngles;
        ComponentRigidbody.MoveRotation(Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + RotationAngle, Inclination)));
    }

    private void SwitchToAwayFromTargetState()
    {
        _currentState = CurrentState.AwayFromTarget;
        GenerateTargetPosition();
    }

    private void SwitchToFinishRotatingState()
    {
        _currentState = CurrentState.FinishRotating;
    }

    private void SwitchToFlyingStraightState()
    {
        _currentState = CurrentState.FlyingStraight;
    }

    private void GenerateTargetPosition()
    {
        const float range = ConstValuesAndUtility.RandomMoveTargetRange;
        TargetPosition = MapCenterPosition + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
    }

    private void CalculateRotationAndInclination(float angle)
    {
        switch (angle)
        {
            case >= -ConstValuesAndUtility.AngleSnapRange and <= ConstValuesAndUtility.AngleSnapRange:
                RotationAngle = 0;
                Inclination = Inclination switch
                {
                    > ConstValuesAndUtility.AngleSnapRange => Mathf.Clamp(Inclination + -ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime, 0, ConstValuesAndUtility.MaximumInclination),
                    < -ConstValuesAndUtility.AngleSnapRange => Mathf.Clamp(Inclination + ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime, -ConstValuesAndUtility.MaximumInclination, 0),
                    _ => 0
                };
                break;
            case > ConstValuesAndUtility.AngleSnapRange:
                RotationAngle = -ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime;
                Inclination = Mathf.Clamp(Inclination + ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime, 0, ConstValuesAndUtility.MaximumInclination);
                break;
            case < -ConstValuesAndUtility.AngleSnapRange:
                RotationAngle = ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime;
                Inclination = Mathf.Clamp(Inclination + -ConstValuesAndUtility.RotationAnglePerSecond * Time.fixedDeltaTime, -ConstValuesAndUtility.MaximumInclination, 0);
                break;
            default:
                return;
        }
    }
}