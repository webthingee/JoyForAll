using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoyForAll
{
	public class InputMaster : MonoBehaviour
	{
		public bool clearJoysitckPlayerPrefs;
		public bool getJoystickValuesToContinue;
		public GameObject inputSetter;
		public KeyCode showSetup;
		
		[Header("Axis Names")]
		public string HorizontalAxis = "Horizontal";
		public string VerticalAxis = "Vertical";

		[Header("Button Names")]
		public string Button01 = "Jump";
		public string Button02 = "Fire1";
		public string Button03 = "Fire2";
		public string Button04 = "Fire3";

		[Serializable] public class ButtonKeys : Dictionary<string, KeyCode> {}
		public ButtonKeys buttonKeys;

		public List<string> inverts = new List<string>();
		
		[Serializable] public class AxisKeys : Dictionary<string, string> {}
		public AxisKeys axisKeys;

		private bool playerPrefsExist = true;
		
		private void Awake()
		{	
			// KeyCodes
			buttonKeys = new ButtonKeys();

			// Axis
			axisKeys = new AxisKeys();
						
			// Define buttons and axis
			SetupJoystick();
		}

		private void Update()
		{
			if (Input.GetKeyDown(showSetup))
			{
				inputSetter.SetActive(true);
			}
		}

		private void SetupJoystick()
		{	
			// KeyCodes
			buttonKeys[Button01] = KeyCode.Space;
			buttonKeys[Button02] = KeyCode.LeftCommand;
			buttonKeys[Button03] = KeyCode.LeftAlt;
			buttonKeys[Button04] = KeyCode.LeftControl;

			// Axis
			axisKeys[HorizontalAxis] = "Vertical";
			axisKeys[VerticalAxis] = "Horizontal";
			
			// Clear Inverts
			inverts.Clear();
			
			// Clear Joystick PlayerPrefs
			if (clearJoysitckPlayerPrefs)
				ClearPlayerPrefs();
			
			// Check for PlayerPrefs and replace values if available, else resets defaults
			if (playerPrefsExist) 
				LoadPlayerPrefs();
			
			// Check for getJoystickValuesToContinue, to ensure a joystick/keyboard is configured by user
			if (!playerPrefsExist && getJoystickValuesToContinue)
				inputSetter.SetActive(true);
		}

		public bool InputMasterGetButtonDown(string buttonName)
		{
			if (!buttonKeys.ContainsKey(buttonName))
			{
				Debug.LogError("InputMaster::GetButtonDown " + buttonName + " does not exist");
				return false;
			}

			return Input.GetKeyDown(buttonKeys[buttonName]);
		}

		public float InputMasterGetAxis(string axisName)
		{
			if (!axisKeys.ContainsKey(axisName))
			{
				Debug.LogError("InputMaster::GetAxis " + axisName + " does not exist");
				return 0;
			}

			// invert values if needed
			if (inverts.Contains(axisName))
			{
				return -Input.GetAxis(axisKeys[axisName]);
			}
			
			return Input.GetAxis(axisKeys[axisName]);
		}

		public void StorePlayerPrefs()
		{
			foreach (var buttonKey in buttonKeys)
			{
				PlayerPrefs.SetString(buttonKey.Key, buttonKey.Value.ToString());
			}
			foreach (var axisKey in axisKeys)
			{
				PlayerPrefs.SetString(axisKey.Key, axisKey.Value);
			}
			foreach (string invert in inverts)
			{
				PlayerPrefs.SetString(invert + "_invert", invert);
			}
		}

		private void LoadPlayerPrefs()
		{
			foreach (var buttonKey in buttonKeys.ToList())
			{
				if (PlayerPrefs.HasKey(buttonKey.Key))
				{
					var buttonName = PlayerPrefs.GetString(buttonKey.Key);

					KeyCode thisKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), buttonName);
					buttonKeys[buttonKey.Key] = thisKeyCode;
				}
				else
				{
					Debug.LogWarning("No PlayerPrefs Exist, Loading Defaults");
					playerPrefsExist = false;
					SetupJoystick();
					return;
				}
			}
			
			foreach (var axisKey in axisKeys.ToList())
			{
				if (PlayerPrefs.HasKey(axisKey.Key))
				{
					axisKeys[axisKey.Key] = PlayerPrefs.GetString(axisKey.Key);
				}
				else
				{
					Debug.LogWarning("No PlayerPrefs Exist, Loading Defaults");
					playerPrefsExist = false;
					SetupJoystick();
					return;
				}

				if (PlayerPrefs.HasKey(axisKey.Value + "_invert"))
				{
					inverts.Add(PlayerPrefs.GetString(axisKey.Value + "_invert"));
				}
			}
		}
		
		public void ClearPlayerPrefs()
		{
			foreach (var buttonKey in buttonKeys)
			{
				PlayerPrefs.DeleteKey(buttonKey.Key);
				PlayerPrefs.DeleteKey(buttonKey.Value + "_invert");
			}
			foreach (var axisKey in axisKeys)
			{
				PlayerPrefs.DeleteKey(axisKey.Key);
				PlayerPrefs.DeleteKey(axisKey.Value + "_invert");
			}
			
			// Clear Inverts
			inverts.Clear();
		}
	}
}