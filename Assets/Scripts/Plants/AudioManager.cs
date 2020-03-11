using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class AudioManager : MonoBehaviour
{
    [FMODUnity.EventRef]
    string oncreateEvent1 = "event:/Plink3";
    [FMODUnity.EventRef]
    string oncreateEvent2 = "event:/Plink1";
    [FMODUnity.EventRef]
    string oncreateEvent3 = "event:/Plink1";

    [FMODUnity.EventRef]
    public string ondestroyEvent = " ";
    [FMODUnity.EventRef]
    public string onfedEvent = " ";
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
        //FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent, plant.position);



    }

    void OnPlantCreated(AGPEvent e)
    {
        var plantEvent = (PlantCreated)e;
        Plant plant = plantEvent.plant;
        var rand = Random.Range(1, 3);
        switch (rand)
        {
            case 1:
                FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent1, plant.position);
                break;
            case 2:
                FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent2, plant.position);
                break;
            case 3:
                FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent3, plant.position);
                break;
            default:
                //UnityEngine.Debug.log("RU ROH");
                break;


        }




        // if (plant.needsMetPercent >= 0.25f)
        //{
        //  FMODUnity.RuntimeManager.PlayOneShot(onfedEvent, plant.position);
        //}
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


}



