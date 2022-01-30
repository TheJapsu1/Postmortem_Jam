using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessRotator : MonoBehaviour
{
    public float targetSpeed = 35f;

    public float startUpTime = 5;
    
    public AnimationCurve startUpCurve;

    private float speedModifier;

    private void Update()
    {
        speedModifier = startUpCurve.Evaluate(Time.time / startUpTime);
        
        transform.Rotate(Vector3.forward, Time.deltaTime * targetSpeed * speedModifier);
    }
}
