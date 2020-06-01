using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerController : MonoBehaviour
{
    
    public int seedPerDay;
    public int seedGainPerDay;
    public static PlayerController instance;
    public bool[] canAccessPlant = new bool[] { true, false, false, false };
    public int dayNum;
    enum oxygenState
    {
        full, draining, low, none
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
    private bool safeRelease; //true after comms exit lerping is complete
    private bool lookEnabled;

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
    //Text resourceText;
    float newHoleRadius = 5f;



    //inventory data
    //float count_moss = 8f;
    //float count_moss_max = 8f;
    //float count_grass = 4f;
    //float count_grass_max = 4f;
    //float count_shrub = 2f;
    //float count_shrub_max = 2f;
    //float count_tree = 1f;
    //float count_tree_max = 1f;
    //float count_special = 2f;
    //float count_special_max = 1f;

    //    None,Spread,Grass,Shrub,Tree,Special,Delete
    List<float> plantCount = new List<float> { 0, 8, 4, 2, 1 };
    List<float> plantMaxCount = new List<float> { 0, 8, 4, 2, 1 };
    List<int> plantVal = new List<int> { 0, 5, 7, 10, 25 };
    List<Image> plantSprite = new List<Image> { };


    float resource = 0;

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
    Vector3 startCamRot;
    Quaternion endCamRot;
    float lerpStartTime;
    float lerpLength;
    float lerpTime;

    

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




    // Start is called before the first frame update
    
    void Awake()
    {
        sentOutOfGoopMessage = false;
        oxygen = maxOxygen;
        spawnPosition = transform.position;
        type = PlantType.Spread;
        mousePos = Input.mousePosition;
        instance = this;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        cam = Camera.main;
        layerGround = LayerMask.NameToLayer("Ground");
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook = cam.gameObject.GetComponent<MouseLook>();
        mouseLook.EnableLook();
        lerpLength = Vector3.Distance(startCamPos, endCamPos);

        fadeOut.gameObject.SetActive(true);
        
        //might not work in awake - move to start if it doesn't

        //resourceText = GameObject.Find("ResourceText").GetComponent<Text>();
        //specialIdx = GameObject.Find("SpecialIdx").GetComponent<Image>();

        //inventory

    }

    /*private void Start()
    {
        FMOD.Studio.EventInstance[] bgmS = new FMOD.Studio.EventInstance[2];

        bgmS[0] = FMODUnity.RuntimeManager.CreateInstance("event:/BGM0");
        bgmS[1] = FMODUnity.RuntimeManager.CreateInstance("event:/BGM1");
        Debug.Log(IsPlaying(bgmS[0]));
        bgmS[0].start();
        Debug.Log(IsPlaying(bgmS[0]));
    }*/

    // Update is called once per frame
    void Update()
    {
        if(!sentOutOfGoopMessage && seedsLeft <= 0){
            sentOutOfGoopMessage = true;
            Services.EventManager.Fire(new RunOutOfPlants());
        }
        int typeNum = (int)type;
        
        float rightTrigger = Input.GetAxis("RightTrigger");
        if(rightTrigger == 1 || Input.GetKeyDown(KeyCode.E)){
            if(!holdingRightTrigger){
                typeNum++;
                if(typeNum > 3){
                    typeNum = 0;
                }
                if(canAccessPlant[typeNum] == false){
                    typeNum = 0;
                }
                type = (PlantType)typeNum;
                //uiSwitchS = FMODUnity.RuntimeManager.CreateInstance("event:/UI_Change");
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Change");
                //CHRISTIAN: UI switch plant
            }
            holdingRightTrigger = true;
            
        }else{
            holdingRightTrigger = false;
        }
        float leftTrigger = Input.GetAxis("LeftTrigger");
        if(leftTrigger == 1 || Input.GetKeyDown(KeyCode.Q)){
            if(!holdingLeftTrigger){
                typeNum--;
                if(typeNum < 0){
                    typeNum = 3;
                    while(canAccessPlant[typeNum] == false){
                        typeNum--;
                    }
                }
                type = (PlantType)typeNum;
                //CHRISTIAN: UI switch plant
                FMODUnity.RuntimeManager.PlayOneShot("event:/UI Change");
            }
            holdingLeftTrigger = true;
            
        }else{
            holdingLeftTrigger = false;
        }



        //UiIndicator.anchoredPosition = new Vector2(UiIndicator.anchoredPosition.x,uiPositions[typeNum]);
        whichSeed.sprite = seedImages[typeNum];
        //seedCounter.text = seedsLeft < 10 ? "0"+seedsLeft+"/"+seedPerDay : seedsLeft+"/"+seedPerDay; switched to try not displaying max in #
        seedCounter.text = seedsLeft < 10 ? "0"+seedsLeft+"b" : seedsLeft+"b";
        float seedPercent = (float)seedsLeft/(float)seedPerDay;
        seedCounterImage.fillAmount+= (seedPercent-seedCounterImage.fillAmount)*0.1f;
        seedCost.text = Services.GameController.plantInfo[(int)PlantInfo.plantCost, (int)typeNum]+"b";

        if(Services.PlantManager.CloseToPylon(transform.position)){
            oxygen+=Time.deltaTime*10;
            if (oState == oxygenState.draining || oState == oxygenState.low)
            {
                //CHRISTIAN: Stop oxygen drain noise and play valve seal
               // FMOD.Studio.PLAYBACK_STATE oPState;
                //oDrainS.getPlaybackState(out oPState);
                oDrainS.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                oState = oxygenState.full;
            }
            else if (oState == oxygenState.none)
            {
                //play valve seal
                oState = oxygenState.full;
                FMODUnity.RuntimeManager.PlayOneShot("event:/Valve");
            }
        }
        else
        {
            oxygen-=Time.deltaTime;
            if (oState != oxygenState.draining)
            {
                //CHRISTIAN: Oxygen drain start
                oDrainS = FMODUnity.RuntimeManager.CreateInstance("event:/lost oxygen");
                
                    oDrainS.start(); 
                
                oState = oxygenState.draining;
            }
        }
        if (oxygen > 0 && oxygen < 0.2)
        {
            if (oState == oxygenState.draining)
            {
                //CHRISTIAN: LOW OXYGEN
                lowOS = FMODUnity.RuntimeManager.CreateInstance("event:/LowOxy2");
                lowOS.start();
                oState = oxygenState.low;
            }
        }
        else if (oxygen <= 0)
        {
            if (oState == oxygenState.low)
            {
                //CHRISTIAN: Power down noise
                powerDownS = FMODUnity.RuntimeManager.CreateInstance("event:/Door3");
                FMODUnity.RuntimeManager.PlayOneShot("event:/PowerDown 2");
                oDrainS.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                lowOS.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                //stop drain sound
                oState = oxygenState.none;
            }
        }
        oxygen = Mathf.Clamp(oxygen,0,maxOxygen);
        oxygenDisplay.fillAmount += ((oxygen/maxOxygen)-oxygenDisplay.fillAmount)*0.1f;

        /*if (Input.GetKeyDown(KeyCode.Tab))
        {
            showShop = !showShop;
            ShowHideShop();
        }*/

        //movement
        float horizontal = 0;
        float vertical = 0;
        if (CanMove(transform.right * Input.GetAxisRaw("Horizontal")))
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }
        if (CanMove(transform.forward * Input.GetAxisRaw("Vertical")))
        {
            vertical = Input.GetAxisRaw("Vertical");
        }
        moveDirection = (horizontal * transform.right + vertical * transform.forward).normalized;
        running = Input.GetKey(KeyCode.LeftShift); // ROWAN: Add || case for controller button
        //walkCheck For Sound
        if(moveDirection.magnitude > .1f)
        {
            isWalking = true;
        }
        if(moveDirection.magnitude < .1f)
        {
            isWalking = false;
        }
        //end movement
        
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


        //items


        //disables mouselook when esc is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            mouseLook.DisableLook();
            #else
            //Application.Quit();
            #endif
        }
        //if (lookEnabled == false && (Input.GetMouseButtonDown(1) || (!Input.GetMouseButton(1) && Input.GetMouseButtonDown(0))))
        //{
        //    mousePos = Input.mousePosition;
        //    Cursor.visible = false; //hides mouse cursor
        //    Cursor.lockState = CursorLockMode.Locked; //locks mouse in center of screen
        //}

        /*if (Input.GetMouseButtonDown(1))
        {
            if (lookEnabled == false)
            {
                mouseLook.enabled = true;
            }
            //CheckInteraction();
            isSpeaking = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                cam.fieldOfView = Mathf.Lerp(fov_default, fov_comms, t);
            }));
        }
        if (Input.GetMouseButtonUp(1))
        {
            //CheckInteraction();
            isSpeaking = false;
            if (!isFocusing)
            {
                StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
                {
                    cam.fieldOfView = Mathf.Lerp(fov_comms, fov_default, t);
                }));
            }
            if (isFocusing)
            {
                StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
                {
                    cam.fieldOfView = Mathf.Lerp(fov_comms, fov_focus, t);
                }));
            }
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }*/
        //focus code begin


        if (Input.GetAxis("Fire1") == 1)
        {
            if (!holdingA){ //on click
                if (Cast(false, false) != null && Cast(false, false)?.tag == "Ground")
                {
                    mouseLook.DisableLook();
                    plantTargetPos = Cast(false, false).transform.position;
                }
                lookEnabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                startCamRot = cam.transform.localEulerAngles;
                lerpStartTime = Time.time;
                //isFocusing = true;
            }
            else if (holdingA) //on hold
            { 
                if (Cast(false, false) != null && Cast(false, false)?.tag == "Ground")
                {
                    // Distance moved equals elapsed time times speed..
                    float distCovered = (Time.time - lerpStartTime) * 2f;
                    float squatProg = distCovered / lerpLength;

                    if (squatProg < 1f)
                    {

                        cam.transform.localPosition = Vector3.Lerp(startCamPos,endCamPos, squatProg);//-.33f

                        //Vector3 newRot = new Vector3(startCamRot.x - 10, startCamRot.y, startCamRot.z);
                        //cam.transform.localEulerAngles = Vector3.Lerp(startCamRot, newRot, squatProg);
                       
                    }
                    else if (squatProg >= 1.2f && squatComplete == false)
                    { 
                        Cast(true, false);
                        mouseLook.EnableLook();
                        squatComplete = true;
                    }
                  
                }
            }
            //seedCounter.text = cam.transform.localEulerAngles.x.ToString();
            //if (!isSpeaking)
            //{
            //    StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            //    {
            //        cam.fieldOfView = Mathf.Lerp(fov_default, fov_focus, t);
            //    }));
            //}
            holdingA = true;
            //releasedA = false;
        }
        else //on release
        {
            if (holdingA)
            {
                lerpStartTime = Time.time;
                mouseLook.EnableLook();
            }
            if (squatComplete == true)
            {
                float distCovered = (Time.time - lerpStartTime) * 3f;
                float squatProg = distCovered / lerpLength;
                cam.transform.localPosition = Vector3.Lerp(endCamPos, startCamPos, squatProg);

                if (squatProg > 1)
                {
                    squatComplete = false;
                }
            }

            
            holdingA = false;

        }


        if (Input.GetAxis("Fire2") == 1){
            if(!holdingB){
                Cast(false, true);
            }
            holdingB = true;
            
        }else{
            holdingB = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            //isFocusing = false;
            //if (!isSpeaking)
            //{
            //    StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            //    {
            //        cam.fieldOfView = Mathf.Lerp(fov_focus, fov_default, t);
            //    }));
            //}
        }
        //focus code end

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSpeaking) //SPRINTING/RUNNING
        {
            safeRelease = false;
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 50, t);
            }));
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !isSpeaking)
        {
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov_default, t);
            }));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckInteraction();
        }
        
        //end items

        //time slow
    }
    private void FixedUpdate()
    {
        Move();
    }
    void CheckInteraction()
    {
        float distance = 4f;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance))
        {
            if (hit.transform.tag == "Item")
            {

            }
        }
    }
    GameObject Cast(bool create, bool destroy)
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.distance > 2f){
                return null;
            }
            if (hit.transform.name == "Button")
            {
                if(!sleptFirstTime){
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
                    //FMODUnity.RuntimeManager.PlayOneShot("event:/Door3");
                }
                /*for (int i = 1; i < plantMaxCount.Count-1; i++)
                {
                    plantCount[i] = plantMaxCount[i];
                }
                
                indicatorImage.fillAmount = 1;
                UpdateCounts();*/
                fadeOut.fadeOut = true;
                Services.EventManager.Register<FadeOutComplete>(OnFadeOutComplete);

                
                return hit.transform.gameObject;
            }
            if (hit.transform.tag == "NarrObj")
            {
                //VO sound
                VOS = FMODUnity.RuntimeManager.CreateInstance("event:/VO_Test");
                VOS.start();
                ps = hit.transform.gameObject.GetComponent<ParticleSystem>();
                ps.Stop();
                ps.gameObject.gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_HoloColor",Color.white);
                ps.gameObject.tag = "Untagged";
                return hit.transform.gameObject;
            }
            if (create && seedsLeft >= Services.GameController.plantInfo[(int)PlantInfo.plantCost,(int)type])
            {
                if (hit.transform.CompareTag("Plant") == false)
                {
                    Instantiate(digFX, hit.point, Quaternion.identity);
                    Services.PlantManager.CreateNewPlant(type, hit.point, true);
                    seedsLeft -= (int)Services.GameController.plantInfo[(int)PlantInfo.plantCost,(int)type];
                }

            }
            else if (create && seedsLeft <= Services.GameController.plantInfo[(int)PlantInfo.plantCost,(int)type])
            {
                //CHRISTIAN: Not enough goo
                FMODUnity.RuntimeManager.PlayOneShot("event:/Not Enough Goo");
            }

            else
            {
                if (!holdingA && destroy && hit.collider != null)
                {

                    bool deleted = Services.PlantManager.DestroyPlantFromLocation(hit.point);

                    if (deleted){
                        //CHRISTIAN: Remove plant
                        unplantS = FMODUnity.RuntimeManager.CreateInstance("event:/Unplant");
                        unplantS.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(hit.point));
                        unplantS.start();

                    }

                }
            }

                //UpdateCounts();
                return hit.transform.gameObject;
        }
        return null;
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
        
        if(dayNum > 2){
            Services.PlantManager.CreateNarrativeMoment();
        }
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
    void Jump()
    {
        if (isGrounded())
        {
            //rb.velocity += new Vector3(0, jumpSpeed * Time.deltaTime, 0);
        }
    }
    bool CanMove(Vector3 direction)
    {
        if(fadeOut.anyFade){
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
        if (Input.GetMouseButton(1))
        {
            //return false;
        }

        return true;
    }
    public bool isGrounded()
    {
        float distanceToPoints = collider.height / 2 - collider.radius;
        Vector3 point1 = transform.position + collider.center + Vector3.up * distanceToPoints;
        Vector3 point2 = transform.position + collider.center - Vector3.up * distanceToPoints;
        float castDistance = 0.1f;
        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, collider.radius, transform.up * -1f, castDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.layer == layerGround)
            {
                groundContactNormal = hit.normal;
                return true;
            }
        }
        groundContactNormal = Vector3.up;
        return false;
    }
    IEnumerator MouseUpContingency()
    {
        yield return new WaitForSeconds(.2f);
        if (!Input.GetMouseButton(1))
        {
            //cam.fieldOfView = fov_default;
        }
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
        if (collision.gameObject.name == "BGMZone0")
        {
            SwitchBGM(0);
        }
        if (collision.gameObject.name == "BGMZone1")
        {
            SwitchBGM(1);
        }
    }
    void SwitchBGM(int bgmNum)
    {

    }
}
