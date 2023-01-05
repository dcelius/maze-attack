using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Code repurposed from course material provided by Simon Colreavy
public class Waves : MonoBehaviour
{

    [System.Serializable]
    public enum SpawnState { STANDARD, WAITING, WARNING, SPAWNING, FIGHTING }

    public SpawnState state = SpawnState.STANDARD;
    public GameObject Enemy;
    private GameObject[] Enemies;
    public GameObject builder;
    public GameObject gameDisplay;
    public MainMenu mainMenu;
    public AstarNav pathfinder;
    private Node path;
    private List<VisualElement> elements;
    public bool isSpawning = false;
    public bool debug;
    private bool firstSpawn;
    private int cap;
    private Vector2Int playerLoc;
    private Vector2Int spawnLoc;

    void Start()
    {
        firstSpawn = true;
        // Get the list of icon labels to use later when toggling spawning on and off
        gameDisplay.SetActive(true);
        elements = gameDisplay.GetComponent<UIDocument>().rootVisualElement.Query("IconPanel").Children<VisualElement>().ToList();
        gameDisplay.SetActive(false);
        cap = 1;
        playerLoc = new Vector2Int(95, 95);
        spawnLoc = new Vector2Int(104, 104);
    }
    void Update()
    {
        if (Input.GetKeyDown("o") && mainMenu.getIsPlaying())
        {
            if (firstSpawn || debug)
            {
                toggleSpawning();
                firstSpawn = false;
            }
        }
        if (isSpawning && mainMenu.getIsPlaying())
        {
            if (state == SpawnState.STANDARD)
            {
                path = pathfinder.findPath(playerLoc, spawnLoc); // Generates a path backwards from goal to source
                if (path == null) Debug.LogError("No Path Found!");
                if (debug && path != null) builder.SendMessage("displayPath", path);
                for (int x = 0; x < cap; x++)
                {
                    Debug.Log("cap: " + cap.ToString() + " x: " + x.ToString());
                    StartCoroutine(SpawnWaiting());
                }
                cap++;
            }

        }
    }
    IEnumerator SpawnWaiting()
    {
        state = SpawnState.WAITING;
        yield return new WaitForSeconds(1f);
        state = SpawnState.WARNING;
        yield return new WaitForSeconds(1f);
        state = SpawnState.SPAWNING;
        // random position spaces out the enemies, so that collision doesn't cause them to shoot across the map
        GameObject newEnemy = Instantiate(Enemy, randomPos(transform.position, 7.5f), Quaternion.identity);
        newEnemy.SetActive(true);
        // as the enemy cap increases, so does the health
        newEnemy.SendMessage("setHealth", cap * 10);
        newEnemy.GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;
        newEnemy.tag = "Enemy";
        newEnemy.layer = 6;
        newEnemy.SendMessage("setPath", path);
        yield return new WaitForSeconds(1.0f);
        state = SpawnState.FIGHTING;
        yield return new WaitForSeconds(10f);
        state = SpawnState.STANDARD;
    }

    private Vector3 randomPos(Vector3 current, float scale)
    {
        current.x += scale * Random.Range(-1, 1);
        current.z += scale * Random.Range(-1, 1);
        return current;
    }

    private void toggleSpawning()
    {
        isSpawning = !isSpawning;
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        // Pause all enemy movement
        if (Enemies is not null)
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i].GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = !Enemies[i].GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped;
            }
        }
        if (isSpawning && !debug)
        {
            builder.SendMessage("leaveBuildMode");
            foreach (VisualElement e in elements)
            {
                e.visible = false;
            }
        }
        else
        {
            foreach (VisualElement e in elements)
            {
                e.visible = true;
            }
        }
    }

    public bool getIsSpawning() 
    { 
        return isSpawning; 
    }

    private void resetSpawner()
    {
        cap = 0;
        isSpawning = false;
        firstSpawn = true;
    }
}