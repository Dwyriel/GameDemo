using UnityEngine;

public class EnemySinusWaveScript : EnemyMovementBase
{
    private enum CurrentState
    {
        AwayFromTarget,
        FinishRotating,
        SinusWaveFlight
    }

    private CurrentState _currentState = CurrentState.AwayFromTarget;
    private Vector3 _generalDirection;
    private float _angleToTurn;

    private void Start()
    {
        MapCenterPosition = GameObject.FindWithTag(ConstValuesAndUtility.MapCenterPointTag).transform.position;
        ComponentRigidbody = GetComponent<Rigidbody>();
        GenerateTargetPosition();
        _generalDirection = (TargetPosition - transform.position).normalized;
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
            case CurrentState.SinusWaveFlight:
                SinusWaveFlightUpdate();
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
        if (angleToTarget is > -2f and < 2f)
        {
            SwitchToSinusWaveFlightState();
            return;
        }
        CalculateRotationAndInclination(angleToTarget);
        var eulerAngles = transform.eulerAngles;
        ComponentRigidbody.MoveRotation(Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y + RotationAngle, Inclination)));
    }

    private void SinusWaveFlightUpdate()
    {
        if (DirectionToTarget.magnitude >= ConstValuesAndUtility.MaxAllowedDistanceToTarget)
        {
            SwitchToAwayFromTargetState();
            return;
        }
        var angleToTarget = Vector3.SignedAngle(_generalDirection, ForwardDirection, Vector3.up);
        _angleToTurn = angleToTarget switch
        {
            > 45 when _angleToTurn < 0 => ConstValuesAndUtility.RotationAnglePerSecond,
            < -45 when _angleToTurn > 0 => -ConstValuesAndUtility.RotationAnglePerSecond,
            _ => _angleToTurn
        };
        CalculateRotationAndInclination(_angleToTurn);
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

    private void SwitchToSinusWaveFlightState()
    {
        _currentState = CurrentState.SinusWaveFlight;
        _angleToTurn = ConstValuesAndUtility.RotationAnglePerSecond;
        SinusWaveFlightUpdate();
    }
}