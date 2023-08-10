using UnityEngine;
using UnityEngine.UI;

namespace Script.Utils
{
    public class SliderFunction : MonoBehaviour
    {
        Slider slider;
        float Value;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            if (slider == null)
                Debug.LogWarning($"{name} is not Slider Component.");
        }

        public void Value_Add(float value)
        {
            slider.value += value;
        }

        public void Value_Subtract(float value)
        {
            slider.value -= value;
        }

        public void Value_Memory()
        {
            Value = slider.value;
        }

        public void Value_ReturnMemory()
        {
            slider.value = Value;
        }
    }
}