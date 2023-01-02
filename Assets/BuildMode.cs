using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class BuildMode : MonoBehaviour
{
    public Camera Camera1; // Free Cam
    public Camera Camera2; // Build Camera
    public UIDocument gameUI;
    public MainMenu mainMenu;
    public Waves spawner;
    public GameObject Block;
    public GameObject Trap;
    public GameObject Grid;
    private GameObject activeItem;
    private GameObject[] Labels;
    private List<GameObject> items;
    private Vector3 newPos = Vector3.zero;
    public float cubeSize;
    public float camThrust;
    public float orthoScrollScale;
    public float screenWidth = 1920;
    public float screenHeight = 1080;
    private int itemType;
    private int numGameObjects;
    private bool isDestroying = false;
    private bool isBuilding = false;
    private bool controlsSplash = true;


    // Use this for initialization
    void Start()
    {
        Camera2.enabled = false;
        Labels = GameObject.FindGameObjectsWithTag("Label");
        toggleLabels(false);
        cubeSize = cubeSize / 2;
        items = new List<GameObject>();
        items.Add(Block);
        items.Add(Trap);
        itemType = items.IndexOf(Block);
        Block.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        Trap.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        Block.SetActive(false);
        Trap.SetActive(false);
        activeItem = Block;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && mainMenu.getIsPlaying())
        {
            controlsSplash = !controlsSplash;
            gameUI.rootVisualElement.Q<VisualElement>("ControlSplash").visible = controlsSplash;
        }
        if (!spawner.getIsSpawning() && mainMenu.getIsPlaying())
        {
            if (Input.GetKeyDown("q") && isBuilding)
            {
                leaveBuildMode();
            }
            else if (Input.GetKeyDown("q"))
            {
                startBuildMode();
            }
            if (isBuilding) {
                // Movement
                Vector3 p = GetBaseInput();
                p = p * camThrust;
                p = p * Time.deltaTime;
                transform.Translate(p);
                // Zoom
                if (Input.mouseScrollDelta.y != 0)
                {
                    Camera2.orthographicSize += -Input.mouseScrollDelta.y * orthoScrollScale;
                    camThrust += -Input.mouseScrollDelta.y * orthoScrollScale;
                }
            }
            // Toggle destroy mode - uses colors to show mode
            if (Input.GetKeyDown("e") && isBuilding)
            {
                isDestroying = !isDestroying;
                if (isDestroying)
                {
                    Block.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    Trap.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                }
                else
                {
                    Block.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    Trap.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                }
            }
            // Switch active object
            if (Input.GetKeyDown("r"))
            {
                itemType = (itemType + 1) % items.Count;
                activeItem.SetActive(false);
                activeItem = items[itemType];
                activeItem.SetActive(true);
            }
            if (isBuilding)
            {
                newPos = GetWorldPosition();
                activeItem.transform.position = newPos;
                // Place item on map at new position of mouse
                if (Input.GetMouseButtonDown(0))
                {
                    newPos = new Vector3(roundUp(newPos.x, cubeSize * 2) 
                        - cubeSize, activeItem.transform.localScale.y / 2, 
                        roundUp(newPos.z, cubeSize * 2) - cubeSize);
                    GameObject collision = findObjectWithinRange(newPos, cubeSize, "MapPiece");
                    // If the location is valid and not in destroy mode, place object
                    if (collision == null)
                    {
                        if (!isDestroying)
                        {
                            GameObject newObject = Instantiate(GameObject.Find(activeItem.name + "R"), newPos, Quaternion.identity);
                            numGameObjects++;
                            newObject.tag = "MapPiece";
                            newObject.name = newObject.name + " " + numGameObjects;
                        }
                    }
                    else
                    {
                        //Debug.Log("Placement at " + newPos.ToString() + " ignored because an object is already there.");
                        if (isDestroying)
                        {
                            //Debug.Log("in Destroy mode, removing " + collision.name + " found at " + collision.transform.position.ToString());
                            Destroy(collision);
                        }
                    }
                }
            }
        }
    }

    //Fetch target position based off mouse location
    //Note: Relies on aspect ratio to be 16:9 - set by screen dimensions
    public Vector3 GetWorldPosition()
    {
        Vector3 adjustedPos = Input.mousePosition;
        float ortho = Camera2.orthographicSize;
        float ratio = ortho * 2 / Camera2.scaledPixelHeight;
        float xcor = Camera2.transform.position.x - ortho * (screenWidth / screenHeight);
        float zcor = Camera2.transform.position.z - ortho;
        Vector3 pos = new Vector3(xcor + ratio * adjustedPos.x, cubeSize, zcor + ratio * adjustedPos.y);
        return pos;
    }

    //Method to snap items to grid
    private float roundUp(float numToRound, float multiple)
    {
        if (multiple < 0.05 && multiple > -0.05)
            return numToRound;
        float remainder = Mathf.Repeat(Mathf.Abs(numToRound), multiple);
        if (remainder < 0.05 && remainder > -0.05)
            return numToRound;
        if (numToRound < 0)
            return -(Mathf.Abs(numToRound) - remainder);
        else
            return numToRound + multiple - remainder;
    }

    //Toggles grid labels
    private void toggleLabels(bool toggle)
    {
        for (int i = 0; i < Labels.Length; i++)
        {
            Labels[i].SetActive(toggle);
        }
    }

    //Detects multiples to prevent trap stacking and an excessive amount of blocks in one space
    private GameObject findObjectWithinRange(Vector3 pos, float radius, string tag)
    {
        //Debug.Log("looking for objects within " + radius + " units of " + pos.ToString() + " with the tag " + tag);
        Collider[] colliders = Physics.OverlapBox(pos, new Vector3(radius * 0.9f, radius, radius * 0.9f));
        if (colliders.Length == 0)
        {
            return null;
        }
        else
        {
            foreach (Collider i in colliders)
            {
                if (i.gameObject.CompareTag(tag))
                {
                    //Debug.Log(i.gameObject.name + " matches the target tag!");
                    return i.gameObject;
                }
            }
            return null;
        }
    }

    private void leaveBuildMode()
    {
        if (isBuilding)
        {
            Camera2.enabled = false;
            Camera1.enabled = true;
            isBuilding = false;
            isDestroying = false;
            activeItem.SetActive(isBuilding);
            toggleLabels(isBuilding);
        }
    }

    private void startBuildMode()
    {
        if (!isBuilding)
        {
            Camera2.enabled = true;
            Camera1.enabled = false;
            isBuilding = true;
            activeItem.SetActive(isBuilding);
            toggleLabels(isBuilding);
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }

    }