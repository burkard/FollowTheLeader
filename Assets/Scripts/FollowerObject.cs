using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerObject : MonoBehaviour
{
    public GameObject target;

    //[Tooltip("Seconds to wait before start moving")]
    //public float waitingTime;

    [Tooltip("Moving speed, in meters per second")]
    public float movingSpeed;
    [Tooltip("The higher the speed, the quicker they turn to the target")]
    public float turningSpeed;

    private Vector3 relativePos;
    private Quaternion rotation;

    private List<Vector3> desiredPositions;
    private bool isFollowing;

    private static float startFollowingSqrDistance, stayAwaySqrDistance, currentSqrDistance, sqrMoveThreshold;

    void Start()
    {
        if (target == null || movingSpeed <= 0f || turningSpeed == 0f) Debug.LogError("Error - please set target and speed for " + this.name);
        desiredPositions = new List<Vector3>();
        isFollowing = false;

        if (startFollowingSqrDistance == 0f) startFollowingSqrDistance = Vector3.SqrMagnitude(
                new Vector3(target.GetComponent<LeaderObject>().startFollowingDistance, 0f, 0f)
            );

        if (stayAwaySqrDistance == 0f) stayAwaySqrDistance = Vector3.SqrMagnitude(
                new Vector3(target.GetComponent<LeaderObject>().stayAwayDistance, 0f, 0f)
            );

        if (sqrMoveThreshold == 0f) sqrMoveThreshold = Vector3.SqrMagnitude(
                new Vector3(target.GetComponent<LeaderObject>().moveThreshold, 0f, 0f)
            );
    }

    void Update()
    {
        currentSqrDistance = Vector3.SqrMagnitude(target.transform.position - transform.position);
        if (currentSqrDistance <= startFollowingSqrDistance)
        {
            isFollowing = true;
            if (currentSqrDistance > stayAwaySqrDistance) MoveToTarget();
        }
        else
        {
            isFollowing = false;
        }

        if (isFollowing)
        {
            LookToTarget();
            RecordTargetPosition();
        }
        else
        {
            desiredPositions.Clear();
        }


    }

    void RecordTargetPosition()
    {
        // record target's position ONLY IF target has moved beyond the threshold:
        if (desiredPositions.Count == 0) desiredPositions.Add(target.transform.position);

        if (desiredPositions.Count > 0 && Vector3.SqrMagnitude(desiredPositions[desiredPositions.Count - 1] - target.transform.position) >= sqrMoveThreshold)
            desiredPositions.Add(target.transform.position);
    }

    void LookToTarget()
    {
        relativePos = target.transform.position - transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), turningSpeed * Time.deltaTime);
    }

    void MoveToTarget()
    {
        if (desiredPositions.Count > 0)
        {
            relativePos = desiredPositions[0] - transform.position;

            if (relativePos.sqrMagnitude <= 0.1f)
            {
                desiredPositions.RemoveAt(0);
            }
            
            transform.position += relativePos.normalized * movingSpeed * Time.deltaTime;
        }
    }
}
