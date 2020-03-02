using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public bool anyFade;
    public bool fadeOut;
    Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        anyFade = image.color.a > 0.05f;
        if(fadeOut){
            image.color = Color.Lerp(image.color,Color.black,0.08f);
            if(image.color.a >= 0.95f){
                fadeOut = false;
                Services.EventManager.Fire(new FadeOutComplete());
            }
        }else{
            image.color = Color.Lerp(image.color,Color.clear,0.08f);
        }
    }
}
