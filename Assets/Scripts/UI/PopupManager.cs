using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
//boxes appear and disappear
//as many boxes as tutorials


public class PopupManager : MonoBehaviour
{
    public TextMeshProUGUI TutText;
    public Image tutorialBG;

    Tutorial[] tutorials = new Tutorial[6];

    float timer = 15;
    Tutorial activeTutorial;

    // Start is called before the first frame update
    void Start()
    {
        string path = "Assets/UI/Popup spreadsheets/Tutorials.tsv";
        string sheet = System.IO.File.ReadAllText(path);
        string[] lines = sheet.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split('\t');
            tutorials[i] = new Tutorial();
            tutorials[i].num = i;
            tutorials[i].tutID = columns[1];
            tutorials[i].tutText = columns[2];

        }
        Services.EventManager.Register<GameStart>(OnGameStart);
        Services.EventManager.Register<After30Seconds>(OnAfter30Seconds);
        Services.EventManager.Register<Day2>(OnDay2);
        Services.EventManager.Register<FirstTreePlanted>(OnFirstTreePlanted);
        Services.EventManager.Register<FirstTreeGrown>(OnFirstTreeGrown);
        Services.EventManager.Register<TooManyPlants>(OnTooManyPlants);
        Services.EventManager.Register<FadeOutComplete>(OnFadeOutComplete);

    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer-= Time.deltaTime;
        }
        else
        {
<<<<<<< HEAD
            DeactivateTutorial();
        }
=======
            TurnOff();

        }
    }
    void TurnOff(){
        Tut1.SetActive(false);
            Tut2.SetActive(false);
            Tut3.SetActive(false);
            Tut4.SetActive(false);
            Tut5.SetActive(false);
            Tut6.SetActive(false);
>>>>>>> f7bb12c5eba07dfc873d7bbede8733f96f283ed2
    }

    void OnGameStart(AGPEvent e)
    {
        Debug.Log("STARTED");
<<<<<<< HEAD
        ActivateTutorial(tutorials[0], 15);
=======
        TurnOff();
        Tut1.SetActive(true);
        timer = 15;
        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    Tut1.SetActive(false);

        //}));
>>>>>>> f7bb12c5eba07dfc873d7bbede8733f96f283ed2
    }
    void OnAfter30Seconds(AGPEvent e)
    {
        Debug.Log("30s");
<<<<<<< HEAD

        ActivateTutorial(tutorials[1], 7);
    }
    void OnFadeOutComplete(AGPEvent e)
    {
        if (activeTutorial.num == 1) DeactivateTutorial();
=======
        TurnOff();
        Tut2.SetActive(true);
        timer = 7;

        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    Tut2.SetActive(false);

        //}));
>>>>>>> f7bb12c5eba07dfc873d7bbede8733f96f283ed2
    }
    void OnDay2(AGPEvent e)
    {
        Debug.Log("day2");
<<<<<<< HEAD

        ActivateTutorial(tutorials[2], 15);
=======
        TurnOff();
        Tut3.SetActive(true);
        timer = 15;
>>>>>>> f7bb12c5eba07dfc873d7bbede8733f96f283ed2

    }
    void OnFirstTreePlanted(AGPEvent e)
    {
        Debug.Log("firsttree");
<<<<<<< HEAD

        ActivateTutorial(tutorials[3], 15);
=======
        TurnOff();
        Tut4.SetActive(true);
        timer = 15;

        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    Tut4.SetActive(false);
>>>>>>> f7bb12c5eba07dfc873d7bbede8733f96f283ed2

    }
    void OnFirstTreeGrown(AGPEvent e)
    {
        Debug.Log("grown");
<<<<<<< HEAD

        ActivateTutorial(tutorials[4], 15);
=======
        TurnOff();
        Tut5.SetActive(true);
        timer = 15;

        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    yield return new WaitForSeconds(5);
        //    Tut5.SetActive(false);
>>>>>>> f7bb12c5eba07dfc873d7bbede8733f96f283ed2

    }
    void OnTooManyPlants(AGPEvent e)
    {
        Debug.Log("toomany");
<<<<<<< HEAD

        ActivateTutorial(tutorials[5], 15);

    }
=======
        Tut6.SetActive(true);
        timer = 15;
>>>>>>> f7bb12c5eba07dfc873d7bbede8733f96f283ed2


    void ActivateTutorial(Tutorial tut, float timeSet)
    {
        TutText.text = tut.tutText.Replace("$", "\n");
        activeTutorial = tut;
        TutText.gameObject.SetActive(true);

        tutorialBG.gameObject.SetActive(true);

        timer = timeSet;
    }
    void DeactivateTutorial()
    {
        TutText.text = null;
        tutorialBG.gameObject.SetActive(false);
        TutText.gameObject.SetActive(false);
        timer = 0;
    }
}
[System.Serializable]
public class Tutorial
{
    public int num;
    public string tutID;
    public string tutText;
}