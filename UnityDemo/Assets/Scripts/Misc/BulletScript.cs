using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField, Min(0)] private float maxTimeBeforeDestruction = 10f;
    [SerializeField, Min(0)] private float bulletSpeed = 50f;
    
    private float _timeSinceInstantiation;
    private void Start()
    {
        ConstValuesAndUtility.AddTag(transform, ConstValuesAndUtility.BulletTag);
    }

    private void Update()
    {
        //note: if problems arise: save last pos and raycast in that direction with length being the travelled distanced between current pos and prev pos. if it hits anything, it means collision was ignored (by going too fast)
        _timeSinceInstantiation += Time.deltaTime;
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime, Space.Self);
        if (_timeSinceInstantiation > maxTimeBeforeDestruction)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(ConstValuesAndUtility.PlayerTag))
            return;
        Destroy(gameObject);
    }
}
