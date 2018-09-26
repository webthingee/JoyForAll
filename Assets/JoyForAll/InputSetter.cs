using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JoyForAll;

public class InputSetter : MonoBehaviour
{
	public Text messageArea;
	public GameObject messageAreaBackground;
	
	private bool bindingInProgress;
	private InputMaster inputMaster;
	
	private enum BindingAction { Request, Set }
	private enum BindingType { KeyCode, Axis, Either }

	private void OnEnable()
	{
		inputMaster = FindObjectOfType<InputMaster>();
		inputMaster.ClearPlayerPrefs();
		StartCoroutine(StartReBinding());
	}

	private IEnumerator StartReBinding()
	{
		yield return new WaitForSeconds(2f);

		yield return StartCoroutine(RebindToThisAction(inputMaster.Button01, BindingType.KeyCode, "Jumping"));
		yield return StartCoroutine(RebindToThisAction(inputMaster.Button02, BindingType.KeyCode, "Attacking"));
		yield return StartCoroutine(RebindToThisAction(inputMaster.Button03, BindingType.KeyCode, "Dodging"));
		yield return StartCoroutine(RebindToThisAction(inputMaster.Button04, BindingType.KeyCode, "Crouching"));
		
		yield return StartCoroutine(RebindToThisAction(inputMaster.HorizontalAxis, BindingType.Axis, "Moving Right"));
		yield return StartCoroutine(RebindToThisAction(inputMaster.VerticalAxis, BindingType.Axis, "Moving Up"));

		inputMaster.StorePlayerPrefs();
		gameObject.SetActive(false);
	}

	private IEnumerator RebindToThisAction(string rebind, BindingType type = BindingType.Either, string buttonReadableName = "")
	{
		// begin rebind process
		bindingInProgress = true;
		
		// use the internal buttonName if no name is specified
		var btnName = (buttonReadableName != "") ? buttonReadableName : rebind;
		// message to display requesting input from the user
		var msg = "Set your desired input for " + btnName;
		
		//Debug.Log(msg);
		MessageAreaStatus(BindingAction.Request, msg);
		
		while (bindingInProgress)
		{
			ExecuteBind(rebind, type);
			// when completed, will set bindingInProgress to false and end the while loop
			yield return null;
		}

		// after input, provide user feedback for their action
		msg = "Setting " + btnName;
		MessageAreaStatus(BindingAction.Set, msg);
		
		// provide a little time between actions
		yield return new WaitForSeconds(0.75f);
	}
	
	private void ExecuteBind(string rebindString, BindingType type = BindingType.Either)
	{		
		// listen for any key to be pressed by the user
		if (type != BindingType.Axis && Input.anyKeyDown)
		{
			// list of possible keys that can be pressed
			var kcsSystem = Enum.GetValues(typeof(KeyCode));
			
			// convert to a Unity readable array 
			KeyCode[] kcs = new KeyCode[kcsSystem.Length];
			for (int i = 0; i < kcsSystem.Length; i++)
			{
				kcs[i] = (KeyCode)kcsSystem.GetValue(i);
			}
	
			// execute when a key is pressed
			foreach (KeyCode kc in kcs)
			{
				if (!Input.GetKey(kc)) continue;
				inputMaster.buttonKeys[rebindString] = kc;
				bindingInProgress = false;
			}
		}

		// listen for any axis input from the user
		for (int i = 1; i < 29; i++)
		{
			string axisName = "Axis " + i;
			if (type != BindingType.KeyCode && Math.Abs(Input.GetAxis(axisName)) > 0.1f)
			{
				inputMaster.axisKeys[rebindString] = axisName;
				if (Input.GetAxis(axisName) < 0)
				{
					// check if the value needs to be inverted, list it in the InputMaster
					inputMaster.inverts.Add(rebindString);
				}
				
				bindingInProgress = false;
			}
		}
	}

	// user messaging
	private void MessageAreaStatus(BindingAction action, string msg)
	{	
		switch (action)
		{
			case BindingAction.Request:
				messageAreaBackground.GetComponent<Image>().color = Color.green;
				break;
			case BindingAction.Set:
				messageAreaBackground.GetComponent<Image>().color = Color.yellow;
				break;
			default:
				msg = "Error: " + msg;
				messageAreaBackground.GetComponent<Image>().color = Color.red;
				break;
		}
		
		messageArea.text = msg;
	}
}