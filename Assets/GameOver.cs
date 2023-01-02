using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOver : MonoBehaviour
{
    public GameObject splash;
    public GameObject pauseMenu;
    public GameObject gameDisplay;
    public GameObject freeCam;
    public UIDocument timerUI;
    public UIDocument splashUI;
    public MainMenu mainMenu;

    private bool gameOver = false;
    void Start()
    {
    }

    void Update()
    {
        if (gameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (mainMenu.getIsPlaying())
        {
            // Open Pause Menu
            if (!gameOver && Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                UnityEngine.Cursor.visible = true;
                splash.SetActive(false);
                gameDisplay.SetActive(false);
                pauseMenu.SendMessage("enableActions");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Triggers end of game if Enemy collides with NPC that has this script attached
        if (other.CompareTag("Enemy"))
        {
            mainMenu.setIsPlaying(false);
            splash.SetActive(true);
            splashUI.rootVisualElement.Q<Label>("TimerSplash").text = 
                "You survived for: " + timerUI.rootVisualElement.Q<Label>("Timer").text;
            gameDisplay.SetActive(false);
            Time.timeScale = 0;
            gameOver = true;
        }
    }
}
