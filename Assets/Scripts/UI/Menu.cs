using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI credits;
    public GameObject startText;
    public Button startButton;
    public Button creditsButton;
    public Button cr_prevButton;
    public Button cr_nextButton;
    public Button beginButton;
    public Button begin_backButton;

    // Start is called before the first frame update
    void Start()
    {
        creditsButton.onClick.AddListener(ShowCredits);
        cr_prevButton.onClick.AddListener(HideCredits);
        cr_nextButton.onClick.AddListener(NextPage);
        startButton.onClick.AddListener(ShowStart);
        beginButton.onClick.AddListener(LoadGame);
        begin_backButton.onClick.AddListener(HideStart);

    }

    void Update()
    {
        if (Input.GetAxis("Fire1") == 1)
        {
            //LoadGame();
        }


    }
    public void LoadGame()
    {
    	SceneManager.LoadScene("BetaLevel", LoadSceneMode.Single);
    }

    public void ShowStart()
    {
        startButton.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
        startText.gameObject.SetActive(true);
    }
    public void HideStart()
    {
        startButton.gameObject.SetActive(true);
        creditsButton.gameObject.SetActive(true);
        startText.gameObject.SetActive(false);
    }
    public void ShowCredits()
    {
        startButton.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
        credits.gameObject.SetActive(true);
    }

    public void HideCredits()
    {
        if (credits.pageToDisplay > 1)
        {
            credits.pageToDisplay--;
            cr_nextButton.gameObject.SetActive(true);
        }
        else
        {
            credits.pageToDisplay = 1;
            startButton.gameObject.SetActive(true);
            creditsButton.gameObject.SetActive(true);
            credits.gameObject.SetActive(false);
        }
    }

    public void NextPage()
    {
        if (credits.pageToDisplay < 3)
        credits.pageToDisplay+= 1;
        else
        {
            credits.pageToDisplay += 1;
            cr_nextButton.gameObject.SetActive(false);
        }
    }
}
