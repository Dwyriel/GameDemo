using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySinusWaveScript : EnemyMovementBase
{
    private enum CurrentState
    {
        AwayFromTarget,
        FinishRotating,
    }
    
    private CurrentState _currentState = CurrentState.AwayFromTarget;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
}
