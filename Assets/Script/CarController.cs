using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{

    #region Variables Setting

    #region Car Controller Components

    // Car Physics Related Components
    Rigidbody carRb;

    [Header("Controlls Setting")]
    [Tooltip("This is Speed of Player Car")] // Tooltip Experiment
    [SerializeField] private int speed = 10;
    private float challengeStartTime = 0f;
    [SerializeField] private float gracePeriod = 3f; // seconds
    float speedMearing;
    private bool isGameOver = false;
    private bool challengeStarted = false; // To track if the speed challenge has started
    private bool isBraking = false; // To track if the brake is applied
    private bool isMoving = false;
    private int startInc = 1;


    //Car Movemnt
    [SerializeField] private float horizentalMove;
    [SerializeField] private float forwardMove;
    [SerializeField] private float backwardMove;

    [SerializeField] Transform centorOfMass; // For Behaviour Controll

    #endregion

    [Header("Game Settings")]
    [SerializeField] private Joystick joystick; // For Mobile Controlls
    [SerializeField] private GameObject startPoint;
    [SerializeField] private float timeToReachMinSpeed = 3f; // seconds to reach min speed
    private float reachSpeedTimer = 0f;
    private bool waitingToReachMinSpeed = false;

    #region Car Wheels Details

    [Header("Wheel Settings")]
    [Tooltip("Rotation speed for visual wheel spin")]
    [SerializeField] private float wheelRotationAngle = 30f;

    [SerializeField] private WheelCollider[] wheelColliders;
    [SerializeField] private Transform[] wheelVisuals;

    // Optional brake settings
    [SerializeField] private float brakeForce = 1000f;

    #endregion

    #region Car Audio Part
    AudioSource runningSound;

    private float runningState;
    private float idleState = 0.7f;
    private float reverseState ;
    #endregion

    #endregion

    #region Controller Functions

    #region Car Movement Controller


    public void SetForwardMove(bool value)
    { 
        isMoving = value;
    }

    void ForwardMove()
    {
        // Use button for speed
        if (isMoving)
        {
            forwardMove = 1;
        }
        else if (joystick.Vertical < 0)
            forwardMove = joystick.Vertical;
        else
            forwardMove = 0;

        float accelaration = speed * forwardMove * 2;
        wheelColliders[2].motorTorque = accelaration;
        wheelColliders[3].motorTorque = accelaration;

    }

    void WheelRotateXAxis()
    {
        // Providing Wheel Rotation to wheel Like Steering using input
        horizentalMove = Input.GetAxis("Horizontal");
        horizentalMove = joystick.Horizontal; // For Mobile Controlls

        wheelColliders[0].steerAngle = wheelRotationAngle * horizentalMove;
        wheelColliders[1].steerAngle = wheelRotationAngle * horizentalMove;
    }

    void MovementControl()
    {
        //BackwordMove();
        ForwardMove();
        WheelRotateXAxis();
    }

    #endregion


    #region Wheel Rotaion Illusion
    void ApplyWheelRotation()
    {
       for(int i = 0; i < wheelColliders.Length; i++)
        {
            WheelsRotation(wheelColliders[i], wheelVisuals[i]);
        }
    }

    void WheelsRotation(WheelCollider wheelRot, Transform wheels)
    {
        //if (wheelRot == null || wheelPos == null) return;

        Vector3 wheelColidrPositon;
        Quaternion wheelColidrRotation;

        wheelRot.GetWorldPose(out wheelColidrPositon, out wheelColidrRotation);

        wheels.position = wheelColidrPositon;
        wheels.rotation = wheelColidrRotation;
    }

    #endregion

    public void SetBrake(bool value)
    {
        isBraking = value;
    }

    void BrakeSystem()
    {
        float currentBrake = isBraking ? brakeForce : 0f;

        foreach (var wheel in wheelColliders)
        {
            wheel.brakeTorque = currentBrake;
        }
    }

    void CentorOfMass()
    {
        carRb.centerOfMass = centorOfMass.localPosition;
    }

    void CarEngineSounds()
    {
        if (forwardMove > 0)
        {
            runningState = forwardMove * 3;
            runningSound.pitch = Mathf.Lerp(runningSound.pitch, runningState, Time.deltaTime);
            runningSound.pitch = Mathf.Clamp(runningSound.pitch, 0.3f, 2.5f);

            runningSound.Play();
        }
        else if (forwardMove < 0)
        {
            reverseState = Mathf.Abs(forwardMove) * 0.9f; // Use positive pitch for reverse
            runningSound.pitch = Mathf.Clamp(reverseState, 0.3f, 1.8f); // Clamp reverse pitch
            runningSound.Play();
        }
        else
        {
            idleState = 0.7f;
            runningSound.pitch = Mathf.Lerp(runningSound.pitch, idleState, Time.deltaTime);
            runningSound.pitch = Mathf.Clamp(runningSound.pitch, 0.3f, 0.9f);
            runningSound.Play();
        }
    }


    void MearingSpeed()
    {
        speedMearing = carRb.linearVelocity.magnitude * 3.6f; // Convert m/s to km/h
        UIManager.Instance.SpeedMeaurment(speedMearing);
    }

    void CheckSpeedChallenge()
    {
        if (isGameOver) return;

        int level = LevelManager.Instance.currentLevel - 1; // 0-based index

        if (level < LevelManager.Instance.maxSpeedLevels)
        {
            float minSpeed = LevelManager.Instance.minSpeeds[level];
            float maxSpeed = LevelManager.Instance.maxSpeeds[level];

            if (waitingToReachMinSpeed)
            {
                reachSpeedTimer += Time.deltaTime;
                if (speedMearing >= minSpeed)
                {
                    waitingToReachMinSpeed = false;
                    challengeStarted = true;
                    challengeStartTime = Time.time;
                    UIManager.Instance.Notification("Speed challenge started! Stay between " + minSpeed + " and " + maxSpeed + " km/h.");
                }
                else if (reachSpeedTimer >= timeToReachMinSpeed)
                {
                    isGameOver = true;
                    UIManager.Instance.Notification("Game Over! You didn't reach the required speed in time.");
                    StartCoroutine(DelyinGameOver());
                }
            }
            else if (challengeStarted)
            {
                // Wait for grace period before enforcing strict check
                if (Time.time - challengeStartTime > gracePeriod)
                {
                    if (speedMearing < minSpeed || speedMearing > maxSpeed)
                    {
                        isGameOver = true;
                        if (speedMearing < minSpeed)
                            UIManager.Instance.Notification("Out of Range! You are Slow Try Again");
                        else if (speedMearing > maxSpeed)
                            UIManager.Instance.Notification("Over Speeding! You are very Fast! Try Again");
                        StartCoroutine(DelyinGameOver());
                    }
                }
            }
        }
    }
    public void ResetSpeedChallenge()
    {
        challengeStarted = false;
        isGameOver = false;
        waitingToReachMinSpeed = true;
        reachSpeedTimer = 0f;
        // Always show the notification for the current level
        int level = LevelManager.Instance.currentLevel - 1;
        if (level < LevelManager.Instance.maxSpeedLevels)
        {
            // Show notification for the minimum speed requirement
            timeToReachMinSpeed += 2; // Adjust time to reach min speed
            float minSpeed = LevelManager.Instance.minSpeeds[level];
            UIManager.Instance.Notification("Reach at " + minSpeed + " km/h in "+ timeToReachMinSpeed +" Sec");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            UIManager.Instance.Notification("Game Over! You Hit Ground !");
            StartCoroutine(DelyinGameOver());
        }
        if (collision.gameObject.CompareTag("FinishPoint"))
        {
            transform.position = startPoint.transform.position;
            transform.localRotation = startPoint.transform.localRotation;
            UIManager.Instance.Notification("Level Completed! Next Level!");
            LevelManager.Instance.EnableNextBlock();
            LevelManager.Instance.OnLevelUp();
            UIManager.Instance.UpdateStarCount(PlayerPrefs.GetInt("FinalScore") + startInc++);
            ResetSpeedChallenge();
            
        }
    }

    IEnumerator DelyinGameOver()
    {
        yield return new WaitForSeconds(2f);
        GameOver();
    }

    void GameOver()
    {
        isGameOver = true;
        SceneManager.LoadScene("Ender");
    }

    #endregion

    #region Predefined Update and Start
    // Start is called before the first frame update
    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        CentorOfMass();
        runningSound = GetComponent<AudioSource>();
        ResetSpeedChallenge();
    }
    // Update is called once per frame
    void Update()
    {
        CheckSpeedChallenge();
        MovementControl();
        ApplyWheelRotation();
        BrakeSystem();
        CarEngineSounds();
        MearingSpeed();
    }

    #endregion

}
