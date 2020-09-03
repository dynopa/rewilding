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

    public Tutorial[] tutorials;
    Dictionary<string, int> tutDict;

    float timer = 15;
    Tutorial activeTutorial;

    public enum GrownCheck
    {
        thirdPlanted,
        energyOut,
        grassUnlock,
        flowerUnlock,
        bushUnlock,
        gotTree
    }
    GrownCheck check;
    int plantsCreated;
    int startSeed;
    int grassGrown;
    int flowersGrown;
    int bushesGrown;
    public TextMeshProUGUI counterText;
    public GameObject counter;
    int hasGivenUnlockTut;



    // Start is called before the first frame update
    void Start()
    {

        TextAsset path = Resources.Load("Tutorials") as TextAsset; //replace with resources.load

        //string sheet = System.IO.File.ReadAllText(path);
        string sheet = path.text;
        string[] lines = sheet.Split('\n');
        tutorials = new Tutorial[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split('\t');
            tutorials[i] = new Tutorial();
            tutorials[i].num = i;
            tutorials[i].tutID = columns[1];
            tutorials[i].tutText = columns[2];

        }

        tutDict = new Dictionary<string, int>();
        for (int i = 0; i < tutorials.Length; i++)
        {
            tutDict.Add(tutorials[i].tutID, i);
        }

        Services.EventManager.Register<GameStart>(OnGameStart);
        Services.EventManager.Register<After30Seconds>(OnAfter30Seconds);
        Services.EventManager.Register<Day2>(OnDay2);
        Services.EventManager.Register<FirstTreePlanted>(OnFirstTreePlanted);
        Services.EventManager.Register<FirstTreeGrown>(OnFirstTreeGrown);
        Services.EventManager.Register<TooManyPlants>(OnTooManyPlants);
        Services.EventManager.Register<FadeOutComplete>(OnFadeOutComplete);
        Services.EventManager.Register<RunOutOfPlants>(OnRunOut);
        Services.EventManager.Register<FirstSleep>(OnFirstSleep);
        Services.EventManager.Register<PlantCreated>(OnPlantCreated);
        Services.EventManager.Register<PlantGrown>(OnPlantGrown);

        check = GrownCheck.thirdPlanted;
        startSeed = PlayerController.instance.seedsLeft;
        hasGivenUnlockTut = 0;

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
        //Debug.Log("STARTED");
        ActivateTutorial(tutorials[tutDict["LookMovePlant"]], 15);
    }
    void OnAfter30Seconds(AGPEvent e)
    {
        //Debug.Log("30s");

    }
    void OnRunOut(AGPEvent e)
    {
        Debug.Log("RunOut");

        //ActivateTutorial(tutorials[1], 15);

    }
    void OnFirstSleep(AGPEvent e)
    {
        Debug.Log("firstsleep");
        DeactivateTutorial();
        ActivateTutorial(tutorials[3], 15);
        ActivateCounter("Grass grown: ", grassGrown, Services.GameController.unlockLevels[0]);
        check = GrownCheck.grassUnlock;

    }
    void OnFadeOutComplete(AGPEvent e)
    {

        hasGivenUnlockTut++;
        // if (activeTutorial.num == 1) DeactivateTutorial();
        //DeactivateTutorial();
        if (check == GrownCheck.flowerUnlock)
        {
            if (hasGivenUnlockTut == 1)
            {
                ActivateTutorial(tutorials[4], 7);
                Debug.Log("FLOWERS COUNTER");
                ActivateCounter("Flowers grown: ", flowersGrown, Services.GameController.unlockLevels[1]);
                //hasGivenUnlockTut = 0;
            }
            if (hasGivenUnlockTut == 4)
            {
                ActivateTutorial(tutorials[10], 10);
            }


            }
            if (check == GrownCheck.bushUnlock)
        {
            if (hasGivenUnlockTut == 1)
            {
                DeactivateTutorial();
                ActivateTutorial(tutorials[5], 7);
                ActivateCounter("Bushes grown: ", bushesGrown, Services.GameController.unlockLevels[2]);
                //hasGivenUnlockTut = 0;
            }
            
        }

        if (check == GrownCheck.grassUnlock) //WHEN YOU UNLOCK IN MORNING
        {
            if (grassGrown >= Services.GameController.unlockLevels[0])
            {
                Debug.Log("FLOWER MSG");
                DeactivateCounter();
                DeactivateTutorial();
                ActivateTutorial(tutorials[6], 15); //HEY YOU UNLOCKED FLOWERS
                check = GrownCheck.flowerUnlock;
                //hasGivenUnlockTut = 0;
            }
           
        }
    }
    void OnDay2(AGPEvent e)
    {
        Debug.Log("day2");

        Services.GameController.makeNarrativeEvents = true;
        //ActivateTutorial(tutorials[2], 15);

    }
    void OnFirstTreePlanted(AGPEvent e)
    {
        Debug.Log("firsttree");

        //ActivateTutorial(tutorials[3], 15);

    }
    void OnFirstTreeGrown(AGPEvent e)
    {
        //Debug.Log("grown");


    }
    void OnTooManyPlants(AGPEvent e)
    {
        //Debug.Log("toomany");


    }
    void OnPlantCreated(AGPEvent e)
    {

        plantsCreated++;
        if (plantsCreated > 2 && check == GrownCheck.thirdPlanted)
        {
            DeactivateTutorial();
            ActivateTutorial(tutorials[1], 15);
            ActivateCounter("Grass planted: ", plantsCreated, startSeed);
            check = GrownCheck.energyOut;
        }
        if (check == GrownCheck.energyOut)
        {
            ActivateCounter("Grass planted: ", plantsCreated, startSeed);
            if (plantsCreated >= startSeed)
            {
                DeactivateTutorial();
                DeactivateCounter();
                ActivateTutorial(tutorials[tutDict["FirstSleep"]], 7);
                check = GrownCheck.energyOut;
            }
        }
    }
    void OnPlantGrown(AGPEvent e)
    {
        var plantEvent = (PlantGrown)e;
        OldPlant plant = plantEvent.plant;

        if (plant.type == OldPlantType.Spread)
        {
            grassGrown++;
            if (check == GrownCheck.grassUnlock)
            {
                ActivateCounter("Grass grown: ", grassGrown, Services.GameController.unlockLevels[0]); //these update the counter
           
                if (grassGrown >= Services.GameController.unlockLevels[0])
                {
                    Debug.Log("FLOWER MSG");
      
                    DeactivateTutorial();
                    ActivateTutorial(tutorials[6], 20); //HEY YOU UNLOCKED FLOWERS
                    check = GrownCheck.flowerUnlock;
                    hasGivenUnlockTut = 0;
                }

            }
        }
        if (plant.type == OldPlantType.Grass)
        {
            flowersGrown++;
            if (check == GrownCheck.flowerUnlock)
            {
                ActivateCounter("Flowers grown: ", flowersGrown, Services.GameController.unlockLevels[1]);
                if (flowersGrown >= Services.GameController.unlockLevels[1])
                {
                    DeactivateCounter();
                    DeactivateTutorial();
                    ActivateTutorial(tutorials[7], 15);
                    check = GrownCheck.bushUnlock;
                    hasGivenUnlockTut = 0;
                }
            }
        }
        if (plant.type == OldPlantType.Shrub)
        {
            bushesGrown++;
            if (check == GrownCheck.bushUnlock)
            {
                ActivateCounter("Bushes grown: ", bushesGrown, Services.GameController.unlockLevels[2]);
                if (bushesGrown >= Services.GameController.unlockLevels[2])
                {
                    DeactivateCounter();
                    DeactivateTutorial();
                    ActivateTutorial(tutorials[8], 15);
                    check = GrownCheck.gotTree;
                    hasGivenUnlockTut = 0;
                }
            }
        }

       

    }
    

    public void ActivateTutorial(Tutorial tut, float timeSet)
    {
        Debug.Log(tut.num);
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

    void ActivateCounter(string typeOfPlant, int startAmt, int endAmt)
    {
        counter.SetActive(true);
        counterText.text = typeOfPlant + startAmt + "/" + endAmt;
    }
    public void DeactivateCounter()
    {
        counter.SetActive(false);
        counterText.text = "";
    }
}

[System.Serializable]
public class Tutorial
{
    public int num;
    public string tutID;
    public string tutText;
}