using UnityEngine;

public class ScenarioScript : MonoBehaviour
{
    private void Start()
    {        
        ConstValuesAndUtility.AddTag(transform, ConstValuesAndUtility.ScenarioTag);
    }
}
