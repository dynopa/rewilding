using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class AudioManager : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string oncreateEvent = " ";
    public string ondestroyEvent = " ";


    void Start()
    {
        Services.EventManager.Register<PlantGrown>(OnPlantGrownUp);
    }
    void OnDestroy()
    {
        Services.EventManager.Unregister<PlantGrown>(OnPlantGrownUp);
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
        //var plantEvent = (PlantCreated)e;
        ///Plant plant = plantEvent.plant;
        //FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent, plant.position);
    }

    void onPlantDestroyed(AGPEvent e)
    {
        var plantEvent = (PlantDestroyed)e;
        Plant plant = plantEvent.plant;
        FMODUnity.RuntimeManager.PlayOneShot(ondestroyEvent, plant.position);

    }



}



