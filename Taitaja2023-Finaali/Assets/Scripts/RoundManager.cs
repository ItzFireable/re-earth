using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class RoundManager : MonoBehaviour
{
    // General
    [SerializeField] Camera targetCamera;
    [SerializeField] GameObject player;
    [SerializeField] GameObject parent;

    // UI
    [SerializeField] Canvas canvas;
    TMP_Text enemyLabel;
    TMP_Text roundLabel;

    // Prefabs for enemies
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> enemies = new List<GameObject>();

    // Round stats
    int curRound = 0;
    bool spawning = false;
    [SerializeField] int enemiesForRound = 0;

    [SerializeField] private GameObject cameraBoundaries;
    private float minX, maxX;

    public void setKill(GameObject enemy)
    {
        for (int i = 0; i < enemies.Count; i++)
            if (enemies[i] == enemy) 
                enemies.Remove(enemies[i]);
    }

    IEnumerator StartRound()
    {
        for (int i = 0; i < enemiesForRound; i++)
        {
            GameObject prefab = enemyPrefabs[UnityEngine.Random.Range(0,enemyPrefabs.Count)];

            Transform target = UnityEngine.Random.Range(1,3) == 1 ? targetCamera.transform.Find("Left") : targetCamera.transform.Find("Right");
            if(maxX < target.position.x)
            {
                target = targetCamera.transform.Find("Left");
            }
            else if(minX > target.position.x)
            {
                target = targetCamera.transform.Find("Right");
            }

            GameObject enemy = Instantiate(prefab, new Vector3(target.position.x, player.transform.position.y, 0), target.rotation);

            enemy.GetComponent<EnemyController>().roundManager = this;
            enemy.transform.parent = parent.transform;
            enemies.Add(enemy);

            yield return new WaitForSeconds(1f);
        }

        spawning = false;
    }

    void Start()
    {
        enemyLabel = canvas.transform.Find("EnemyText").GetComponent<TMP_Text>();
        roundLabel = canvas.transform.Find("RoundText").GetComponent<TMP_Text>();

        foreach (var i in cameraBoundaries.GetComponent<PolygonCollider2D>().points)
        {
            if (i.x < minX) 
                minX = i.x;
            if (i.x > maxX)
                maxX = i.x;
        }
    }
    
    void Update()
    {
        enemyLabel.text = "Enemies left: " + enemies.Count;
        roundLabel.text = "Round " + curRound;

        if (enemies.Count <= 0 && !spawning)
        {
            spawning = true;
            curRound += 1;

            if (curRound > 1)
                enemiesForRound += 1;

            if (curRound > 5)
                print("end game");
            else
                StartCoroutine("StartRound");
        }
    }
}
