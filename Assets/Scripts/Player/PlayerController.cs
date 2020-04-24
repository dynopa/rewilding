﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
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

    public int seedPerDay;
    public int seedsLeft;
    //situation data
    [HideInInspector]


    //private data
    Rigidbody rb;
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
    public Sprite[] seedImages;
    //Text resourceText;
    float newHoleRadius = 5f;
    

    bool showShop = false;

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
    List<float> plantCount = new List<float> {0,8,4,2,1};
    List<float> plantMaxCount = new List<float> {0,8,4,2,1};
    List<int> plantVal = new List<int> {0, 5, 7, 10, 25};
    List<Image> plantSprite = new List<Image> {};


    float resource = 0;
    public PrinterScrollList playerInv;

    //int mossVal = 5;
    //int grassVal = 7;
    //int shrubVal = 10;
    //int treeVal = 25;
    //int specialVal = 15;
    Vector3 spawnPosition;
    int[] uiPositions = new int[]{-63,-168,-273,-378};
    bool holdingRightTrigger;
    bool holdingLeftTrigger;
    bool holdingA;
    bool releasedA;
    bool holdingB;

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

    public TextMeshProUGUI dayText;

    public TextMeshProUGUI seedCounter;
    public Image seedCounterImage;

    public float maxOxygen;
    public float oxygen;
    public Image oxygenDisplay;

    bool sentOutOfGoopMessage;
    bool sleptFirstTime;


    // Start is called before the first frame update
    void Awake()
    {
        sentOutOfGoopMessage = false;
        seedsLeft = seedPerDay;
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

        //resourceText = GameObject.Find("ResourceText").GetComponent<Text>();
        //specialIdx = GameObject.Find("SpecialIdx").GetComponent<Image>();

        //inventory

    }

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
                type = (PlantType)typeNum;
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
                }
                type = (PlantType)typeNum;
            }
            holdingLeftTrigger = true;
            
        }else{
            holdingLeftTrigger = false;
        }



        //UiIndicator.anchoredPosition = new Vector2(UiIndicator.anchoredPosition.x,uiPositions[typeNum]);
        whichSeed.sprite = seedImages[typeNum];
        seedCounter.text = seedsLeft < 10 ? "0"+seedsLeft+"/"+seedPerDay : seedsLeft+"/"+seedPerDay;
        float seedPercent = (float)seedsLeft/(float)seedPerDay;
        seedCounterImage.fillAmount+= (seedPercent-seedCounterImage.fillAmount)*0.1f;

        if(Services.PlantManager.CloseToPylon(transform.position)){
            oxygen+=Time.deltaTime*10;
        }else{
            oxygen-=Time.deltaTime;
        }
        oxygen = Mathf.Clamp(oxygen,0,maxOxygen);
        oxygenDisplay.fillAmount += ((oxygen/maxOxygen)-oxygenDisplay.fillAmount)*0.1f;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showShop = !showShop;
            ShowHideShop();
        }

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
        running = Input.GetKey(KeyCode.LeftShift);
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
            lookEnabled = false;
            #else
            Application.Quit();
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
                if (Cast(false, false) != null && Cast(false, false)?.tag == "Ground") mouseLook.DisableLook();
                lookEnabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                startCamRot = cam.transform.localEulerAngles;
                lerpStartTime = Time.time;
                plantTargetPos = Cast(false, false).transform.position;
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSpeaking)
        {
            safeRelease = false;
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 50, t);
            }));
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !isSpeaking)
        {
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov_default, t);
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
            if(hit.distance > 3f){
                return null;
            }
            if (hit.transform.name == "Button")
            {
                if(!sleptFirstTime){
                    sleptFirstTime = true;
                    Services.EventManager.Fire(new FirstSleep());
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
            if(create && seedsLeft >= ((int)type)+1){
                if(hit.transform.CompareTag("Plant") == false){
                    Services.PlantManager.CreateNewPlant(type,hit.point,true);
                    seedsLeft-= ((int)type)+1;
                }
                
            }else{
                if(!holdingA && destroy && hit.collider.CompareTag("Plant")){
                    Services.PlantManager.DestroyPlantFromGameObject(hit.collider.gameObject);
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
        seedsLeft+=seedPerDay/2;
        seedsLeft = Mathf.Clamp(seedsLeft,0,seedPerDay);
        Services.PlantManager.Update();
        Services.EventManager.Unregister<FadeOutComplete>(OnFadeOutComplete);
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

    private void ShowHideShop()
    {
        if (showShop == false)
        {
            economyUI.SetActive(false);
            hud.SetActive(true);
            crosshair.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            mouseLook.EnableLook();
            resource = playerInv.resource;
        }
        else if (showShop == true)
        {
            economyUI.SetActive(true);
            hud.SetActive(false);
            crosshair.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            mouseLook.DisableLook();
            Time.timeScale = slowSpeed;
        }
    }
}
