using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Gyroscope : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationThreshold = 5f; // Threshold for "close to zero" rotation in degrees
    
    [Header("Timer Settings")]
    public float countdownDuration = 3f; // Countdown from 3 to 0
    public Text timerText; // Reference to the timer UI text
    public Text discoveryText; // Reference to the discovery message UI text
    public Image discoveryPanel; // Background panel for the discovery message
    
    private bool isCountingDown = false;
    private float currentTimer = 0f;
    private bool hasDiscovered = false;
    private Canvas uiCanvas;
    
    void Start()
    {
        Input.gyro.enabled = true;
        
        // Rotate object by random angles between 30 and 330 degrees on all 3 axes
        float randomX = Random.Range(30f, 330f);
        float randomY = Random.Range(30f, 330f);
        float randomZ = Random.Range(30f, 330f);
        transform.localEulerAngles = new Vector3(randomX, randomY, randomZ);
        
        // Create UI Canvas if it doesn't exist
        CreateUI();
    }
    
    void CreateUI()
    {
        // Check if Canvas already exists
        uiCanvas = FindAnyObjectByType<Canvas>();
        
        if (uiCanvas == null)
        {
            // Create Canvas
            GameObject canvasObject = new GameObject("Canvas");
            uiCanvas = canvasObject.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }
        
        // Create Timer Text if it doesn't exist
        if (timerText == null)
        {
            GameObject timerObj = new GameObject("TimerText");
            timerObj.transform.SetParent(uiCanvas.transform);
            
            timerText = timerObj.AddComponent<Text>();
            timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (timerText.font == null)
            {
                timerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            timerText.fontSize = 48;
            timerText.color = Color.white;
            timerText.alignment = TextAnchor.MiddleCenter;
            
            // Position the timer text
            RectTransform timerRect = timerText.GetComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0.5f, 0.5f);
            timerRect.anchorMax = new Vector2(0.5f, 0.5f);
            timerRect.pivot = new Vector2(0.5f, 0.5f);
            timerRect.sizeDelta = new Vector2(200, 100);
            timerRect.anchoredPosition = new Vector2(0, 0);
        }
        
        // Create Discovery Panel (background) if it doesn't exist
        if (discoveryPanel == null)
        {
            GameObject panelObj = new GameObject("DiscoveryPanel");
            panelObj.transform.SetParent(uiCanvas.transform);
            
            discoveryPanel = panelObj.AddComponent<Image>();
            discoveryPanel.color = new Color(0f, 0f, 0f, 0.7f); // Semi-transparent black
            
            // Position the panel
            RectTransform panelRect = discoveryPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(450, 150);
            panelRect.anchoredPosition = new Vector2(0, 0);
            
            // Create Discovery Text as child of panel
            GameObject discoveryObj = new GameObject("DiscoveryText");
            discoveryObj.transform.SetParent(panelObj.transform);
            
            discoveryText = discoveryObj.AddComponent<Text>();
            discoveryText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (discoveryText.font == null)
            {
                discoveryText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            discoveryText.fontSize = 36;
            discoveryText.color = Color.white;
            discoveryText.alignment = TextAnchor.MiddleCenter;
            
            // Position the discovery text in the center of the panel
            RectTransform discoveryRect = discoveryText.GetComponent<RectTransform>();
            discoveryRect.anchorMin = new Vector2(0, 0);
            discoveryRect.anchorMax = new Vector2(1, 1);
            discoveryRect.pivot = new Vector2(0.5f, 0.5f);
            discoveryRect.offsetMin = new Vector2(10, 10);
            discoveryRect.offsetMax = new Vector2(-10, -10);
            discoveryRect.anchoredPosition = Vector2.zero;
        }
        else
        {
            // If panel exists but text doesn't, create it
            if (discoveryText == null)
            {
                GameObject discoveryObj = new GameObject("DiscoveryText");
                discoveryObj.transform.SetParent(discoveryPanel.transform);
                
                discoveryText = discoveryObj.AddComponent<Text>();
                discoveryText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (discoveryText.font == null)
                {
                    discoveryText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }
                discoveryText.fontSize = 36;
                discoveryText.color = Color.white;
                discoveryText.alignment = TextAnchor.MiddleCenter;
                
                RectTransform discoveryRect = discoveryText.GetComponent<RectTransform>();
                discoveryRect.anchorMin = new Vector2(0, 0);
                discoveryRect.anchorMax = new Vector2(1, 1);
                discoveryRect.pivot = new Vector2(0.5f, 0.5f);
                discoveryRect.offsetMin = new Vector2(10, 10);
                discoveryRect.offsetMax = new Vector2(-10, -10);
                discoveryRect.anchoredPosition = Vector2.zero;
            }
        }
        
        // Hide both texts initially
        if (timerText != null) timerText.enabled = false;
        if (discoveryText != null) discoveryText.enabled = false;
        if (discoveryPanel != null) discoveryPanel.enabled = false;
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
        
        // Check if rotation is close to zero on x and y axes (ignoring z)
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
                if (timerText != null)
                {
                    timerText.enabled = true;
                }
                
                // Hide discovery panel and text
                if (discoveryText != null)
                {
                    discoveryText.enabled = false;
                }
                if (discoveryPanel != null)
                {
                    discoveryPanel.enabled = false;
                }
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
                    if (timerText != null)
                    {
                        timerText.enabled = false;
                    }
                    
                    if (discoveryText != null)
                    {
                        discoveryText.text = "Nummer 2 entdeckt";
                        discoveryText.enabled = true;
                    }
                    
                    if (discoveryPanel != null)
                    {
                        discoveryPanel.enabled = true;
                    }
                }
                else
                {
                    // Update timer display - show integer seconds
                    int displayValue = Mathf.CeilToInt(currentTimer);
                    if (timerText != null)
                    {
                        timerText.text = displayValue.ToString();
                    }
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
                if (timerText != null)
                {
                    timerText.enabled = false;
                }
            }
            
            // If already discovered, keep the discovery message visible
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
}
