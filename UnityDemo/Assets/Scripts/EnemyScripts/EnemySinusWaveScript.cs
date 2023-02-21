using System.Collections.Generic;
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
        SetStartAttributes(GetComponentsInChildren<Renderer>(), Color.magenta);
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
        _generalDirection = (TargetPosition - transform.position).normalized;
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

    protected override void SetStartAttributes(IEnumerable<Renderer> renderers, Color color)
    {
        base.SetStartAttributes(renderers, color);
        _generalDirection = (TargetPosition - transform.position).normalized;
        _angleToTurn = ConstValuesAndUtility.RotationAnglePerSecond;
    }
}