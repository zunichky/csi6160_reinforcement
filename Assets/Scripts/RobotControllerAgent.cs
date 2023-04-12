using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class RobotControllerAgent : Agent
{
   [SerializeField]
   private GameObject[] armAxes;
   [SerializeField] 
   private GameObject endEffector;
    [SerializeField]
    private int numberOfRuns;

   public float rotateTune = 20f;
   public bool trainingMode;
   public bool writeLog = true;
   private bool inFrontOfComponent = false;
   public GameObject nearestComponent;
   private Ray rayToTest = new Ray();
   private float[] angles = new float[5];
   private KinematicsCalculator calculator;
   private float beginDistance;
   private float prevBest;
   bool  inrange2 = false, inrange3 = false, inrange4 = false, inrange5 = false;
   private float baseAngle;
   private const float stepPenalty = -0.0001f;
    private int runCount = 0;
    private string resultsFile = "results.csv";
   private bool rotateCompelete = true;
   private Quaternion[] pendingRotations = new Quaternion[5];

   public Results results = new Results();
    public EnvironmentInstance cur_run = null;
   private void Start()
   {
      
   }

   public override void Initialize()
   {
      ResetAllAxis();
      MoveToSafeRandomPosition();
   }

   private void ResetAllAxis()
   {
      armAxes.All(c =>
      {
         c.transform.localRotation =  Quaternion.Euler(0f, 0f, 0f);
         return true;
      });
   }

   public override void OnEpisodeBegin()
   {
      rotateCompelete = true;

      //if(trainingMode)
      ResetAllAxis();

      MoveToSafeRandomPosition();
      UpdateNearestComponent();
   }

   private void UpdateNearestComponent()
   {
         if (!trainingMode)
         {
            if (cur_run != null)
            {
                  // Only log results if we don't collide after the first couple movements
                  if (cur_run.totalMovements > 3)
                  {
                     if (cur_run.totalMovements >= (MaxStep))
                     {
                        cur_run.maxSteps = true;
                     }
                     results.results.Add(cur_run);
                     cur_run.PrintResults();
                     if (writeLog)
                     {
                        cur_run.WriteResults(resultsFile);
                     }
                     runCount += 1;
               }

            }
            cur_run = new EnvironmentInstance(nearestComponent.transform.position);
         }
        if (!trainingMode)
        {
            if (runCount >= numberOfRuns)
            {
                endRun();
            }
        }

      if (trainingMode)
      {
         inFrontOfComponent = UnityEngine.Random.value > 0.5f;
      }
      if(!inFrontOfComponent)
         nearestComponent.transform.position = transform.position + new Vector3(Random.Range(0.3f,0.6f),Random.Range(0.1f,0.3f), Random.Range(0.3f,0.6f));
      else
      {
         nearestComponent.transform.position = endEffector.transform.TransformPoint(Vector3.zero) + new Vector3(Random.Range(0.01f,0.15f),Random.Range(0.01f,0.15f), Random.Range(0.01f,0.15f));
      }
      beginDistance = Vector3.Distance(endEffector.transform.TransformPoint(Vector3.zero), nearestComponent.transform.position);
      prevBest = beginDistance;
      
      baseAngle = Mathf.Atan2( transform.position.x - nearestComponent.transform.position.x, transform.position.z - nearestComponent.transform.position.z) * Mathf.Rad2Deg;
      if (baseAngle < 0) baseAngle = baseAngle + 360f;
   }

    private void endRun()
    {
        #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }
    /// <summary>
    /// Markov Decision Process - Observes state for the current time step
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
   {
      sensor.AddObservation(angles); //5
      sensor.AddObservation(transform.position.normalized); //3
      sensor.AddObservation(nearestComponent.transform.position.normalized); //3
      sensor.AddObservation(endEffector.transform.TransformPoint(Vector3.zero).normalized); //3
      Vector3 toComponent = (nearestComponent.transform.position - endEffector.transform.TransformPoint(Vector3.zero));
      sensor.AddObservation(toComponent.normalized); //3
      sensor.AddObservation(Vector3.Distance(nearestComponent.transform.position,endEffector.transform.TransformPoint(Vector3.zero))); //1
      sensor.AddObservation(StepCount / MaxStep); //1
   }

   public override void OnActionReceived(float[] vectorAction)
   {
      angles = vectorAction;
      /*
      // Translate the floating point actions into Degrees of rotation for each axis
      armAxes[0].transform.localRotation =
         Quaternion.AngleAxis(angles[0] * 180f, armAxes[0].GetComponent<Axis>().rotationAxis);
      armAxes[1].transform.localRotation =
         Quaternion.AngleAxis(angles[1] * 90f, armAxes[1].GetComponent<Axis>().rotationAxis);
      armAxes[2].transform.localRotation =
         Quaternion.AngleAxis(angles[2] * 180f, armAxes[2].GetComponent<Axis>().rotationAxis);
      armAxes[3].transform.localRotation =
         Quaternion.AngleAxis(angles[3] * 90f, armAxes[3].GetComponent<Axis>().rotationAxis);
      armAxes[4].transform.localRotation =
         Quaternion.AngleAxis(angles[4] * 90f, armAxes[4].GetComponent<Axis>().rotationAxis);
      */
      armAxes[0].transform.localRotation = Quaternion.RotateTowards(armAxes[0].transform.localRotation,  
      Quaternion.AngleAxis(angles[0] * 180f, armAxes[0].GetComponent<Axis>().rotationAxis), rotateTune * Time.deltaTime);
      armAxes[1].transform.localRotation = Quaternion.RotateTowards(armAxes[1].transform.localRotation,  
         Quaternion.AngleAxis(angles[1] * 90f, armAxes[1].GetComponent<Axis>().rotationAxis), rotateTune * Time.deltaTime);
      armAxes[2].transform.localRotation = Quaternion.RotateTowards(armAxes[2].transform.localRotation,  
         Quaternion.AngleAxis(angles[2] * 180f, armAxes[2].GetComponent<Axis>().rotationAxis), rotateTune * Time.deltaTime);   
      armAxes[3].transform.localRotation = Quaternion.RotateTowards(armAxes[3].transform.localRotation,  
         Quaternion.AngleAxis(angles[3] * 90f, armAxes[3].GetComponent<Axis>().rotationAxis), rotateTune * Time.deltaTime);
      armAxes[4].transform.localRotation = Quaternion.RotateTowards(armAxes[4].transform.localRotation,  
         Quaternion.AngleAxis(angles[4] * 90f, armAxes[4].GetComponent<Axis>().rotationAxis), rotateTune * Time.deltaTime);

      if (trainingMode)
      {
         float distance = Vector3.Distance(endEffector.transform.TransformPoint(Vector3.zero),
            nearestComponent.transform.position);
         float diff = beginDistance - distance;
         
         if (distance > prevBest)
         {
            // Penalty if the arm moves away from the closest position to target
            AddReward(prevBest - distance);
         }
         else
         {
            // Reward if the arm moves closer to target
            AddReward(diff);
            prevBest = distance;
         }
         //AddReward(stepPenalty);
      }
      else
      {
         cur_run.totalMovements += 1;
      }
   }

   public void GroundHitPenalty()
   {
      //Debug.LogWarning("Ground hit penalty");
      if (!trainingMode)
      {
         cur_run.groundHit = true;
      }
      
      AddReward(-1f);
      EndEpisode();
   }
    public void SelfHitPenalty()
    {
      //Debug.LogWarning("Self hit penalty");
      if (!trainingMode)
      {
         cur_run.selfHit = true;
      }

      AddReward(-1f);
      EndEpisode();
    }

    private void OnTriggerEnter(Collider other)
   {
      JackpotReward(other);
   }

   public void JackpotReward(Collider other)
   {
      if (other.transform.CompareTag("Components"))
      {
         float SuccessReward = 0.5f;
         float bonus = Mathf.Clamp01(Vector3.Dot(nearestComponent.transform.up.normalized,
                          endEffector.transform.up.normalized));
         float reward = SuccessReward + bonus;
         if (float.IsInfinity(reward) || float.IsNaN(reward)) return;
          Debug.LogWarning("Great! Component reached. Positive reward:" + reward );
        if (trainingMode)
        {
            AddReward(reward);
        }
        else
        {
            cur_run.objectHit = true;
        }
         if (trainingMode)
            EndEpisode();
         else
            UpdateNearestComponent();
      }
   }
   

   // private float[] NormalizedAngles()
   // {
   //    float[] normalized = new float[6];
   //    for (int i = 0; i < 6; i++)
   //    {
   //       normalized[i] = angles[i] / 360f;
   //    }
   //
   //    return normalized;
   // }

   private void MoveToSafeRandomPosition()
   {
      int maxTries = 100;
      
      while (maxTries > 0)
      {
         armAxes.All(axis =>
            {
               Axis ax = axis.GetComponent<Axis>();
               Vector3 angle = ax.rotationAxis * Random.Range(ax.MinAngle, ax.MaxAngle);
               ax.transform.localRotation = Quaternion.Euler(angle.x, angle.y, angle.z);
               return true;
            }
         );
         Vector3 tipPosition = endEffector.transform.TransformPoint(Vector3.zero);
         Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
         float distanceFromGround = groundPlane.GetDistanceToPoint(tipPosition);
         if (distanceFromGround > 0.1f && distanceFromGround <= 1f && tipPosition.y > 0.01f)
         {
            break;
         }
         maxTries--;
      }
   }
   private void Update()
   {
      if(nearestComponent != null)
         Debug.DrawLine(endEffector.transform.position,nearestComponent.transform.position, Color.green);
   }
}
