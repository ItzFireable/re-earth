using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{
    // General
    [SerializeField] Camera targetCamera;
    [SerializeField] GameObject parent;

    // UI
    [SerializeField] Canvas canvas;
    TMP_Text enemyLabel;

    // Prefabs for enemies
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    // Round stats
    int enemiesLeft = 0;
    [SerializeField] int enemiesForRound = 0;

    IEnumerator StartRound()
    {
        for (int i = 0; i < enemiesForRound; i++)
        {
            GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0,enemyPrefabs.Count)];

            Transform target = UnityEngine.Random.Range(1,3) == 1 ? targetCamera.transform.Find("Left") : targetCamera.transform.Find("Right");
            GameObject enemy = Instantiate(prefab, target.position, target.rotation);

            enemy.transform.parent = parent.transform;
            enemies.Add(enemy);

            enemiesLeft += 1;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Start()
    {
        enemyLabel = canvas.transform.Find("EnemyText").GetComponent<TMP_Text>();
        StartCoroutine("StartRound");
    }
    
    void Update()
    {
        enemyLabel.text = "Enemies left: " + enemiesLeft;
    }
}
