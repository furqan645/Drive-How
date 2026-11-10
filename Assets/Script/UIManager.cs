using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI starText;

    [SerializeField] private float notificationDuration = 1.5f; // seconds
    private Coroutine notificationCoroutine;
    public static int finalScore;

    [SerializeField] private AudioClip uiShow;
    private AudioSource audioSource;

    public static UIManager Instance;

    private void Start()
    {
        finalScore = PlayerPrefs.GetInt("FinalScore"); // Load final score from PlayerPrefs
        starText.text = "" + finalScore;
        audioSource = GetComponent<AudioSource>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Notification(string msg)
    {
        notificationText.text = msg;
        notificationPanel.SetActive(true);
        audioSource.PlayOneShot(uiShow);

        // Stop any previous coroutine to avoid overlapping
        if (notificationCoroutine != null)
            StopCoroutine(notificationCoroutine);

        notificationCoroutine = StartCoroutine(HideNotificationAfterDelay());
    }

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSeconds(notificationDuration);
        notificationPanel.SetActive(false);
    }

    public void SpeedMeaurment(float speed)
    {
        speedText.text = "" + Mathf.RoundToInt(speed) + " km/h";
    }
    public void UpdateStarCount(int stars)
    {
        starText.text = "" + stars;
        finalScore = stars;
        PlayerPrefs.SetInt("FinalScore", finalScore); // Save final score to PlayerPrefs
        PlayerPrefs.Save();
    }

    public void ShowFinalScore()
    {
       starText.text = finalScore.ToString();
    }
}
