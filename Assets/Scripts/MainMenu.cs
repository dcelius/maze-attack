using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public UIDocument ui;
    public GameObject gameui;
    public Camera gameStartCam;
    public Camera menuCam;
    public static bool isPlaying;
    private Button startGame;
    private Button endGame;

    // Start is called before the first frame update
    void Start()
    {
        startGame = ui.rootVisualElement.Q<Button>("startGame");
        endGame = ui.rootVisualElement.Q<Button>("endGame");
        startGame.clicked += gameStart;
        endGame.clicked += gameEnd;
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void gameStart()
    {
        gameStartCam.gameObject.SetActive(true);
        UnityEngine.Cursor.visible = false;
        gameui.SetActive(true);
        menuCam.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        isPlaying = true;
    }

    private void gameEnd()
    {
        Application.Quit();
    }

    public void enableActions()
    {
        startGame = ui.rootVisualElement.Q<Button>("startGame");
        endGame = ui.rootVisualElement.Q<Button>("endGame");
        startGame.clicked += gameStart;
        endGame.clicked += gameEnd;
    }

    public bool getIsPlaying()
    {
        return isPlaying;
    }

    public void setIsPlaying(bool status)
    {
        isPlaying = status;
    }
}
