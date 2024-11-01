using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MovingPlatformWithGizmos : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints = new Transform[3];
    [SerializeField] private float speed = 1f;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float arrowSize = 0.5f;
    [SerializeField] private float arrowAmount = 1f;
    
    private int currentWaypointIndex = 0;
    float distance = 0.1f;
    public GameManager gameManager;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPosition) < distance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        other.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = gameManager.transform;
    }

    // Arrows
    private void DrawArrow(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float pathLength = Vector3.Distance(start, end);
        int arrowCount = Mathf.Max(1, Mathf.FloorToInt(pathLength * arrowAmount));

        for (float i = 0; i < arrowCount; i++)
        {
            float t = (i + 0.5f) / arrowCount;
            Vector3 arrowPos = Vector3.Lerp(start, end, t);

            // Arrow
            Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
            Vector3 arrowTip = arrowPos + direction * (arrowSize * 0.5f);
            Vector3 arrowBase = arrowPos - direction * (arrowSize * 0.5f);
            Vector3 arrowRight = arrowBase + right * (arrowSize * 0.25f);
            Vector3 arrowLeft = arrowBase - right * (arrowSize * 0.25f);

            Gizmos.DrawLine(arrowBase, arrowTip);
            Gizmos.DrawLine(arrowRight, arrowTip);
            Gizmos.DrawLine(arrowLeft, arrowTip);
        }
    }

    // Gizmo
    private void OnDrawGizmos()
    {
        if (!showGizmos || waypoints == null || waypoints.Length != 3)
            return;

        // Draw path
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.color = (i == currentWaypointIndex) ? Color.green : Color.red;

                // Draw spheres for waypoints
                Gizmos.DrawWireSphere(waypoints[i].position, 0.25f);

                // Waypoint number stuff
#if UNITY_EDITOR
                Vector3 textPosition = waypoints[i].position + Vector3.up * 0.5f;
                Handles.Label(textPosition, (i + 1).ToString(), new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = (i == currentWaypointIndex) ? Color.green : Color.white },
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                });
#endif

                // Draw lines between waypoints
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    DrawArrow(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }

        // Connect last point to first point
        if (waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
            DrawArrow(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }

        // Draw platform pos
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
