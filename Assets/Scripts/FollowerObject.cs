using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WhatToLook{ target, nextPosition }

public class FollowerObject : MonoBehaviour
{
    public GameObject target;

    //[Tooltip("Seconds to wait before start moving")]
    //public float waitingTime;

    [Tooltip("Moving speed, in meters per second")]
    public float movingSpeed;
    [Tooltip("The higher the speed, the quicker they turn to the target")]
    public float turningSpeed;

    [Tooltip("The delay, in seconds, until it starts to follow the target")]
    public float startDelay = 0f;

    [Tooltip("Should I stop when reach the target, or follow the whole path?")]
    public bool stopWhenReachedTarget;

    [Header("Buffer size")]
    [Tooltip("The higher the number, the bigger the path will be - but more memory will be used! It will use the last X positions (the prior ones will be discarded)")]
    public int bufferSize = 100;

    [Header("What should I look at while I'm moving?")]
    public WhatToLook lookAt;

    private float startTime = 0f;

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
            RecordTargetPosition();

            if (!isFollowing) startTime = Time.time + startDelay;
            isFollowing = true;
            if (currentSqrDistance > stayAwaySqrDistance || !stopWhenReachedTarget) MoveToTarget();
            else isFollowing = false;
        }
        else
        {
            if (currentSqrDistance > stayAwaySqrDistance || !stopWhenReachedTarget) MoveToTarget();
        }

        if (isFollowing)
        {
            LookToTarget();
        }
        else
        {
            if (desiredPositions.Count > bufferSize) desiredPositions.RemoveAt(0);
        }
        if (desiredPositions.Count == 0) isFollowing = false;
    }

    void RecordTargetPosition()
    {
        // record target's position ONLY IF target has moved beyond its threshold:
        if (desiredPositions.Count == 0) desiredPositions.Add(target.transform.position);

        if (desiredPositions.Count > 0 && Vector3.SqrMagnitude(desiredPositions[desiredPositions.Count - 1] - target.transform.position) >= sqrMoveThreshold)
            desiredPositions.Add(target.transform.position);
    }

    void LookToTarget()
    {
        if (lookAt == WhatToLook.target || desiredPositions.Count == 0)
            relativePos = target.transform.position - transform.position;
        else
            relativePos = desiredPositions[0] - transform.position;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(relativePos, Vector3.up), turningSpeed * Time.deltaTime);
    }

    void MoveToTarget()
    {
        if (Time.time > startTime && desiredPositions.Count > 0)
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
