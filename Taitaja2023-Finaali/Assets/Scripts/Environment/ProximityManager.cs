using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to manage player proximities (currently unused)
public class ProximityManager : MonoBehaviour
{
    // Properties for distance, player and target list
    [SerializeField] float distance;
    [SerializeField] Transform player;
    [SerializeField] Transform targetList;

    // Values for nearest target
    [SerializeField] Transform nearestObject;
    [SerializeField] bool isInRange = false;

    void Update()
    {
        foreach (Transform child in targetList)
        {
            // Get the magnitude of the child
            Vector3 offset = child.position - player.position;
            float sqrLen = offset.sqrMagnitude;

            if (!child.GetComponent<EnemyController>().isDead)
            {
                // If nearest object exists
                if (nearestObject)
                {
                    if (!nearestObject.GetComponent<EnemyController>().isDead)
                    {
                        // Get the magnitude of the nearest object
                        Vector3 nearestOffset = nearestObject.position - player.position;
                        float nearestSqlLen = offset.sqrMagnitude;

                        // Compare the magnitudes and update nearestobject accordingly
                        nearestObject = sqrLen < nearestSqlLen ? child : nearestObject;
                        isInRange = (nearestSqlLen < sqrLen ? nearestSqlLen : sqrLen) < distance * distance;
                    }
                    else
                    {
                        // Update nearest object to be current child
                        nearestObject = child;
                    }
                }
                else
                {
                    // Update nearest object to be current child
                    nearestObject = child;
                }
            }
        }
    }
}
