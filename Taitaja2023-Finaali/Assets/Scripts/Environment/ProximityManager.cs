using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to manage player proximities (currently unused)
public class ProximityManager : MonoBehaviour
{
    // Properties for distance, player and target list
    [SerializeField] float distance;
    [SerializeField] Transform player;
    [SerializeField] GameObject prompt;
    [SerializeField] Transform targetList;

    // Values for nearest target
    [SerializeField] Transform nearestObject;
    [SerializeField] bool isInRange = false;
    float nearestSqlLen = 0;

    void Update()
    {
        foreach (Transform child in targetList)
        {
            if (!child.transform.Find("Real Position").Find("Prompt"))
            {
                GameObject promptObject = Instantiate(prompt);
                promptObject.transform.localPosition = new Vector3(0,1,0);
                promptObject.SetActive(false);
                promptObject.name = "Prompt";
                
                promptObject.transform.parent = child.transform.Find("Real Position");
            }

            // Get the magnitude of the child
            Vector3 offset = child.position - player.position;
            float sqrLen = offset.sqrMagnitude;

            if (child.GetComponent<EnemyController>().isDead)
            {
                // If nearest object exists
                if (nearestObject)
                {
                    if (nearestObject.GetComponent<EnemyController>().isDead)
                    {
                        // Get the magnitude of the nearest object
                        Vector3 nearestOffset = nearestObject.position - player.position;
                        nearestSqlLen = offset.sqrMagnitude;

                        // Compare the magnitudes and update nearestobject accordingly
                        nearestObject = sqrLen < nearestSqlLen ? child : nearestObject;
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

        isInRange = nearestSqlLen < distance * distance;

        if (nearestObject)
        {
            if (isInRange)
            {
                if (nearestObject.GetComponent<EnemyController>().isDead && nearestObject.transform.Find("Real Position").Find("Prompt"))
                {
                    nearestObject.transform.Find("Real Position").Find("Prompt").gameObject.SetActive(true);
                    nearestObject.transform.Find("Real Position").Find("Prompt").localScale = new Vector3(nearestObject.transform.localScale.x < 0 ? -0.5f : 0.5f,0.5f,0.5f);
                    nearestObject.transform.Find("Real Position").Find("Prompt").position = new Vector3(nearestObject.transform.position.x,nearestObject.transform.position.y + 1,0);
                } 
            }
            else
            {
                if (nearestObject != null && nearestObject.transform.Find("Real Position").Find("Prompt"))
                {
                    nearestObject.transform.Find("Real Position").Find("Prompt").gameObject.SetActive(false);
                } 
            }
        }
    }
}
