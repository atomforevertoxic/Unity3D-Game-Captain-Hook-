using Root;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Root
{
    public class PanelRepair : MonoBehaviour
    {
        [SerializeField] private SlotUI _slotRocket;
        [SerializeField] private SlotUI _slotBackpack;
        [SerializeField] private SlotUI _slotHook;

        [SerializeField] private GameObject _rocket;
        [SerializeField] private GameObject _backpack;
        [SerializeField] private GameObject _hook;

        [SerializeField] private Button _btnRocket;
        [SerializeField] private Button _btnBackpack;
        [SerializeField] private Button _btnHook;

        Resource _resourceRocket = new Resource(5, 5, 9);

        int _levelBackpack = -1;
        List<Resource> _resourceBackpack = new List<Resource>
        {
            new Resource(9, 10, 5),
            new Resource(12, 15, 9),
        };

        int _levelHook = -1;
        List<Resource> _resourceHook = new List<Resource>
        {
            new Resource(9, 5, 5),
            new Resource(12, 7, 7),
        };

        public void SetUp()
        {
            ShowNextBackpackInfo();
            ShowNextHookInfo();
            _slotRocket.SetSlot(_resourceRocket);
        }

        public void UpdateRocketData(Resource resource)
        {
            _btnRocket.interactable = resource.Subtract(_resourceRocket);
        }

        public void UpdateBackpackData(Resource resource)
        {
            if (_levelBackpack == _resourceBackpack.Count) return;

            if (_levelBackpack < 0)
            {
                _btnBackpack.interactable = false;
                return;
            }

            _btnBackpack.interactable = resource.Subtract(_resourceBackpack[_levelBackpack]);
        }

        public void UpdateHookData(Resource resource)
        {
            if (_levelHook == _resourceHook.Count) return;

            if (_levelHook < 0)
            {
                _btnHook.interactable = false;
                return;
            }

            _btnHook.interactable = resource.Subtract(_resourceHook[_levelHook]);
        }

        public void UpdateRocketHealth(int health, int maxHealth)
        {
            _slotRocket._level.text = $"{(health * 100) / maxHealth}%";
        }

        public void ShowNextBackpackInfo()
        {
            _levelBackpack++;

            if (_levelBackpack < _resourceBackpack.Count)
            {
                _slotBackpack.SetSlot(_resourceBackpack[_levelBackpack]);
            }
            else
            {
                _backpack.SetActive(false);
            }

            _slotBackpack._level.text = "Lvl " + (_levelBackpack + 1);
        }

        public void ShowNextHookInfo()
        {
            _levelHook++;

            if (_levelHook < _resourceHook.Count)
            {
                _slotHook.SetSlot(_resourceHook[_levelHook]);
            }
            else
            {
                _hook.SetActive(false);
            }

            _slotHook._level.text = "Lvl " + (_levelHook + 1);
        }

        public Resource GetRocketShipPrice()
        {
            return _slotRocket.GetSlot();
        }

        public Resource GetBackpackPrice()
        {
            return _slotBackpack.GetSlot();
        }

        public Resource GetHookPrice()
        {
            return _slotHook.GetSlot();
        }
    }
}