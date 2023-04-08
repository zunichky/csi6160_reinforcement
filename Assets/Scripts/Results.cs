using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class Results {
    public List<EnvironmentInstance> results = new List<EnvironmentInstance>();

    public Results()
    {
    }

}

public class EnvironmentInstance {

    public Vector3 target;
    public int totalMovements = 0;
    public bool objectHit = false;
    public bool groundHit = false;
    public bool selfHit = false;
    
    public EnvironmentInstance(Vector3 cur_target)
    {
        target = cur_target;
    }

    public void PrintResults()
    {
        Debug.LogWarning(target.x.ToString() + ',' + target.y.ToString() + ',' + target.z.ToString() + ',' 
            + totalMovements.ToString() + ',' + objectHit.ToString() + ',' + groundHit.ToString() + ',' + selfHit.ToString());
    }
    public void WriteResults(string path)
    {
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine("X,Y,Z,MovementCount,ObjectHit,GroundHit,SelfHit");
            }
        }
        using (StreamWriter writer = File.AppendText(path))
        {
            writer.WriteLine(target.x.ToString() + ',' + target.y.ToString() + ',' + target.z.ToString() + ','
            + totalMovements.ToString() + ',' + objectHit.ToString() + ',' + groundHit.ToString() + ',' + selfHit.ToString());
        }
    }
}