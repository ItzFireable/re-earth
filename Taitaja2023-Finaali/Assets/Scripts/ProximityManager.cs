using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityManager : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] Transform player;
    [SerializeField] Transform proximityList;

    [SerializeField] bool isNear;

    void Start()
    {
        
    }

    void Update()
    {
        foreach (Transform child in proximityList)
        {
            Vector3 offset = child.position - player.position;
            float sqrLen = offset.sqrMagnitude;

            isNear = sqrLen < distance * distance;
        }
    }
}
