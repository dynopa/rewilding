using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using Beat;

public class AudioManager : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string oncreateEvent = "event:/Dirt";
    public string ondestroyEvent = " ";
    public string onfedEvent = " ";
    public string grassS1Event = " ";
    public string grassS2Event = " ";

 
  
    //public string onFedEvent = "event:"
    //FMOD.Studio.EventInstance creator;





    void Start()
    {
        Services.EventManager.Register<PlantGrown>(OnPlantGrownUp);
        Services.EventManager.Register<PlantCreated>(OnPlantCreated);
    }
    void OnDestroy()
    {
        Services.EventManager.Unregister<PlantGrown>(OnPlantGrownUp);
        Services.EventManager.Unregister<PlantCreated>(OnPlantCreated);
    }
    void OnPlantGrownUp(AGPEvent e)
    {
        //Need To check if all FMOD get components work with decentralized eventsystem.
        //GetComponent<FMODUnity.StudioEvent.Emitter>().Play();
        var plantEvent = (PlantGrown)e;
        Plant plant = plantEvent.plant;
        FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent, plant.position);



    }

    void OnPlantCreated(AGPEvent e)
    {
        var plantEvent = (PlantCreated)e;
        Plant plant = plantEvent.plant;
        //Play creation sound
        FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent, plant.position);
    }

    void onPlantDestroyed(AGPEvent e)
    {
        var plantEvent = (PlantDestroyed)e;
        Plant plant = plantEvent.plant;
        FMODUnity.RuntimeManager.PlayOneShot(ondestroyEvent, plant.position);

    }

    void onPlantFed(AGPEvent e)
    {
        var plantEvent = (PlantJustFed)e;
        Plant plant = plantEvent.plant;
        FMODUnity.RuntimeManager.PlayOneShot(ondestroyEvent, plant.position);
    }


    void Update()
    {
        foreach (Plant p in Services.PlantManager.plants)
        {
            if (p.GetState == 0)
            {
                //FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent, p.position);
                //UnityEngine.Debug.Log("FINE");
                //check the plant's type
                // Play a sound based on state
               
                // Play a sound based on state
                //FMODUnity.RuntimeManager.PlayOneShot()
            }
            else if (p.GetState == 1)
            {
                //UnityEngine.Debug.Log("Alright");
            }
            else if (p.GetState == 2)
            {
               // UnityEngine.Debug.Log("Fix it or i die kinda soon");
            }
            else if (p.GetState == 3)
            {
                //UnityEngine.Debug.Log("Dying soon");
            }
        }
    }

}



