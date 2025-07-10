using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool interact;
		public bool hold;
		public bool createClone;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Mobile UI Settings")]
		public bool enableMobileUI = false; // Mobile UI aktif mi?

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);
		}

		public void OnHold(InputValue value)
		{
			HoldInput(value.isPressed);
		}

		public void OnCreateClone(InputValue value)
		{
			CreateCloneInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void InteractInput(bool newInteractState)
		{
			interact = newInteractState;
			if (newInteractState) Debug.Log("🎮 Interact Button Pressed - Kutu taşıma aktif!");
		}

		public void HoldInput(bool newHoldState)
		{
			hold = newHoldState;
		}

		public void CreateCloneInput(bool newCreateCloneState)
		{
			createClone = newCreateCloneState;
		}

		public void CloneInput(bool newCloneState)
		{
			createClone = newCloneState;
			if (newCloneState) Debug.Log("🎮 Clone Button Pressed - Klon oluşturma aktif!");
		}

		// Mobile UI button support
		public void OnMobileGrab()
		{
			interact = !interact; // Toggle mantığı
			Debug.Log($"🎮 Mobile Grab Button: {(interact ? "Aktif" : "Pasif")}");
		}

		public void OnMobileClone()
		{
			createClone = !createClone; // Toggle mantığı
			Debug.Log($"🎮 Mobile Clone Button: {(createClone ? "Aktif" : "Pasif")}");
		}

		// Jump için mobile button support
		public void OnMobileJump()
		{
			// Jump için momentary mantığı (basıldığında true, hemen sonra false)
			jump = true;
			Debug.Log("🎮 Mobile Jump Button Pressed!");
			// Coroutine ile kısa süre sonra false yap
			StartCoroutine(ResetJumpAfterFrame());
		}

		private System.Collections.IEnumerator ResetJumpAfterFrame()
		{
			yield return new WaitForEndOfFrame();
			jump = false;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			// Mobile UI aktifse cursor'ı kilitleme
			if (!enableMobileUI)
			{
				SetCursorState(cursorLocked);
			}
		}

		private void SetCursorState(bool newState)
		{
			// Mobile UI aktifse cursor'ı her zaman free bırak
			if (enableMobileUI)
			{
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
			}
		}

		// Mobile UI için yeni metodlar
		public void EnableMobileUI(bool enable)
		{
			enableMobileUI = enable;
			SetCursorState(!enable); // Mobile UI aktifse cursor free olsun
		}

		public void VirtualMoveInput(Vector2 virtualMoveDirection)
		{
			MoveInput(virtualMoveDirection);
			// Debug joystick input'u
			if (virtualMoveDirection.magnitude > 0.1f && enableMobileUI)
			{
				Debug.Log($"📱 Joystick Input: {virtualMoveDirection} (Magnitude: {virtualMoveDirection.magnitude:F2})");
			}
		}

		public void VirtualLookInput(Vector2 virtualLookDirection)
		{
			LookInput(virtualLookDirection);
		}

		public void VirtualJumpInput(bool virtualJumpState)
		{
			JumpInput(virtualJumpState);
		}

		public void VirtualSprintInput(bool virtualSprintState)
		{
			SprintInput(virtualSprintState);
		}
	}
	
}