using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioScript : MonoBehaviour
{
    private void Start()
    {        
        ConstValuesAndUtility.AddTag(transform, ConstValuesAndUtility.ScenarioTag);
    }
}
