using UnityEngine;
using TMPro;

namespace Root
{
    public class PanelResources : MonoBehaviour
    {
        [SerializeField] private ProgressSlider slider;
        [SerializeField] private TextMeshProUGUI backpack;
        [SerializeField] private TextMeshProUGUI plastic;
        [SerializeField] private TextMeshProUGUI rubber;
        [SerializeField] private TextMeshProUGUI metal;

        public void UpdateData(Resource resource, int maxCapacity, bool enableMaxCapacity)
        {
            slider.SetValue(resource.Count());

            slider.SetMaxValue(maxCapacity);

            slider.SetExtremeColor(maxCapacity - resource.Count() < 3);

            backpack.text = enableMaxCapacity
                ? resource.Count() + " / " + maxCapacity 
                : resource.Count().ToString();

            plastic.text = resource.PlasticCount.ToString();

            rubber.text = resource.RubberCount.ToString();

            metal.text = resource.MetalCount.ToString();
        }

        public void SetActivePanel(bool enable)
        {
            gameObject.SetActive(enable);
        }
    }
}
