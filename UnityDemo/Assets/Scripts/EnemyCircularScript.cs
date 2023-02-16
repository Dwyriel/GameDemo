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
        if (DirectionToTarget.magnitude < ConstValuesAndUtility.MaxDistanceToMidAllowed)
            SwitchToFinishRotatingState();
        var angleToTarget = Vector3.SignedAngle(DirectionToTarget, ForwardDirection, Vector3.up);
        if (angleToTarget is > -2f and < 2f)
            return;
        var angleToRotate = angleToTarget > 0 ? -ConstValuesAndUtility.RotationAnglePerSecond : ConstValuesAndUtility.RotationAnglePerSecond;
        ComponentRigidbody.MoveRotation(ComponentRigidbody.rotation * Quaternion.Euler(new Vector3(0, angleToRotate * Time.fixedDeltaTime, 0)));
    }

    private void FinishRotatingUpdate()
    {
        var angleToTarget = Vector3.SignedAngle(DirectionToTarget, ForwardDirection, Vector3.up);
        if (angleToTarget is > -2f and < 2f)
            SwitchToFlyUntilCloseToTargetState();
        var angleToRotate = angleToTarget > 0 ? -ConstValuesAndUtility.RotationAnglePerSecond : ConstValuesAndUtility.RotationAnglePerSecond;
        ComponentRigidbody.MoveRotation(ComponentRigidbody.rotation * Quaternion.Euler(new Vector3(0, angleToRotate * Time.fixedDeltaTime, 0)));
    }

    private void FlyUntilCloseToTargetUpdate()
    {
        switch (DirectionToTarget.magnitude)
        {
            case > ConstValuesAndUtility.MaxDistanceToMidAllowed:
                SwitchToAwayFromTargetState();
                break;
            case < 10f:
                SwitchToRotatingState();
                break;
        }
    }

    private void RotatingUpdate()
    {
        if (DirectionToTarget.magnitude > ConstValuesAndUtility.MaxDistanceToMidAllowed)
            SwitchToAwayFromTargetState();
        ComponentRigidbody.MoveRotation(ComponentRigidbody.rotation * Quaternion.Euler(new Vector3(0, RotationAngle / 2f * Time.fixedDeltaTime, 0)));
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

    private void SwitchToFlyUntilCloseToTargetState()
    {
        _currentState = CurrentState.FlyUntilCloseToTarget;
    }

    private void SwitchToRotatingState()
    {
        _currentState = CurrentState.Rotating;
        RotationAngle = Random.value > .5f ? ConstValuesAndUtility.RotationAnglePerSecond : -ConstValuesAndUtility.RotationAnglePerSecond;
    }

    private void GenerateTargetPosition()
    {
        const float range = ConstValuesAndUtility.RandomMoveTargetRange;
        TargetPosition = MapCenterPosition + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
    }
}