using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class ProgressSlider : MonoBehaviour
    {
        [SerializeField] public Slider slider;
        [SerializeField] public Image fill;

        private Color _defaultColor = new Color(0.066f, 1f, 0.098f);
        private Color _extremeColor = Color.red;

        public void SetValue(int value)
        {
            slider.value = value;
        }

        public void SetMaxValue(int value)
        {
            slider.maxValue = value;
        }

        public void SetExtremeColor(bool enable)
        {
            fill.color = enable ? _extremeColor : _defaultColor;
        }
    }
}

