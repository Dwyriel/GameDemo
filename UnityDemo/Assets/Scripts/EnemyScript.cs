using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private void Start()
    {
        ConstValuesAndUtility.AddTag(transform, ConstValuesAndUtility.EnemyTag);
    }

    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag(ConstValuesAndUtility.BulletTag))
            return;
        Destroy(gameObject);
    }
}
