using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Interaction01 : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationThreshold = 5f; // Threshold for "close to zero" rotation in degrees
    
    [Header("Timer Settings")]
    public float countdownDuration = 3f; // Countdown from 3 to 0
    
    [Header("UI References")]
    public TextMeshProUGUI countdownText; // Reference to the Countdown TMP-Text
    public TextMeshProUGUI completeDialogueText; // Reference to the CompleteDialogue TMP-Text
    public Button continueButton; // Reference to the ContinueButton
    
    private bool isCountingDown = false;
    private float currentTimer = 0f;
    private bool hasDiscovered = false;
    
    void Start()
    {
        Input.gyro.enabled = true;
        
        // Rotate object by random angles between 30 and 330 degrees on all 3 axes
        float randomX = Random.Range(30f, 330f);
        float randomY = Random.Range(30f, 330f);
        float randomZ = Random.Range(30f, 330f);
        transform.localEulerAngles = new Vector3(randomX, randomY, randomZ);
        
        // Hide all UI elements at start
        countdownText.gameObject.SetActive(false);
        completeDialogueText.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 gyroRate = Input.gyro.rotationRateUnbiased * Mathf.Rad2Deg;

        // 1) gyro space (left-handed) -> Unity space (right-handed)
        Vector3 unityRate = new Vector3(-gyroRate.x, -gyroRate.y, gyroRate.z);

        // 2) rotate the vector into the camera's frame (camera looks along y:180)
        Quaternion cam = Quaternion.Euler(0, 180, 0);
        Vector3 rot = cam * unityRate * Time.deltaTime;

        // deltas are now in world axes, so rotate in world space
        transform.Rotate(rot, Space.World);
        
        // Get current rotation
        Vector3 currentRotation = transform.localEulerAngles;
        
        // Normalize angles to -180 to 180 range
        float xRotation = NormalizeAngle(currentRotation.x);
        float yRotation = NormalizeAngle(currentRotation.y);
        
        // Check if rotation is close to zero on x any y ignoring z
        bool isCloseToZero = Mathf.Abs(xRotation) < rotationThreshold && Mathf.Abs(yRotation) < rotationThreshold;
        
        // Handle timer logic - only if not already discovered
        if (isCloseToZero && !hasDiscovered)
        {
            if (!isCountingDown)
            {
                // Start countdown
                isCountingDown = true;
                currentTimer = countdownDuration;
                
                // Show timer
                countdownText.gameObject.SetActive(true);
            }
            
            // Update timer
            if (isCountingDown)
            {
                currentTimer -= Time.deltaTime;
                
                if (currentTimer <= 0f)
                {
                    currentTimer = 0f;
                    hasDiscovered = true;
                    isCountingDown = false;
                    
                    // Hide timer, show discovery message in panel
                    countdownText.gameObject.SetActive(false);
                    completeDialogueText.gameObject.SetActive(true); 
                    continueButton.gameObject.SetActive(true);
                }
                else
                {
                    // Update timer display - show integer seconds
                    int displayValue = Mathf.CeilToInt(currentTimer);
                    countdownText.text = displayValue.ToString();
                }
            }
        }
        else
        {
            // Rotation is not close to zero - reset timer only if not already discovered
            if (isCountingDown && !hasDiscovered)
            {
                isCountingDown = false;
                currentTimer = countdownDuration;
                
                // Hide timer
                countdownText.gameObject.SetActive(false);
            }
        }
    }
    
    // Helper method to normalize angles to -180 to 180 range
    float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f)
        {
            angle -= 360f;
        }
        return angle;
    }

    public void ContinueButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
}
