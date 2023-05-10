using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Canvas menu;
    [SerializeField] GameObject credits;
    [SerializeField] Image fade;
    [SerializeField] AudioSource sound;

    float openPos = -790f;
    float closePos = -220f;

    bool menuOpen = false;

    void ToggleCredits()
    {
        menuOpen = !menuOpen;

        RectTransform rect = credits.GetComponent<RectTransform>();
        rect.localPosition = new Vector2(rect.localPosition.x,menuOpen ? closePos : openPos);
        credits.transform.Find("Button").Find("Label").localScale = new Vector3(4.5f,menuOpen ? 1.25f : -1.25f,1);
    }

    public void Transition()
    {
        StartCoroutine("FadeImage");
    }

    public void Close()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        credits.transform.Find("Button").GetComponent<Button>().onClick.AddListener(ToggleCredits);
    }

    IEnumerator FadeImage()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            fade.color = new Color(0, 0, 0, i);
            sound.volume = 0.1f - (i/10);
            yield return null;
        }

        yield return new WaitForSeconds(.25f);

        SceneManager.LoadScene("Level-1");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
