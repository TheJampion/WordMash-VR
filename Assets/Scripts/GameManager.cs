using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum GameMode
{
    TextIcon, Text, Icon
}
public class GameManager : MonoBehaviour
{
    //Static References
    public static GameManager instance;
    public static Action<GameMode> startGame;
    public static Action<GameMode> stopGame;
    private const float MAX_TIME = 90f;
    
    //Assigned in Inspector
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private AudioSource whistleSource;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject timerPanel;

    //Variables
    private GameMode gameMode;
    private float timer = MAX_TIME;
    private bool gameStarted;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SetGameMode(int index)
    {
        gameMode = (GameMode)index;
    }
    public void StartGame()
    {
        timer = MAX_TIME;
        startGame?.Invoke(gameMode);
        menuPanel.SetActive(false);
        timerPanel.SetActive(true);
        gameStarted = true;
    }

    public void EndGame()
    {
        whistleSource.Play();
        timerPanel.SetActive(false);
        menuPanel.SetActive(true);
        stopGame?.Invoke(gameMode);
        gameStarted = false;
    }

    private void Update()
    {
        if (gameStarted)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                Mathf.Clamp(timer, 0, MAX_TIME);
                timerText.text = timer.ToString("F2");
            }
            else
            {
                EndGame();
            }
        }
    }
}
