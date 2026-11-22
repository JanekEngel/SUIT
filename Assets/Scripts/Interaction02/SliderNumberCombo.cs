using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class SliderNumberCombo : MonoBehaviour
{
    
    [SerializeField]
    public InputField numberField;
    
    [SerializeField]
    public Slider slider;
    
    [SerializeField]
    public float value;
    
    void Start()
    {
        this.SetValue(value);
    }
    
    public void OnSubmitNumber()
    {
        try
        {
            this.SetValue(this.GetValue());
        }
        catch (Exception e)
        {
            this.SetValue(0.0f);
        }
    }
    
    public void OnSliderChange()
    {
        this.numberField.text = this.slider.value.ToString(CultureInfo.InvariantCulture);
    }
    
    public float GetValue()
    {
        return float.Parse(this.numberField.text, CultureInfo.InvariantCulture);
    }
    
    public void SetValue(float value)
    {
        this.slider.value = value;
        this.numberField.text = value.ToString(CultureInfo.InvariantCulture);
    }
    
}
