using UnityEngine;

public class EnemyCircularScript : EnemyMovementBase
{
    private enum CurrentState
    {
        AwayFromTarget,
        FinishRotating,
        FlyUntilCloseToTarget,
        Rotating
    }

    private CurrentState _currentState = CurrentState.AwayFromTarget;
    private float _rotationDirection; 

    private void Start()
    {
        MapCenterPosition = GameObject.FindWithTag(ConstValuesAndUtility.MapCenterPointTag).transform.position;
        ComponentRigidbody = GetComponent<Rigidbody>();
        GenerateTargetPosition();
        TargetPosition.y = transform.position.y;
        transform.position = TargetPosition;
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
            case CurrentState.FlyUntilCloseToTarget:
                FlyUntilCloseToTargetUpdate();
                break;
            case CurrentState.Rotating:
                RotatingUpdate();
                break;
            default:
                return;
        }
    }

    private void AwayFromTargetUpdate()
    {
        if (DirectionToTarget.magnitude < ConstValuesAndUtility.MaxAllowedDistanceToTarget)
        {
            SwitchToFinishRotatingState();
            return;
        }
        var angleToTarget = Vector3.SignedAngle(DirectionToTarget, ForwardDirection, Vector3.up);
        CalculateRotationAndInclination(angleToTarget);
        var eulerAngles = transform.eulerAngles;
        ComponentRigidbody.MoveRotation(Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + RotationAngle, Inclination)));
    }

    private void FinishRotatingUpdate()
    {
        var angleToTarget = Vector3.SignedAngle(DirectionToTarget, ForwardDirection, Vector3.up);
        if (angleToTarget is > -2f and < 2f || DirectionToTarget.magnitude < 50f)
        {
            SwitchToFlyUntilCloseToTargetState();
            return;
        }
        CalculateRotationAndInclination(angleToTarget);
        var eulerAngles = transform.eulerAngles;
        ComponentRigidbody.MoveRotation(Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + RotationAngle, Inclination)));
    }

    private void FlyUntilCloseToTargetUpdate()
    {
        switch (DirectionToTarget.magnitude)
        {
            case > ConstValuesAndUtility.MaxAllowedDistanceToTarget:
                SwitchToAwayFromTargetState();
                return;
            case < 50f:
                SwitchToRotatingState();
                return;
        }
        CalculateRotationAndInclination(0);
        var eulerAngles = transform.eulerAngles;
        ComponentRigidbody.MoveRotation(Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + RotationAngle, Inclination)));
    }

    private void RotatingUpdate()
    {
        if (DirectionToTarget.magnitude > ConstValuesAndUtility.MaxAllowedDistanceToTarget)
        {
            SwitchToAwayFromTargetState();
            return;
        }
        CalculateRotationAndInclination(_rotationDirection);
        var eulerAngles = transform.eulerAngles;
        ComponentRigidbody.MoveRotation(Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + RotationAngle / 2f, Inclination)));
    }

    private void SwitchToAwayFromTargetState()
    {
        _currentState = CurrentState.AwayFromTarget;
        GenerateTargetPosition();
        AwayFromTargetUpdate();
    }

    private void SwitchToFinishRotatingState()
    {
        _currentState = CurrentState.FinishRotating;
        FinishRotatingUpdate();
    }

    private void SwitchToFlyUntilCloseToTargetState()
    {
        _currentState = CurrentState.FlyUntilCloseToTarget;
        FlyUntilCloseToTargetUpdate();
    }

    private void SwitchToRotatingState()
    {
        _currentState = CurrentState.Rotating;
        _rotationDirection = Random.value > .5f ? ConstValuesAndUtility.RotationAnglePerSecond : -ConstValuesAndUtility.RotationAnglePerSecond;
        RotatingUpdate();
    }
}