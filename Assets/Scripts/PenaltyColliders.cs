﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenaltyColliders : MonoBehaviour
{
    public RobotControllerAgent parentAgent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("RobotInternal"))
        {
            Debug.LogWarning("RobotInternal Collider");
            if(parentAgent != null)
                parentAgent.GroundHitPenalty();
        }
    }
}
