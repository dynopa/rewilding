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
            DeactivateTutorial();
        }
    }

    void OnGameStart(AGPEvent e)
    {
        Debug.Log("STARTED");
        ActivateTutorial(tutorials[0], 15);
    }
    void OnAfter30Seconds(AGPEvent e)
    {
        Debug.Log("30s");

        ActivateTutorial(tutorials[1], 7);
    }
    void OnFadeOutComplete(AGPEvent e)
    {
        if (activeTutorial.num == 1) DeactivateTutorial();
    }
    void OnDay2(AGPEvent e)
    {
        Debug.Log("day2");

        ActivateTutorial(tutorials[2], 15);

    }
    void OnFirstTreePlanted(AGPEvent e)
    {
        Debug.Log("firsttree");

        ActivateTutorial(tutorials[3], 15);

    }
    void OnFirstTreeGrown(AGPEvent e)
    {
        Debug.Log("grown");

        ActivateTutorial(tutorials[4], 15);

    }
    void OnTooManyPlants(AGPEvent e)
    {
        Debug.Log("toomany");

        ActivateTutorial(tutorials[5], 15);

    }


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