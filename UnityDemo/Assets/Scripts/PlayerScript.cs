using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private GameObject turret;
    [SerializeField] private GameObject barrel;
    [SerializeField, Range(-360f, 360f)] private float barrelMaxAngle;
    [SerializeField, Range(-360f, 360f)] private float barrelMinAngle;

    private Rigidbody _rigidbody;
    private float _barrelAngleMiddleGround;
    private float _barrelMaxAngle;
    private float _barrelMinAngle;

    private void Start()
    {
        if (barrelMaxAngle < barrelMinAngle)
            (barrelMaxAngle, barrelMinAngle) = (barrelMinAngle, barrelMaxAngle);
        _barrelMaxAngle = barrelMaxAngle < 0 ? 360 + barrelMaxAngle : barrelMaxAngle;
        _barrelMinAngle = barrelMinAngle < 0 ? 360 + barrelMinAngle : barrelMinAngle;
        _barrelAngleMiddleGround = (Mathf.Max(_barrelMaxAngle, _barrelMinAngle) - Mathf.Min(_barrelMinAngle, _barrelMaxAngle)) / 2;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        TankMovement();
        TurretRotation();
        BarrelRotation();
    }

    private void TankMovement()
    {
        var forward = transform.right;
        var rotationSpeed = new Vector3(0, 0, 0);
        var acceleration = 0f;
        if (Input.GetKey(KeyCode.W))
            acceleration += 10000f;
        if (Input.GetKey(KeyCode.S))
            acceleration -= 10000f;
        if (Input.GetKey(KeyCode.A))
            rotationSpeed += new Vector3(0, -90, 0);
        if (Input.GetKey(KeyCode.D))
            rotationSpeed += new Vector3(0, 90, 0);
        var deltaRotation = Quaternion.Euler(rotationSpeed * Time.fixedDeltaTime);
        _rigidbody.AddForce(forward * acceleration, ForceMode.Force);
        _rigidbody.MoveRotation(_rigidbody.rotation * deltaRotation);
    }

    private void TurretRotation()
    {
        var rotationSpeed = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
            rotationSpeed += -90;
        if (Input.GetKey(KeyCode.RightArrow))
            rotationSpeed += 90;
        turret.transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }

    private void BarrelRotation()
    {
        var rotationSpeed = 0;
        if (TcpClientScript.Instance.InputCommands.RotateUp || Input.GetKey(KeyCode.UpArrow))
            rotationSpeed += 30;
        if (TcpClientScript.Instance.InputCommands.RotateDown || Input.GetKey(KeyCode.DownArrow))
            rotationSpeed += -30;
        var eulerAngles = barrel.transform.localEulerAngles;
        eulerAngles.z += rotationSpeed * Time.fixedDeltaTime;
        if (eulerAngles.z < _barrelMinAngle && eulerAngles.z > _barrelAngleMiddleGround)
            eulerAngles.z = _barrelMinAngle;
        if (eulerAngles.z > _barrelMaxAngle && eulerAngles.z <= _barrelAngleMiddleGround)
            eulerAngles.z = _barrelMaxAngle;
        barrel.transform.localRotation = Quaternion.Euler(eulerAngles);
    }
}