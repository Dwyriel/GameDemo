using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject cameraTurretAnchor;
    [SerializeField] private GameObject cameraBarrelAnchor;
    [SerializeField] private GameObject aimAtAnchor;
    [SerializeField] private float positionReductionWhenLookingUp = 2.65f;
    [SerializeField] private float positionReductionWhenLookingDown = 3f;

    private void Update()
    {
        var turretAnchorPos = cameraTurretAnchor.transform.position;
        var barrelAnchorPos = cameraBarrelAnchor.transform.position;
        var y = barrelAnchorPos.y - turretAnchorPos.y;
        y = turretAnchorPos.y + y / (y < 0 ? positionReductionWhenLookingUp : positionReductionWhenLookingDown);
        transform.position = new Vector3(turretAnchorPos.x, y, turretAnchorPos.z);
        transform.LookAt(aimAtAnchor.transform.position);
    }
}