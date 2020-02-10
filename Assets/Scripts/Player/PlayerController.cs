using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    //public states
    public bool running;

    //public data
    public float walkSpeed;
    public float runSpeed;
    public float jumpSpeed;
    public float talkSpeed = 0.1f; //multiplies run/walk speed while talking
    public float slowSpeed = 0.5f; //multiplies time.timeScale
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
    RectTransform UiIndicator;
    Image mossIdx;
    Image grassIdx;
    Image shrubIdx;
    Image treeIdx;
    Image specialIdx;
    Image deleteIdx;
    Image indicatorImage;
    Text indicatorAmount;
    Text resourceText;
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




    // Start is called before the first frame update
    void Awake()
    {
        type = PlantType.Spread;
        mousePos = Input.mousePosition;
        instance = this;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        cam = Camera.main;
        layerGround = LayerMask.NameToLayer("Ground");
        Cursor.lockState = CursorLockMode.Locked;
        mouseLook = cam.gameObject.GetComponent<MouseLook>();

        //ui
        UiIndicator = GameObject.Find("Indicator").GetComponent<RectTransform>();
        indicatorImage = GameObject.Find("Indicator").GetComponent<Image>();
        mossIdx = GameObject.Find("MossIdx").GetComponent<Image>();
        grassIdx = GameObject.Find("GrassIdx").GetComponent<Image>();
        shrubIdx = GameObject.Find("ShrubIdx").GetComponent<Image>();
        treeIdx = GameObject.Find("TreeIdx").GetComponent<Image>();
        deleteIdx = GameObject.Find("DeleteIdx").GetComponent<Image>();
        indicatorAmount = GameObject.Find("IndicatorAmount").GetComponent<Text>();
        resourceText = GameObject.Find("ResourceText").GetComponent<Text>();
        //specialIdx = GameObject.Find("SpecialIdx").GetComponent<Image>();

        //inventory
        plantSprite.Add(mossIdx); 
        plantSprite.Add(grassIdx);
        plantSprite.Add(shrubIdx);
        plantSprite.Add(treeIdx);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            type = PlantType.Spread;
            UiIndicator.anchoredPosition = new Vector2(UiIndicator.anchoredPosition.x,-63);
            UpdateCounts();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            type = PlantType.Grass;
            UiIndicator.anchoredPosition = new Vector2(UiIndicator.anchoredPosition.x, -168);
            UpdateCounts();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            type = PlantType.Shrub;
            UiIndicator.anchoredPosition = new Vector2(UiIndicator.anchoredPosition.x, -273);
            UpdateCounts();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            type = PlantType.Tree;
            UiIndicator.anchoredPosition = new Vector2(UiIndicator.anchoredPosition.x, -378);
            UpdateCounts();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UiIndicator.anchoredPosition = new Vector2(UiIndicator.anchoredPosition.x, -483);
            UpdateCounts();

        }

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
        //end movement
        //items

        //disables mouselook when esc is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mouseLook.enabled = false;
            lookEnabled = false;
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
        if (Input.GetMouseButtonDown(0))
        {
            mouseLook.enabled = true;
            lookEnabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            //isFocusing = true;
            if (!showShop)
            {
                Cast(true);
            }
            //if (!isSpeaking)
            //{
            //    StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            //    {
            //        cam.fieldOfView = Mathf.Lerp(fov_default, fov_focus, t);
            //    }));
            //}
        }
        if(Input.GetMouseButtonDown(1)){
            Cast(false);
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
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 50, t);
            }));
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !isSpeaking)
        {
            StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 50, t);
            }));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckInteraction();
        }
        
        //end items

        //time slow
        if (isSpeaking)
        {
            Time.timeScale = slowSpeed;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    private void FixedUpdate()
    {
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
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
    GameObject Cast(bool create)
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {

            if (hit.transform.name == "Button")
            {

                /*for (int i = 1; i < plantMaxCount.Count-1; i++)
                {
                    plantCount[i] = plantMaxCount[i];
                }
                
                indicatorImage.fillAmount = 1;
                UpdateCounts();*/
                Services.PlantManager.Update();
                return hit.transform.gameObject;
            }
            if(create){
                Services.PlantManager.CreateNewPlant(type,hit.point);
            }else{
                if(hit.collider.CompareTag("Plant")){
                    Services.PlantManager.DestroyPlantFromGameObject(hit.collider.gameObject);
                }
            }
            
            //UpdateCounts();
            return hit.transform.gameObject;
        }
        return hit.transform.gameObject;
    }
    void Move()
    {

        Vector3 yVel = new Vector3(0, rb.velocity.y, 0);
        if (running)
        {
            if (isSpeaking)
            {
                rb.velocity = moveDirection * runSpeed * talkSpeed * Time.deltaTime;
            }
            else if (!isSpeaking)
            {
                rb.velocity = moveDirection * runSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (isSpeaking)
            {
                rb.velocity = moveDirection * walkSpeed * talkSpeed * Time.deltaTime;
            }
            else
            {
                rb.velocity = moveDirection * walkSpeed * Time.deltaTime;
            }
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
            cam.fieldOfView = fov_default;
        }
    }

    void UpdateCounts()
    {
        resourceText.text = resource.ToString() + "r";
        for (int i = 1; i < plantCount.Count; i++)
        {
            plantSprite[i-1].fillAmount = plantCount[i] / plantMaxCount[i];
            //Debug.Log((PlantType)i);
            //Debug.Log(plantCount[i] / plantMaxCount[i]);
        }
        indicatorImage.fillAmount = plantCount[(int)type] / plantMaxCount[(int)type];
        indicatorAmount.text = plantCount[(int)type].ToString();


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
            mouseLook.enabled = true;
            resource = playerInv.resource;
        }
        else if (showShop == true)
        {
            economyUI.SetActive(true);
            hud.SetActive(false);
            crosshair.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            mouseLook.enabled = false;
            Time.timeScale = slowSpeed;
        }
    }
}
