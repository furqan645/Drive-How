using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnControlls : MonoBehaviour
{
    public GameObject howToPlay;
    public GameObject challengeUI;

    public void ToMainManu()
    {
        SceneManager.LoadScene("Starter");
    }

    public void GamePlayBtn()
    {
        SceneManager.LoadScene("GamePlay");
        PlayerPrefs.SetInt("Level", 0); // Reset level to 0 when starting a new game
    }

    public void ResumeBtn()
    {
        SceneManager.LoadScene("GamePlay");
        // Subscribe to sceneLoaded event to ensure we load the level after the scene is fully loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayerPrefs.GetInt("Level"); // Load current level from PlayerPrefs
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe so it doesn’t run multiple times
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void EixtBtn()
    {
        Application.Quit();
    }

    public void InstructionEnable()
    {
        howToPlay.SetActive(true);
    }

    public void InstructionDisable()
    {
        howToPlay.SetActive(false);
    }

    public void ChallengeUI()
    {
        challengeUI.SetActive(false);
    }
}