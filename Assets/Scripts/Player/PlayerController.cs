using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using TMPro;


public class PlayerController : MonoBehaviour
{

    public int seedPerDay;
    public int seedGainPerDay;
    public static PlayerController instance;
    private PlayerState p_state;
    private PlayerSc p_sc;
    public bool[] canAccessPlant = new bool[] { true, false, false, false };
    public int dayNum;
    enum oxygenState
    {
        full, draining, low, none
    }
    enum playerState
    {
        idle,walking,running,planting
    }
    oxygenState oState;
    public bool oxygenOn;
    //public states
    public bool running;
    public bool isWalking;
    //public data
    public float walkSpeed;
    public float runSpeed;
    public float jumpSpeed;
    public float talkSpeed = 0.1f; //multiplies run/walk speed while talking
    public float slowSpeed = 0.5f; //multiplies time.timeScale
    public FadeOut fadeOut;
    float fov_default = 47;
    float fov_comms = 57;
    float fov_focus = 42;

    //testing data
    public float verticalPos;
    public float horizontalPos;
    public bool isSpeaking;
    bool isFocusing;

    public int seedsLeft;
    //situation data
    [HideInInspector]


    //private data
    public Rigidbody rb;
    Vector3 moveDirection;
    CapsuleCollider collider;
    Camera cam;

    Vector3 itemHeldOffset;
    Vector3 groundContactNormal = Vector3.up;//the slope of whatever you're standing on
    LayerMask layerGround;
    MouseLook mouseLook;
    Vector2 mousePos;

    //raycast data
    public GameObject holePrefab;

    public PlantType type;

    //UI Data
    public GameObject economyUI;
    public GameObject hud;
    public GameObject crosshair;
    public Image whichSeed;
    public TextMeshProUGUI seedCost;
    public Sprite[] seedImages;
    public TextMeshProUGUI dayText;

    public TextMeshProUGUI seedCounter;
    public Image seedCounterImage;

    public float maxOxygen;
    public float oxygen;
    public Image oxygenDisplay;
    public Image cursor;

    //    None,Spread,Grass,Shrub,Tree,Special,Delete
    List<float> plantCount = new List<float> { 0, 8, 4, 2, 1 };
    List<float> plantMaxCount = new List<float> { 0, 8, 4, 2, 1 };
    List<int> plantVal = new List<int> { 0, 5, 7, 10, 25 };
    List<Image> plantSprite = new List<Image> { };



    //int mossVal = 5;
    //int grassVal = 7;
    //int shrubVal = 10;
    //int treeVal = 25;
    //int specialVal = 15;
    Vector3 spawnPosition;
    int[] uiPositions = new int[] { -63, -168, -273, -378 };
    bool holdingRightTrigger;
    bool holdingLeftTrigger;
    public bool holdingA;
    bool releasedA;
    bool holdingB;
    bool clickedOnce;
    ParticleSystem ps;
    public Material narrClearMat;
    public GameObject digFX;

    //squat variables
    bool squatComplete = false;
    Vector3 plantTargetPos;
    Vector3 startCamPos = new Vector3(0f, 0.45f, 0f);
    Vector3 endCamPos = new Vector3(0f, 0.1f, 0f);

    bool sentOutOfGoopMessage;
    bool sleptFirstTime;

    //soundStuff
    [FMODUnity.EventRef]
    public FMOD.Studio.EventInstance doorOpenS;
    public FMOD.Studio.EventInstance powerDownS;
    public FMOD.Studio.EventInstance oDrainS;
    public FMOD.Studio.EventInstance uiSwitchS;
    public FMOD.Studio.EventInstance lowOS;
    public FMOD.Studio.EventInstance VOS;
    public FMOD.Studio.EventInstance unplantS;
    //public FMOD.Studio.EventInstance[] bgmS;

    void Awake()
    {
        sentOutOfGoopMessage = false;
        spawnPosition = transform.position;
        type = PlantType.Spread;
        mousePos = Input.mousePosition;
        instance = this;
        p_state = GetComponent<PlayerState>();
        p_sc = GetComponent<PlayerSc>();
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        cam = Camera.main;
        layerGround = LayerMask.NameToLayer("Ground");
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook = cam.gameObject.GetComponent<MouseLook>();
        mouseLook.EnableLook();
        fadeOut.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        seedCounter.text = seedsLeft < 10 ? "0"+seedsLeft+"b" : seedsLeft+"b";
        float seedPercent = (float)seedsLeft/(float)seedPerDay;
        seedCounterImage.fillAmount+= (seedPercent-seedCounterImage.fillAmount)*0.1f;
        
        //Walk Audio Trigger
        if(isWalking == true)
        {
             if(!GetComponent<FMODUnity.StudioEventEmitter>().IsPlaying())
             {
                 GetComponent<FMODUnity.StudioEventEmitter>().Play();
             }
        }
        if(isWalking == false)
        {
            GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Services.PlantManager.CreateNarrativeMoment();
        }

        //disables mouselook when esc is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            mouseLook.DisableLook();
            #else
            //Application.Quit();
            #endif
        }
        ReticleCheck();
    }
    private void FixedUpdate()
    {
       Move();
    }
    void ReticleCheck()
    {
        if (p_state.CastCheck() == lookObject.ground) cursor.color = Color.white;
        else cursor.color = Color.grey;
    }
    public void Plant() //plant
    {
        if (seedsLeft >= Services.GameController.plantInfo[(int)PlantInfo.plantCost, (int)type])
        {
            Vector3 hitPt = CastPoint();
            Instantiate(digFX, hitPt, Quaternion.identity);
            Services.PlantManager.CreateNewPlant(type, hitPt, true);
            seedsLeft -= (int)Services.GameController.plantInfo[(int)PlantInfo.plantCost, (int)type];
        }
        else if (seedsLeft <= Services.GameController.plantInfo[(int)PlantInfo.plantCost, (int)type])
        {
            //Sound: Not enough goo
            FMODUnity.RuntimeManager.PlayOneShot("event:/Not Enough Goo");
        }

        if (!sentOutOfGoopMessage && seedsLeft <= 0)
        {
            sentOutOfGoopMessage = true;
            Services.EventManager.Fire(new RunOutOfPlants());
        }
    }
    public void DestroyPlant()
    {
        Vector3 point = CastPoint();
        bool deleted = Services.PlantManager.DestroyPlantFromLocation(point);

        if (deleted)
        {
            //CHRISTIAN: Remove plant
            unplantS = FMODUnity.RuntimeManager.CreateInstance("event:/Unplant");
            unplantS.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(point));
            unplantS.start();
        }
    }
    public void HitDoor()
    {
        if (!sleptFirstTime)
        {
            sleptFirstTime = true;
            Services.EventManager.Fire(new FirstSleep());
        }

        if (!clickedOnce)
        {
            if (ps != null) Destroy(ps.gameObject);

            //CHRISTIAN: Door open
            doorOpenS = FMODUnity.RuntimeManager.CreateInstance("event:/Door3");
            doorOpenS.start();
            clickedOnce = true;
        }
        fadeOut.fadeOut = true;
        Services.EventManager.Register<FadeOutComplete>(OnFadeOutComplete);

    }
    public void HitNarrObj()
    {
        //VO sound
        VOS = FMODUnity.RuntimeManager.CreateInstance("event:/VO_Test");
        VOS.start();
        ps = CastObj().transform.gameObject.GetComponent<ParticleSystem>();
        ps.Stop();
        ps.gameObject.gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_HoloColor", Color.white);
        ps.gameObject.tag = "Untagged";
    }
    GameObject CastObj()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            /*if (hit.distance < 2.5f)
            {
                return hit.transform.gameObject;
            }
            else return null;*/

            return hit.transform.gameObject;
        }
        else return null;
    }
    Vector3 CastPoint()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else return transform.position;
    }
    void OnFadeOutComplete(AGPEvent e){
        transform.position = spawnPosition;
        transform.eulerAngles = new Vector3(0,180,0);
        seedsLeft+=seedGainPerDay;
        seedsLeft = Mathf.Clamp(seedsLeft,0,seedPerDay);
        clickedOnce = false;
        Services.PlantManager.Update();
        Services.EventManager.Unregister<FadeOutComplete>(OnFadeOutComplete);
        dayNum++;
        Debug.Log("FADEOUTCOMPLETE");
        //if(dayNum > 2){
            Services.PlantManager.CreateNarrativeMoment();
            Debug.Log("NARRATIVE");
        //}
    }
    public void StartMove(Vector2 moveVector)
    {
        float horizontal = 0;
        float vertical = 0;
        if (CanMove(transform.right * moveVector.x))
        {
            horizontal = moveVector.x;
        }
        if (CanMove(transform.forward * moveVector.y))
        {
            vertical = moveVector.y;
        }
        moveDirection = (horizontal * transform.right + vertical * transform.forward).normalized;
        //walkCheck For Sound
        if (moveDirection.magnitude > .1f)
        {
            isWalking = true;
        }
        if (moveDirection.magnitude < .1f)
        {
            isWalking = false;
            //return;
        }
    }
    bool CanMove(Vector3 direction)
    {
        if (fadeOut.anyFade)
        {
            return false;
        }
        float distanceToPoints = collider.height / 2 - collider.radius;
        Vector3 point1 = transform.position + collider.center + Vector3.up * distanceToPoints;
        Vector3 point2 = transform.position + collider.center - Vector3.up * distanceToPoints;
        float radius = collider.radius * 0.95f;
        float castDistance = 0.5f;
        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, radius, direction, castDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "Wall")
            {
                return false;
            }
        }
        return true;
    }
    void Move()
    {
        Vector3 yVel = new Vector3(0, rb.velocity.y, 0);
        if (running)
        {
           rb.velocity = moveDirection * runSpeed * Time.fixedDeltaTime;
        }
        else
        {
           // GetComponent<FMODUnity.StudioEventEmitter>().Stop();

            rb.velocity = moveDirection * walkSpeed * Time.fixedDeltaTime;
        }
        rb.velocity += yVel;
    }
    public void Run(bool isRunning)
    {
        running = isRunning;
        if (isRunning)
        {
            safeRelease = false;
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 50, t);
            }));
        }
        else if (!isRunning)
        {
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov_default, t);
            }));
        }
    }
    public void SwitchPlant(bool next)
    {
        int typeNum = (int)type;
        if (next)
        {
            typeNum++;
            if (typeNum > 3)
            {
                typeNum = 0;
            }
            if (canAccessPlant[typeNum] == false)
            {
                typeNum = 0;
            }
        }
        else //previous
        {
            typeNum--;
            if (typeNum < 0)
            {
                typeNum = 3;
                while (canAccessPlant[typeNum] == false)
                {
                    typeNum--;
                }
            }
            type = (PlantType)typeNum;
        }
        type = (PlantType)typeNum;

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI Change");
        whichSeed.sprite = seedImages[typeNum];
        seedCost.text = Services.GameController.plantInfo[(int)PlantInfo.plantCost, (int)typeNum] + "b";
    }
    void UpdateCounts()
    {
        //resourceText.text = resource.ToString() + "r";
        for (int i = 1; i < plantCount.Count; i++)
        {
            plantSprite[i-1].fillAmount = plantCount[i] / plantMaxCount[i];
            //Debug.Log((PlantType)i);
            //Debug.Log(plantCount[i] / plantMaxCount[i]);
        }
    }
    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
    private void OnCollisionEnter(Collision collision)
    {
        
    }
}