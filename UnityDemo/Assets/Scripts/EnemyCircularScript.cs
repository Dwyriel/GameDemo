using UnityEngine;

public class EnemyCircularScript : MonoBehaviour
{
    private enum CurrentState
    {
        AwayFromTarget,
        FinishRotating,
        FlyUntilCloseToTarget,
        Rotating
    }

    private CurrentState _currentState = CurrentState.AwayFromTarget;
    private Rigidbody _rigidbody;
    private Vector3 _mapCenterPosition;
    private Vector3 _targetPosition;
    private Vector3 _currentPosition;
    private Vector3 _forwardDirection;
    private Vector3 _directionToTarget;
    private float _rotateAngle;

    private void Start()
    {
        _mapCenterPosition = GameObject.FindWithTag(ConstValuesAndUtility.MapCenterPointTag).transform.position;
        _rigidbody = GetComponent<Rigidbody>();
        GenerateTargetPosition();
    }

    private void FixedUpdate()
    {
        _currentPosition = transform.position;
        _forwardDirection = transform.forward;
        _rigidbody.MovePosition(_currentPosition + _forwardDirection * ConstValuesAndUtility.MovingUnitsPerSecond * Time.fixedDeltaTime);
        _directionToTarget = _targetPosition - _currentPosition;
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
        if (_directionToTarget.magnitude < ConstValuesAndUtility.MaxDistanceToMidAllowed)
            SwitchToFinishRotatingState();
        var angleToTarget = Vector3.SignedAngle(_directionToTarget, _forwardDirection, Vector3.up);
        if (angleToTarget is > -2f and < 2f)
            return;
        var angleToRotate = angleToTarget > 0 ? -ConstValuesAndUtility.RotationAnglePerSecond : ConstValuesAndUtility.RotationAnglePerSecond;
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(new Vector3(0, angleToRotate * Time.fixedDeltaTime, 0)));
    }

    private void FinishRotatingUpdate()
    {
        var angleToTarget = Vector3.SignedAngle(_directionToTarget, _forwardDirection, Vector3.up);
        if (angleToTarget is > -2f and < 2f)
            SwitchToFlyUntilCloseToTargetState();
        var angleToRotate = angleToTarget > 0 ? -ConstValuesAndUtility.RotationAnglePerSecond : ConstValuesAndUtility.RotationAnglePerSecond;
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(new Vector3(0, angleToRotate * Time.fixedDeltaTime, 0)));
    }

    private void FlyUntilCloseToTargetUpdate()
    {
        switch (_directionToTarget.magnitude)
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
        if (_directionToTarget.magnitude > ConstValuesAndUtility.MaxDistanceToMidAllowed)
            SwitchToAwayFromTargetState();
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(new Vector3(0, _rotateAngle / 2f * Time.fixedDeltaTime, 0)));
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
        _rotateAngle = Random.value > .5f ? ConstValuesAndUtility.RotationAnglePerSecond : -ConstValuesAndUtility.RotationAnglePerSecond;
    }

    private void GenerateTargetPosition()
    {
        const float range = ConstValuesAndUtility.RandomMoveTargetRange;
        _targetPosition = _mapCenterPosition + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));
    }
}