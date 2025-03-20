using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Root
{
    public class SlotUI : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI _level;

        [SerializeField] public Button _buy;

        [SerializeField] public TextMeshProUGUI _type1;
        [SerializeField] public TextMeshProUGUI _type2;
        [SerializeField] public TextMeshProUGUI _type3;

        public void SetSlot(Resource resource)
        {
            _type1.text = resource.PlasticCount.ToString();
            _type2.text = resource.RubberCount.ToString();
            _type3.text = resource.MetalCount.ToString();
        }

        public Resource GetSlot()
        {
            return new Resource(
                int.Parse(_type1.text),
                int.Parse(_type2.text),
                int.Parse(_type3.text)
            );
        }
    }
}
