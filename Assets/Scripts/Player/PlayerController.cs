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
    RectTransform UiIndicator;
    Image mossIdx;
    Image grassIdx;
    Image shrubIdx;
    Image treeIdx;
    Image specialIdx;
    Image indicatorImage;
    Text indicatorAmount;

    //inventory data
    float count_moss = 8f;
    float count_moss_max = 8f;
    float count_grass = 4f;
    float count_grass_max = 4f;
    float count_shrub = 2f;
    float count_shrub_max = 2f;
    float count_tree = 1f;
    float count_tree_max = 1f;
    float count_special = 2f;
    float count_special_max = 2f;

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
        indicatorAmount = GameObject.Find("IndicatorAmount").GetComponent<Text>();
        //specialIdx = GameObject.Find("SpecialIdx").GetComponent<Image>();
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
            //type = PlantType.Special;
            //UiIndicator.anchoredPosition = new Vector2(UiIndicator.anchoredPosition.x, -483);
            //UpdateCounts();
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
        if (lookEnabled == false && (Input.GetMouseButtonDown(1) || (!Input.GetMouseButton(1) && Input.GetMouseButtonDown(0))))
        {
            mousePos = Input.mousePosition;
            Cursor.visible = false; //hides mouse cursor
            Cursor.lockState = CursorLockMode.Locked; //locks mouse in center of screen
        }
        if (Input.GetMouseButtonDown(1))
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
        }
        //focus code begin
        if (Input.GetMouseButtonDown(0))
        {
            isFocusing = true;
            Cast();
            //if (!isSpeaking)
            //{
            //    StartCoroutine(Coroutines.DoOverEasedTime(0.1f, Easing.Linear, t =>
            //    {
            //        cam.fieldOfView = Mathf.Lerp(fov_default, fov_focus, t);
            //    }));
            //}
        }
        if (Input.GetMouseButtonUp(0))
        {
            isFocusing = false;
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
    GameObject Cast()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            if (hit.transform.gameObject.layer == 8) //ground
            {
                GameObject newHole = Instantiate(holePrefab, hit.point, Quaternion.identity);
                PlantNeighborManager.instance.plants.Add(newHole.transform.GetComponent<Plant>());
                Debug.Log("Dug");
                return newHole;
            }
            if (hit.transform.gameObject.tag == "Hole" && hit.transform.GetComponent<Plant>().type == PlantType.None)
            {
                //Debug.Log("its a hole");
                hit.transform.GetComponent<MeshRenderer>().enabled = false;
            
                switch (type)
                {
                    case PlantType.Spread:
                        if (count_moss > 0)
                        {
                            count_moss--;
                            UpdateCounts();
                        }
                        else
                            return hit.transform.gameObject;
                        break;
                    case PlantType.Grass:
                        if (count_grass > 0)
                        {
                            count_grass--;
                            UpdateCounts();

                        }
                        else
                            return hit.transform.gameObject;
                        break;
                    case PlantType.Shrub:
                        if (count_shrub > 0)
                        {
                            count_shrub--;
                            UpdateCounts();
                        }
                        else
                            return hit.transform.gameObject;
                        break;
                    case PlantType.Tree:
                        if (count_tree > 0)
                        {
                            count_tree--;
                            UpdateCounts();

                        }
                        else
                            return hit.transform.gameObject;
                        break;
                    case PlantType.Special:
                        if (count_special > 0)
                        {
                            count_special--;
                            UpdateCounts();

                        }
                        else
                            return hit.transform.gameObject;
                        break;
                    default:
                        break;
                }
                Plant plant = hit.transform.GetComponent<Plant>();
                plant.SetType(type);
                return hit.transform.gameObject;
            }
            if (hit.transform.name == "Button")
            {
                count_moss = count_moss_max;
                count_grass = count_grass_max;
                count_shrub = count_shrub_max;
                count_tree = count_tree_max;
                count_special = count_special_max;

                mossIdx.fillAmount = 1;
                grassIdx.fillAmount = 1;
                shrubIdx.fillAmount = 1;
                treeIdx.fillAmount = 1;
                specialIdx.fillAmount = 1;
                Debug.Log("HIT BUTTON");
                UpdateCounts();
            }
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
        switch (type)
        {
            case PlantType.Spread:
                mossIdx.fillAmount = count_moss / count_moss_max;
                indicatorImage.fillAmount = count_moss / count_moss_max;
                indicatorAmount.text = count_moss.ToString();
                break;
            case PlantType.Grass:
                grassIdx.fillAmount = count_grass / count_grass_max;
                indicatorImage.fillAmount = count_grass / count_grass_max;
                indicatorAmount.text = count_grass.ToString();
                break;
            case PlantType.Shrub:
                shrubIdx.fillAmount = count_shrub / count_shrub_max;
                indicatorImage.fillAmount = count_shrub / count_shrub_max;
                indicatorAmount.text = count_shrub.ToString();
                break;
            case PlantType.Tree:
                treeIdx.fillAmount = count_tree / count_tree_max;
                indicatorImage.fillAmount = count_tree / count_tree_max;
                indicatorAmount.text = count_tree.ToString();
                break;
            case PlantType.Special:
                specialIdx.fillAmount = count_special / count_special_max;
                indicatorImage.fillAmount = count_special / count_special_max;
                indicatorAmount.text = count_special.ToString();
                break;
            default:
                break;
        }
    }
}
