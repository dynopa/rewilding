using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
//using Beat;

public class AudioManager : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string oncreateEvent = "event:/Dirt";
    public string ondestroyEvent = " ";
    public string onfedEvent = " ";
    public string grassS1Event = " ";
    public string grassS2Event = " ";

    //make an array of all event instances 
    //set it length to the length of all plants


    [FMODUnity.EventRef]
    public string onfeedEvent = "event:/Dirt";
    //FMOD.Studio.EventInstance plantFood;



    //public string onFedEvent = "event:"
    //FMOD.Studio.EventInstance creator;


    void SetEventInstance(string name)
    {

    }


    void Start()
    {
        Services.EventManager.Register<PlantGrown>(OnPlantGrownUp);
        Services.EventManager.Register<PlantCreated>(OnPlantCreated);
        Services.EventManager.Register<FadeOutComplete>(OnFadeOutComplete);

    }
    void OnDestroy()
    {
        Services.EventManager.Unregister<PlantGrown>(OnPlantGrownUp);
        Services.EventManager.Unregister<PlantCreated>(OnPlantCreated);
        Services.EventManager.Unregister<FadeOutComplete>(OnFadeOutComplete);

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

    void OnPlantFed(AGPEvent e)
    {
        var plantEvent = (PlantJustFed)e;
        Plant plant = plantEvent.plant;
        FMODUnity.RuntimeManager.PlayOneShot(ondestroyEvent, plant.position);
        UnityEngine.Debug.Log("REEEEEE");
        UnityEngine.Debug.Log("REEEEEE");

    }
    void OnFadeOutComplete(AGPEvent e)
    {
        foreach (Plant p in Services.PlantManager.plants)
        {
            if (p.plantFood.isValid())
            {
                p.shouldPlay = false;
                UnityEngine.Debug.Log(p.shouldPlay);
                p.plantFood.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                p.plantFood.release();
            }
        }
    }
    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }


   


    void Update()
    {
        foreach (Plant p in Services.PlantManager.plants)
        {
            if (p.shouldPlay == false) continue;
            
            if(p.GetState == 0)
            {
                // UnityEngine.Debug.Log(plantFood.isValid());
                //FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent, p.position);
                // UnityEngine.Debug.Log("FINE");
                //check the plant's type
                // Play a sound based on state
                //FMODUnity.RuntimeManager.PlayOneShot(oncreateEvent, p.position);
                //GetComponent<FMODUnity.StudioEventEmitter>().Play(oncreateEvent);
                // Play a sound based on state
                //FMODUnity.RuntimeManager.PlayOneShot()
                //plantFood.release();
                //plantFood.clearHandle();
                if (!p.plantFood.isValid())
                {
                    //UnityEngine.Debug.Log("FINE");
                    /*switch (p.type)
                    {
                        case (PlantType)0:
                            p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/t1_plink");
                            break;
                        case (PlantType)1:
                            p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/t2_plink");
                            break;
                        case (PlantType)2:
                            p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/t3_plink");
                            break;
                        case (PlantType)3:
                            p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/t4_plink");
                            break;
                        default:
                            break;
                    }*/
                    p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/all_plink");
                    p.plantFood.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(p.gameObject));
                    UnityEngine.Debug.Log(IsPlaying(p.plantFood));
                    p.plantFood.start();
                    UnityEngine.Debug.Log(IsPlaying(p.plantFood));
                }
                else
                {
                  // UnityEngine.Debug.Log("FINE");
                    FMOD.Studio.PLAYBACK_STATE playbackState;
                    p.plantFood.getPlaybackState(out playbackState);
                    if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                    {

                        //plantFood.stop();
                         p.plantFood.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                       // UnityEngine.Debug.Log("Made it to stop");
                    }
                }

                
                //plantFood.start();
               
               /* if(plantFood.isValid())
                {
                    FMOD.Studio.PLAYBACK_STATE playbackState;
                    plantFood.getPlaybackState(out playbackState);
                    if(playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                    {
                        plantFood.release();
                        plantFood.clearHandle();
                        //UnityEngine.Debug.Log("YO");
                    }
               }*/ 
                

            }
            if (p.GetState == 1)
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
                if (!p.plantFood.isValid())
                {
                    //UnityEngine.Debug.Log("FINE");
                    /*switch (p.type)
                    {
                        case (PlantType)0:
                            p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/withering");
                            break;
                        case (PlantType)1:
                            p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/withering");
                            break;
                        case (PlantType)2:
                            p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/withering");
                            break;
                        case (PlantType)3:
                            p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/withering");
                            break;
                        default:
                            break;
                    }*/
                    p.plantFood = FMODUnity.RuntimeManager.CreateInstance("event:/withering");
                    p.plantFood.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(p.gameObject));
                    UnityEngine.Debug.Log(IsPlaying(p.plantFood));
                    p.plantFood.start();
                    UnityEngine.Debug.Log(IsPlaying(p.plantFood));



                }
                else
                {
                    // UnityEngine.Debug.Log("FINE");
                    FMOD.Studio.PLAYBACK_STATE playbackState;
                    p.plantFood.getPlaybackState(out playbackState);
                    if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                    {

                        //plantFood.stop();
                        p.plantFood.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                        // UnityEngine.Debug.Log("Made it to stop");
                    }
                }
            }
            else
            {

               // p.plantFood.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
    }

}


                                                               