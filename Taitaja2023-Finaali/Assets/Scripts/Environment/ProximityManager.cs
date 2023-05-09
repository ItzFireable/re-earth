using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to manage player proximities (currently unused)
public class ProximityManager : MonoBehaviour
{
    // Properties for distance, player and target list
    [SerializeField] float distance;
    [SerializeField] Transform player;
    [SerializeField] Transform[] targetList;

    // Values for nearest target
    [SerializeField] Transform nearestObject;
    [SerializeField] bool isInRange = false;

    void Update()
    {
        foreach (Transform child in targetList)
        {
            Vector3 offset = child.position - player.position;
            float sqrLen = offset.sqrMagnitude;
            
            if (nearestObject)
            {
                Vector3 nearestOffset = nearestObject.position - player.position;
                float nearestSqlLen = offset.sqrMagnitude;

                nearestObject = sqrLen < nearestSqlLen ? child : nearestObject;
                isInRange = (nearestSqlLen < sqrLen ? nearestSqlLen : sqrLen) < distance * distance;
            }
            else
                nearestObject = child;
        }
    }
}
