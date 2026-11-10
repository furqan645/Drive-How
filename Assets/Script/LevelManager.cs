using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Road Settings")]
    [SerializeField] private Transform roadParent;
    public float roadContractAmount = 0.9f;

    private List<Transform> roadCubes = new List<Transform>();
    public int currentBlock = 0; // Standard 0-indexed: Block 0 is the start
    [SerializeField] private GameObject finishPointPrefab;
    private GameObject currentFinishPoint;
    private int cubesPerBlock = 6;
    private int totalBlocks = 8;

    [SerializeField] private float finishPozX = 2f, finishPozY = .1f, finishPozZ = 8f;

    [Header("Speed Challenge")]
    public int currentLevel = 1;
    public int maxSpeedLevels = 7;
    public float[] minSpeeds = { 7, 30, 40, 50, 60, 70, 80 };
    public float[] maxSpeeds = { 20, 40, 50, 60, 70, 80, 100 };

    void Awake()
    {
        // If LevelManager manages scene-specific objects (like roadParent), 
        // do NOT make it DontDestroyOnLoad, or re-bind references on reload.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentLevel = PlayerPrefs.GetInt("Level", 1);
        if (currentLevel <= 0) currentLevel = 1;

        currentBlock = PlayerPrefs.GetInt("RoadBlock", 0);
        
        // Prevent currentBlock from locking the game if saved at max
        if (currentBlock >= totalBlocks)
        {
            currentBlock = 0;
            PlayerPrefs.SetInt("RoadBlock", 0);
        }

        Debug.Log($"[Awake] Current Level: {currentLevel} | Current Block: {currentBlock}");
    }

    void Start()
    {
        InitializeRoad();
    }

    void InitializeRoad()
    {
        roadCubes.Clear();

        if (roadParent == null)
        {
            Debug.LogError("[LevelManager] roadParent is NOT assigned in the Inspector!");
            return;
        }

        // Collect all road pieces
        for (int i = 0; i < roadParent.childCount; i++)
        {
            roadCubes.Add(roadParent.GetChild(i));
        }

        Debug.Log($"[LevelManager] Total road cubes collected: {roadCubes.Count}");

        // Activate all cubes up to currentBlock, disable the rest
        int maxActiveIndex = (currentBlock + 1) * cubesPerBlock;
        for (int i = 0; i < roadCubes.Count; i++)
        {
            roadCubes[i].gameObject.SetActive(i < maxActiveIndex);
        }

        // Place the finish point for current block
        PlaceFinishPoint(currentBlock);
    }

    void PlaceFinishPoint(int block)
    {
        if (currentFinishPoint != null)
            Destroy(currentFinishPoint);

        int finishIdx = (block + 1) * cubesPerBlock - 1;
        if (finishIdx < roadCubes.Count && roadCubes[finishIdx] != null)
        {
            Vector3 pos = roadCubes[finishIdx].position + new Vector3(finishPozX, finishPozY, finishPozZ);
            currentFinishPoint = Instantiate(finishPointPrefab, pos, Quaternion.Euler(0f, -120f, 0f));
        }
    }

    public void EnableNextBlock()
    {
        Debug.Log($"[EnableNextBlock Called] Current Block Before: {currentBlock}");

        currentBlock++;
        PlayerPrefs.SetInt("RoadBlock", currentBlock);
        PlayerPrefs.Save();

        if (currentBlock < totalBlocks)
        {
            int startIndex = currentBlock * cubesPerBlock;
            int endIndex = Mathf.Min(startIndex + cubesPerBlock, roadCubes.Count);

            for (int idx = startIndex; idx < endIndex; idx++)
            {
                if (roadCubes[idx] != null)
                {
                    roadCubes[idx].gameObject.SetActive(true);
                }
            }

            PlaceFinishPoint(currentBlock);

            if (UIManager.Instance != null)
                UIManager.Instance.Notification("Level " + (currentBlock + 1) + " unlocked! Keep going!");
        }
        else
        {
            if (UIManager.Instance != null)
                UIManager.Instance.Notification("Congratulations! Level Completed!");
        }
    }

    public void OnLevelUp()
    {
        currentLevel++;
        PlayerPrefs.SetInt("Level", currentLevel);
        PlayerPrefs.Save();     

        if (currentLevel <= maxSpeedLevels)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.Notification("Level " + currentLevel + ": Keep speed between "
                    + minSpeeds[currentLevel - 1] + " and " + maxSpeeds[currentLevel - 1] + " km/h!");

            var car = FindAnyObjectByType<CarController>();
            if (car != null) car.ResetSpeedChallenge();
        }
        else
        {
            ContractRoad();
            if (UIManager.Instance != null)
                UIManager.Instance.Notification("Road is getting narrower! Stay focused!");
        }
    }

    void ContractRoad()
    {
        if (roadParent != null)
        {
            Vector3 scale = roadParent.localScale;
            scale.x *= roadContractAmount;
            roadParent.localScale = scale;
        }
    }

    public void DelAllPref()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        currentBlock = 0;
        currentLevel = 1;
        Debug.Log("[LevelManager] All PlayerPrefs cleared!");
    }
}



// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class LevelManager : MonoBehaviour
// {

//     public static LevelManager Instance;

//     [Header("Road Settings")]
//     [SerializeField] private Transform roadParent; // Assign your parent GameObject in Inspector
//     public float roadContractAmount = 0.9f; // How much to shrink per level

//     private List<Transform> roadCubes = new List<Transform>();
//     public int currentBlock = 1; // 0-based index for active block
//     [SerializeField] private GameObject finishPointPrefab;
//     private GameObject currentFinishPoint;
//     private int cubesPerBlock = 6;
//     private int totalBlocks = 8;


//     [SerializeField] private float finishPozX = 2f, finishPozY = .1f, finishPozZ = 8f;
//     // Adjust these as needed for finish point position

//     [Header("Speed Challenge")]
//     public int currentLevel = 1;
//     public int maxSpeedLevels = 7;
//     public float[] minSpeeds = { 7, 30, 40, 50, 60, 70, 80 };
//     public float[] maxSpeeds = { 20, 40, 50, 60, 70, 80, 100 };

//     void Awake()
//     {
//         currentLevel = PlayerPrefs.GetInt("Level");
//         currentBlock = PlayerPrefs.GetInt("RoadBlock");
//         Debug.Log("currentLevel Awake"+ currentLevel);
//         Debug.Log("Awake Currrentt Block:" + currentBlock);
//         if(currentLevel == 0 )
//         currentLevel = 1;

//         if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
//         else Destroy(gameObject);
//     }


//     void Start()
//     {
//         // Collect all road pieces
//         for (int i = 0; i < roadParent.childCount; i++)
//         {
//             roadCubes.Add(roadParent.GetChild(i));
//         }

//         // Disable all blocks except the first
//         for (int block = 1; block < totalBlocks; block++)
//         {
//             for (int j = 0; j < cubesPerBlock; j++)
//             {
//                 int idx = block * cubesPerBlock + j;
//                 if (idx < roadCubes.Count)
//                     roadCubes[idx].gameObject.SetActive(false);
//             }
//         }

//         // Place the first finish point
//         PlaceFinishPoint(currentBlock);
//     }

//     void PlaceFinishPoint(int block)
//     {
//         Debug.Log("Block Number:" + block);
//         // Remove old finish point if exists
//         if (currentFinishPoint != null)
//             Destroy(currentFinishPoint);

//         int finishIdx = (block + 1) * cubesPerBlock - 1; // 6th piece of current block
//         if (finishIdx < roadCubes.Count)
//         {
//             Vector3 pos = roadCubes[finishIdx].position + new Vector3(finishPozX, finishPozY, finishPozZ); // Adjust Y as needed
//             currentFinishPoint = Instantiate(finishPointPrefab, pos, Quaternion.Euler(0f, -120f, 0f));
//         }
//     }

//     public void EnableNextBlock()
//     {
//         currentBlock++;
//         PlayerPrefs.SetInt("RoadBlock", currentBlock); // Save current level to PlayerPrefs   
//         PlayerPrefs.Save();
//         Debug.Log("Current Bloack:" + currentBlock);
//         Debug.Log("Cubesperblock:" + cubesPerBlock);
//         Debug.Log("TotalBlocks:" + totalBlocks);
//         if (currentBlock < totalBlocks)
//         {
//             Debug.Log("Current Bloack:" + currentBlock);
//             Debug.Log("Cubesperblock:" + cubesPerBlock);
//             // Enable next block
//             for (int j = 0; j < cubesPerBlock; j++)
//             {
//                 Debug.Log("Cubes Counting");
//                 int idx = currentBlock * cubesPerBlock + j;
//                 if (idx < roadCubes.Count)
//                     roadCubes[idx].gameObject.SetActive(true);
//             }
//             PlaceFinishPoint(currentBlock);

//             // Optionally, update speed range here if you want per-block speed
//             UIManager.Instance.Notification("Level " + (currentBlock + 1) + " unlocked! Keep going!");
//         }
//         else
//         {
//             UIManager.Instance.Notification("Congratulations! Level Completed!");
//             // Optionally, trigger game win logic here
//         }
//     }

//     public void OnLevelUp()
//     {
//         currentLevel++;
//         PlayerPrefs.SetInt("Level", currentLevel); // Save current level to PlayerPrefs   
//         PlayerPrefs.Save();     
//         if (currentLevel <= maxSpeedLevels)
//         {
//             UIManager.Instance.Notification("Level " + currentLevel + ": Keep speed between "
//                 + minSpeeds[currentLevel - 1] + " and " + maxSpeeds[currentLevel - 1] + " km/h!");

//             // Reset challenge flag
//             FindAnyObjectByType<CarController>().ResetSpeedChallenge();
//         }
//         else
//         {
//             ContractRoad();
//             UIManager.Instance.Notification("Road is getting narrower! Stay focused!");
//         }
//     }

//     void ContractRoad()
//     {
//         Vector3 scale = roadParent.localScale;
//         scale.x *= roadContractAmount;
//         roadParent.localScale = scale;
//     }

//     public void DelAllPref()
//     {
//         PlayerPrefs.DeleteAll();
//         PlayerPrefs.Save();
//     }
// }
