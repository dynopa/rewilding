using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    void Start(){
        Services.EventManager.Register<PlantGrown>(OnPlantGrownUp);
    }
    void OnDestroy(){
        Services.EventManager.Unregister<PlantGrown>(OnPlantGrownUp);
    }
    void OnPlantGrownUp(AGPEvent e){
     //Need To check if all FMOD get components work with decentralized eventsystem.
     //GetComponent<FMODUnity.StudioEvent.Emitter>().Play();
    }
}
