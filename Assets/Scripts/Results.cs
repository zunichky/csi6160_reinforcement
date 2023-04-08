using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results {

    public Results()
    {
        Debug.LogWarning("Test");
    }


}

public class EnvironmentInstance {

    public List<float> target = new List<float>();
    public int totalMovements = 0;
    public bool objectHit = false;
    public bool groundHit = false;
    public bool selfHit = false;
    
    public EnvironmentInstance()
    {

    }
}