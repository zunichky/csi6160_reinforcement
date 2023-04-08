﻿using UnityEngine;

public class EndEffector : MonoBehaviour
{
    public RobotControllerAgent parentAgent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Components"))
        {
            if(parentAgent != null)
                parentAgent.JackpotReward(other);
        }
        else if (other.transform.CompareTag("Ground"))
        {
            Debug.LogWarning("Robot Controller penalty");

            if(parentAgent != null)
                parentAgent.GroundHitPenalty();
        }
    }
}
