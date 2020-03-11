using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//boxes appear and disappear
//as many boxes as tutorials


public class PopupManager : MonoBehaviour
{
    public GameObject Tut1;
    public GameObject Tut2;
    public GameObject Tut3;
    public GameObject Tut4;
    public GameObject Tut5;
    public GameObject Tut6;

    float timer = 15;

    // Start is called before the first frame update
    void Start()
    {
        Services.EventManager.Register<GameStart>(OnGameStart);
        Services.EventManager.Register<After30Seconds>(OnAfter30Seconds);
        Services.EventManager.Register<Day2>(OnDay2);
        Services.EventManager.Register<FirstTreePlanted>(OnFirstTreePlanted);
        Services.EventManager.Register<FirstTreeGrown>(OnFirstTreeGrown);
        Services.EventManager.Register<TooManyPlants>(OnTooManyPlants);
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
            Tut1.SetActive(false);
            Tut2.SetActive(false);
            Tut3.SetActive(false);
            Tut4.SetActive(false);
            Tut5.SetActive(false);
            Tut6.SetActive(false);

        }
    }

    void OnGameStart(AGPEvent e)
    {
        Debug.Log("STARTED");
        Tut1.SetActive(true);
        timer = 15;
        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    Tut1.SetActive(false);

        //}));
    }
    void OnAfter30Seconds(AGPEvent e)
    {
        Debug.Log("30s");

        Tut2.SetActive(true);
        timer = 7;

        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    Tut2.SetActive(false);

        //}));
    }
    void OnDay2(AGPEvent e)
    {
        Debug.Log("day2");

        Tut3.SetActive(true);
        timer = 15;

        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    Tut3.SetActive(false);

        //}));
    }
    void OnFirstTreePlanted(AGPEvent e)
    {
        Debug.Log("firsttree");

        Tut4.SetActive(true);
        timer = 15;

        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    Tut4.SetActive(false);

        //}));
    }
    void OnFirstTreeGrown(AGPEvent e)
    {
        Debug.Log("grown");

        Tut5.SetActive(true);
        timer = 15;

        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    yield return new WaitForSeconds(5);
        //    Tut5.SetActive(false);

        //}));
    }
    void OnTooManyPlants(AGPEvent e)
    {
        Debug.Log("toomany");

        Tut6.SetActive(true);
        timer = 15;

        //StartCoroutine(Coroutines.DoOverTime(10f, t =>
        //{
        //    Tut6.SetActive(false);

        //}));
    }
}
