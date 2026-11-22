using UnityEngine;

public class Settings : MonoBehaviour
{
    
    [SerializeField]
    public SliderNumberCombo cutoff;
    
    [SerializeField]
    public SliderNumberCombo volume;
    
    [SerializeField]
    public GameObject canvas;
    
    public static Settings instance = null;
    
    void Awake()
    {
        Settings.instance = this;
    }
    
    public void ToggleCanvas()
    {
        this.canvas.SetActive(!this.canvas.activeSelf);
    }
    
}
