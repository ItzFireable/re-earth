using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script to manage player proximities (currently unused)
public class ProximityManager : MonoBehaviour
{
    // Properties for distance, player and target list
    [SerializeField] float distance;
    [SerializeField] Transform player;
    [SerializeField] GameObject prompt;
    [SerializeField] Transform targetList;
    [SerializeField] AudioSource collectSource;

    // Values for nearest target
    [SerializeField] Transform nearestObject;
    [SerializeField] bool isInRange = false;
    float nearestSqlLen = 0;

    private float progress = 0;
    private float timeToCollect = 1f;

    List<GameObject> clearedEnemies = new List<GameObject>();

    IEnumerator ClearEnemy(GameObject enemy)
    {
        enemy.GetComponent<Animator>().SetTrigger("Death2");

        yield return new WaitForSeconds(10f);
        Destroy(enemy);
    }

    void Update()
    {
        foreach (Transform child in targetList)
        {

            if (clearedEnemies.Contains(child.gameObject)) continue;

            if (!child.transform.Find("Real Position").Find("Prompt"))
            {
                GameObject promptObject = Instantiate(prompt);
                promptObject.transform.localPosition = new Vector3(0,1,0);
                promptObject.SetActive(false);
                promptObject.name = "Prompt";
                
                promptObject.transform.SetParent(child.transform.Find("Real Position"), false);
            }
            else{
                if(child != nearestObject)
                {
                    child.transform.Find("Real Position").Find("Prompt").gameObject.SetActive(false);
                }
            }

            // Get the magnitude of the child
            Vector3 offset = child.Find("Real Position").position - player.position;
            float sqrLen = offset.sqrMagnitude;

            if (child.GetComponent<EnemyController>().isDead)
            {
                // If nearest object exists
                if (nearestObject)
                {
                    if (nearestObject.GetComponent<EnemyController>().isDead)
                    {
                        // Get the magnitude of the nearest object
                        Vector3 nearestOffset = nearestObject.Find("Real Position").position - player.position;
                        nearestSqlLen = nearestOffset.sqrMagnitude;

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

        // 💀
        if (nearestObject)
        {
            if (isInRange)
            {
                if (nearestObject.GetComponent<EnemyController>().isDead && nearestObject.Find("Real Position").Find("Prompt"))
                {
                    var prompt = nearestObject.Find("Real Position").Find("Prompt");
                    prompt.gameObject.SetActive(true);
                    prompt.localScale = new Vector3(nearestObject.localScale.x < 0 ? -0.5f : 0.5f,0.5f,0.5f);
                    prompt.position = new Vector3(nearestObject.Find("Real Position").position.x,nearestObject.Find("Real Position").position.y + 1,0);

                    if (Input.GetKey(KeyCode.E))
                    {
                        progress += Time.deltaTime;
                        // pls change this
                        prompt.GetComponentInChildren<Slider>().value = progress / timeToCollect;
                        if(progress >= timeToCollect)
                        {
                            player.GetComponent<PlayerController>().GainEnergy(10);
                            StartCoroutine(ClearEnemy(nearestObject.gameObject));

                            Destroy(nearestObject.Find("Real Position").Find("Prompt").gameObject);

                            clearedEnemies.Add(nearestObject.gameObject);
                            nearestObject = null;
                            progress = 0;

                            collectSource.Play(0);
                        }
                    }
                    else if(Input.GetKeyUp(KeyCode.E))
                    {
                        progress = 0;
                        prompt.GetComponentInChildren<Slider>().value = 0;
                    }
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
