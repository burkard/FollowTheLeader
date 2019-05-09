using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderObject : MonoBehaviour
{
    [Tooltip("when objects reach this distance, they stop")]
    public float stayAwayDistance;

    [Tooltip("when objects are inside this range, they follow me")]
    public float startFollowingDistance;

    [Tooltip("Minimum travelled distance to record position, in meters")]
    public float moveThreshold; // the followers will keep it's own records

    void Start()
    {
        if (stayAwayDistance == 0f || startFollowingDistance == 0f || moveThreshold == 0f) Debug.LogWarning("If you don't set a startFollowingDistance (and stayAway) for " + this.gameObject.name + ", the followers will start to follow IMEDIATELLY! Is this what you want?");
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stayAwayDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, startFollowingDistance);
    }
}
