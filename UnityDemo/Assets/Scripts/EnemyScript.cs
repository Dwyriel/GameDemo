using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    #region Events

    public delegate void EnemyDestroyedEventHandler();

    public event EnemyDestroyedEventHandler EnemyDestroyedEvent;

    #endregion

    private void Start()
    {
        ConstValuesAndUtility.AddTag(transform, ConstValuesAndUtility.EnemyTag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(ConstValuesAndUtility.BulletTag))
            return;
        EnemyDestroyedEvent?.Invoke();
        Destroy(gameObject);
    }
}