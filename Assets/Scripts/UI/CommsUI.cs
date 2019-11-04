using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommsUI : MonoBehaviour
{

    public Image letter_w;
    public Image letter_a;
    public Image letter_s;
    public Image letter_d;
    public Image bg;
    public GameObject keys;
    public Text ticker;
    public Color textColor;
    public CanvasGroup chatLogGroup;
    public CanvasGroup CommsUIGroup;
    public KeyCode openComms;
    public KeyCode interactComms;
    bool isSpeaking = false;
    bool isFocusing = false;
    public SoundMaker sm;

    // Start is called before the first frame update
    void Start()
    {
        ticker.text = null;
        textColor = new Color(254, 207, 255);
        CommsUIGroup.alpha = 0;
        chatLogGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //ticker + playercomms code
        if (Input.GetMouseButton(1))
        {
            isSpeaking = true;
            // keys.SetActive(true);
            if (Input.GetKeyDown(KeyCode.W))
            {
                letter_w.color = Color.cyan;
                ticker.text += "w";
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                letter_w.color = textColor;
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                letter_a.color = Color.cyan;
                ticker.text += "a";
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                letter_a.color = textColor;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                letter_s.color = Color.cyan;
                ticker.text += "s";
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                letter_s.color = textColor;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                letter_d.color = Color.cyan;
                ticker.text += "d";
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                letter_d.color = textColor;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ticker.text += " ";
            }
        }
        else if (!Input.GetMouseButton(0))
        {
            isSpeaking = false;
        }
        //end playercomms

        //Comms UI Pops up
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(Coroutines.DoOverEasedTime(0.01f, Easing.Linear, t =>
            {
                CommsUIGroup.alpha = Mathf.Lerp(0, 1.5f, t);
                if (!isFocusing && chatLogGroup.alpha < 0.5f)
                {
                    chatLogGroup.alpha = Mathf.Lerp(0, 1.5f, t);
                }
            }));
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                CommsUIGroup.alpha = Mathf.Lerp(1, -1.5f, t);
                if (!isFocusing && chatLogGroup.alpha > 0.5f)
                {
                    chatLogGroup.alpha = Mathf.Lerp(1, -1.5f, t);
                }

            }));
            //sends message to language broadcast and chat ticker
            if (ticker.text.Length > 0)
            {
                if (PlayerController.instance.ih.holdingItem)
                {
                    Language.TakeMessage(ticker.text, sm, PlayerController.instance.ih.itemHeld.transform);
                }
                else
                {
                    Language.TakeMessage(ticker.text, sm);
                }
                ChatLog.instance.TakeMessage("You: " + ticker.text + "\n");
            }
            
            
            // Debug.Log("_" + ticker.text + "_");
            ticker.text = null;
            letter_w.color = textColor;
            letter_a.color = textColor;
            letter_s.color = textColor;
            letter_d.color = textColor;
        }
        //Focus UI
        if (Input.GetMouseButtonDown(0))
        {
            isFocusing = true;
            if (!isSpeaking)
            {
                StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
                {
                    chatLogGroup.alpha = Mathf.Lerp(0, 1.5f, t);
                }));
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            isFocusing = false;
            if (!isSpeaking)
            {
                StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
                {
                    chatLogGroup.alpha = Mathf.Lerp(1, -1.5f, t);
                }));
            }

            //keys.SetActive(false);
        }
    }
    IEnumerator KillAlpha()
    {
        yield return new WaitForSeconds(.2f);
        if (!Input.GetMouseButton(1))
        {
            chatLogGroup.alpha = 0;
            CommsUIGroup.alpha = 0;
        }
    }
}
