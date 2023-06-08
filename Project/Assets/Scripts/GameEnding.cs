using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject prompt;
    private GameObject promptObject;

    private float range = 1.5f;

    private int round;

    // Start is called before the first frame update
    void Start()
    {
        promptObject = Instantiate(prompt);
        promptObject.transform.SetParent(transform);
        promptObject.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {

        round = GameObject.Find("GameManager").GetComponent<RoundManager>().curRound;

        if(round == 6)
        {
            promptObject.SetActive(false);
            if(player.position.x < transform.position.x + range && player.position.x > transform.position.x - range)
            {
                promptObject.SetActive(true);
                if(Input.GetKeyDown(KeyCode.E))
                {
                    EndGame();
                }
            }
        }
    }

    void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
