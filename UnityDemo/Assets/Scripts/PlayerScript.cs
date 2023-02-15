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

    private Rigidbody _rigidbody;
    private InputCommands _inputCommands;
    private float _barrelAngleMiddleGround;
    private float _barrelMaxAngle;
    private float _barrelMinAngle;
    private bool _playerCanShoot = true;
    private Coroutine _shootCooldownCoroutine;

    private void Start()
    {
        if (barrelMaxAngle < barrelMinAngle)
            (barrelMaxAngle, barrelMinAngle) = (barrelMinAngle, barrelMaxAngle);
        _barrelMaxAngle = barrelMaxAngle < 0 ? 360 + barrelMaxAngle : barrelMaxAngle;
        _barrelMinAngle = barrelMinAngle < 0 ? 360 + barrelMinAngle : barrelMinAngle;
        _barrelAngleMiddleGround = (Mathf.Max(_barrelMaxAngle, _barrelMinAngle) - Mathf.Min(_barrelMinAngle, _barrelMaxAngle)) / 2;
        _rigidbody = GetComponent<Rigidbody>();
        ConstValuesAndUtility.AddTag(transform, ConstValuesAndUtility.PlayerTag);
    }

    private void FixedUpdate()
    {
        _inputCommands = TcpClientScript.Instance.InputCommands;
        TankMovement();
        TurretRotation();
        BarrelRotation();
        Shoot();
    }

    private void TankMovement()
    {
        var forward = transform.right;
        var rotationSpeed = new Vector3(0, 0, 0);
        var acceleration = 0f;
        if (_inputCommands.MoveForward)
            acceleration += 10000f;
        if (_inputCommands.MoveBackward)
            acceleration -= 10000f;
        if (_inputCommands.MoveLeft)
            rotationSpeed += new Vector3(0, -90, 0);
        if (_inputCommands.MoveRight)
            rotationSpeed += new Vector3(0, 90, 0);
        var deltaRotation = Quaternion.Euler(rotationSpeed * Time.fixedDeltaTime);
        _rigidbody.AddForce(forward * acceleration, ForceMode.Force);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }

    private void TurretRotation()
    {
        var rotationSpeed = 0;
        if (_inputCommands.RotateLeft)
            rotationSpeed += -90;
        if (_inputCommands.RotateRight)
            rotationSpeed += 90;
        turret.transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }

    private void BarrelRotation()
    {
        var rotationSpeed = 0;
        if (_inputCommands.RotateUp)
            rotationSpeed += 30;
        if (_inputCommands.RotateDown)
            rotationSpeed += -30;
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
}