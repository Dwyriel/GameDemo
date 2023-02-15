using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    #region Events

    public delegate void ShotFiredEventHandler();

    public event ShotFiredEventHandler ShotFiredEvent;

    #endregion

    [SerializeField] private GameObject turret;
    [SerializeField] private GameObject barrel;
    [SerializeField] private GameObject barrelTip;
    [SerializeField] private GameObject barrelBase;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField, Range(-360f, 360f)] private float barrelMaxAngle;
    [SerializeField, Range(-360f, 360f)] private float barrelMinAngle;
    [SerializeField, Min(0)] private float shootCooldown = .2f;
    [SerializeField, Min(0)] private float tankAcceleration = 10000f;
    [SerializeField, Min(0)] private float tankRotationSpeed = 90f;
    [SerializeField, Min(0)] private float turretRotationSpeed = 90f;
    [SerializeField, Min(0)] private float barrelRotationSpeed = 30f;

    private Rigidbody _rigidbody;
    private InputCommands _inputCommands;
    private bool _hasControl = true;
    private float _barrelAngleMiddleGround;
    private float _barrelMaxAngle;
    private float _barrelMinAngle;
    private bool _playerCanShoot = true;
    private Coroutine _shootCooldownCoroutine;

    private void Start()
    {
        CalculateCorrectAngles();
        _rigidbody = GetComponent<Rigidbody>();
        FindObjectOfType<GameSceneManager>().GameOverEvent += GameOverEvent;
        ConstValuesAndUtility.AddTag(transform, ConstValuesAndUtility.PlayerTag);
    }

    private void FixedUpdate()
    {
        if (!_hasControl)
            return;
        _inputCommands = TcpClientScript.Instance.InputCommands;
        TankMovement();
        TurretRotation();
        BarrelRotation();
        Shoot();
    }

    private void CalculateCorrectAngles()
    {
        if (barrelMaxAngle < barrelMinAngle)
            (barrelMaxAngle, barrelMinAngle) = (barrelMinAngle, barrelMaxAngle);
        _barrelMaxAngle = barrelMaxAngle < 0 ? 360 + barrelMaxAngle : barrelMaxAngle;
        _barrelMinAngle = barrelMinAngle < 0 ? 360 + barrelMinAngle : barrelMinAngle;
        _barrelAngleMiddleGround = (Mathf.Max(_barrelMaxAngle, _barrelMinAngle) - Mathf.Min(_barrelMinAngle, _barrelMaxAngle)) / 2;
    }

    private void TankMovement()
    {
        var forward = transform.right;
        var rotationDirectionAndSpeed = 0f;
        var acceleration = 0f;
        if (_inputCommands.MoveForward)
            acceleration += tankAcceleration;
        if (_inputCommands.MoveBackward)
            acceleration -= tankAcceleration;
        if (_inputCommands.MoveLeft)
            rotationDirectionAndSpeed -= tankRotationSpeed;
        if (_inputCommands.MoveRight)
            rotationDirectionAndSpeed += tankRotationSpeed;
        _rigidbody.AddForce(forward * acceleration, ForceMode.Force);
        _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(new Vector3(0, rotationDirectionAndSpeed * Time.fixedDeltaTime, 0)));
    }

    private void TurretRotation()
    {
        var rotationDirectionAndSpeed = 0f;
        if (_inputCommands.RotateLeft)
            rotationDirectionAndSpeed -= turretRotationSpeed;
        if (_inputCommands.RotateRight)
            rotationDirectionAndSpeed += turretRotationSpeed;
        turret.transform.Rotate(Vector3.up, rotationDirectionAndSpeed * Time.fixedDeltaTime, Space.Self);
    }

    private void BarrelRotation()
    {
        var rotationSpeed = 0f;
        if (_inputCommands.RotateUp)
            rotationSpeed += barrelRotationSpeed;
        if (_inputCommands.RotateDown)
            rotationSpeed -= barrelRotationSpeed;
        var eulerAngles = barrel.transform.localEulerAngles;
        eulerAngles.z += rotationSpeed * Time.fixedDeltaTime;
        if (eulerAngles.z < _barrelMinAngle && eulerAngles.z > _barrelAngleMiddleGround)
            eulerAngles.z = _barrelMinAngle;
        if (eulerAngles.z > _barrelMaxAngle && eulerAngles.z <= _barrelAngleMiddleGround)
            eulerAngles.z = _barrelMaxAngle;
        barrel.transform.localRotation = Quaternion.Euler(eulerAngles);
    }

    private void Shoot()
    {
        if (!_playerCanShoot || !_inputCommands.FireWeapon)
            return;
        var barrelTipPosition = barrelTip.transform.position;
        var barrelBasePosition = barrelBase.transform.position;
        Instantiate(bulletPrefab, barrelTipPosition, Quaternion.LookRotation(barrelTipPosition - barrelBasePosition));
        _playerCanShoot = false;
        ShotFiredEvent?.Invoke();
        if (_shootCooldownCoroutine != null)
            StopCoroutine(_shootCooldownCoroutine);
        _shootCooldownCoroutine = StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        _playerCanShoot = true;
    }

    private void GameOverEvent()
    {
        _hasControl = false;
    }
}