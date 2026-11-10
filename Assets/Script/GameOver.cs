using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    // Start is called before the first frame update
    void Start()
    {
        finalScoreText.text = "" + UIManager.finalScore; 
    }
}
