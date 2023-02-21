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
        SetStartAttributes(GetComponentsInChildren<Renderer>(), new Color(1f, .4f, 0f)); //orange
    }

    private void FixedUpdate()
    {
        UpdateAttributes();
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
}