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
        var rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                gameObject.AddComponent<EnemyHorizontalScript>();
                break;
            case 1:
                gameObject.AddComponent<EnemyCircularScript>();
                break;
            case 2:
                gameObject.AddComponent<EnemySinusWaveScript>();
                break;
        }
    }

    private void OnTriggerEnter(Collider colliderObject)
    {
        CollisionWithTag(colliderObject, ConstValuesAndUtility.BulletTag);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionWithTag(collision.collider, ConstValuesAndUtility.EnemyTag);
    }

    private void CollisionWithTag(Collider collisionCollider, string possibleTag)
    {
        if(!collisionCollider.CompareTag(possibleTag))
            return;
        if(_gotHit)
            return;
        _gotHit = true;
        EnemyDestroyedEvent?.Invoke();
        gameObject.SetActive(false);
    }
}