using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private UIDocument ui;
    private Button confirm;
    private Button cancel;

    public Camera freeCam;
    public Camera mainMenuCam;
    public Camera buildModeCam;
    public GameObject mainMenu;
    public GameObject gameDisplay;

    // Start is called before the first frame update
    void Start()
    {
        confirm = ui.rootVisualElement.Q<Button>("Yes");
        cancel = ui.rootVisualElement.Q<Button>("No");
        confirm.clicked += returnToMain;
        cancel.clicked += returnToGame;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void returnToMain()
    {
        // Clear all new Game Objects
        GameObject[] mapPieces = GameObject.FindGameObjectsWithTag("MapPiece");
        foreach (GameObject x in mapPieces) Destroy(x);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject x in enemies) Destroy(x);
        // Leave build mode if in it
        buildModeCam.gameObject.SendMessage("leaveBuildMode");
        // Reset timer
        gameDisplay.SetActive(true);
        gameDisplay.SendMessage("resetTimer");
        gameDisplay.SetActive(false);
        // Reset time
        Time.timeScale = 1;
        // Call up main menu UI
        mainMenuCam.gameObject.SetActive(true);
        mainMenu.SetActive(true);
        // Update booleans, set cameras, and enable actions for buttons
        mainMenu.SendMessage("setIsPlaying", false);
        mainMenu.SendMessage("enableActions");
        freeCam.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void returnToGame()
    {
        Time.timeScale = 1;
        gameDisplay.SetActive(true);
        this.gameObject.SetActive(false);
        UnityEngine.Cursor.visible = false;
    }

    public void enableActions()
    {
        confirm = ui.rootVisualElement.Q<Button>("Yes");
        cancel = ui.rootVisualElement.Q<Button>("No");
        confirm.clicked += returnToMain;
        cancel.clicked += returnToGame;
    }
}
