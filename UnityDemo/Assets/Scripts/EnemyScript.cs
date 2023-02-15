using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    #region Events

    public delegate void EnemyDestroyedEventHandler();

    public event EnemyDestroyedEventHandler EnemyDestroyedEvent;

    #endregion

    private bool _gotHit;

    private void Start()
    {
        ConstValuesAndUtility.AddTag(transform, ConstValuesAndUtility.EnemyTag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(ConstValuesAndUtility.BulletTag))
            return;
        if(_gotHit)
            return;
        _gotHit = true;
        EnemyDestroyedEvent?.Invoke();
        Destroy(gameObject);
    }
}