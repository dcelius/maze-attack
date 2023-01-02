using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
    public UIDocument ui;
    public Waves spawner;
    private Label timer;
    private float timeCount;

    void Start()
    {
        timeCount = 0f;
        timer = ui.rootVisualElement.Q<Label>("Timer");
    }

    void Update()
    {
        if (spawner.getIsSpawning())
        {
            timeCount += Time.deltaTime;
            int minutes = Mathf.FloorToInt(timeCount / 60F);
            int seconds = Mathf.FloorToInt(timeCount % 60F);
            int milliseconds = Mathf.FloorToInt((timeCount * 100F) % 100F);
            // Update UI text with new time
            string newTime = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");
            timer.text = newTime;
        }
    }

    private void resetTimer()
    {
        timeCount = 0;
    }

    private void toggleView (VisualElement element)
    {
        element.visible = !element.visible;
    }
}
