using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class AudioManager : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string oncreateEvent = "event:/Plant";
    public string ondestroyEvent = " ";
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
        FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent, plant.position);
    }

    void onPlantDestroyed(AGPEvent e)
    {
        var plantEvent = (PlantDestroyed)e;
        Plant plant = plantEvent.plant;
        FMODUnity.RuntimeManager.PlayOneShot(ondestroyEvent, plant.position);

    }



}



