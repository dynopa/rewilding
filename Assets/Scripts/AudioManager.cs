using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    void Start(){
        Services.EventManager.Register<PlantGrown>(OnPlantGrownUp);
    }
    void OnDestroy(){
        Services.EventManager.Unregister<PlantGrown>(OnPlantGrownUp)
    }
    void OnPlantGrownUp(AGPEvent e){
        
    }
}
