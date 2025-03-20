using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 look;
		public bool shoot;
		public bool retract;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnLook(InputValue value)
		{
			if(cursorInputForLook && Time.timeScale == 1f)
			{
				LookInput(value.Get<Vector2>());
			}
		}

        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }

        public void OnRetract(InputValue value)
        {
            RetractInput(value.isPressed);
        }
#endif

        public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        public void RetractInput(bool newRetractState)
        {
            retract = newRetractState;
        }
	}
	
}