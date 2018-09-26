using UnityEngine;
using UnityEngine.UI;
using JoyForAll;

public class InputMover : MonoBehaviour
{
    public Text displayAction;
    public string btnPress;
    
    private Vector3 mover;
    private InputMaster inputMaster;
    
    private void Awake()
    {
        inputMaster = FindObjectOfType<InputMaster>();
    }

    private void Update()
    {
        // Buttons
        if (inputMaster.InputMasterGetButtonDown(inputMaster.Button01))
        {
            btnPress = inputMaster.Button01;
        }
        
        if (inputMaster.InputMasterGetButtonDown(inputMaster.Button02))
        {
            btnPress = inputMaster.Button02;
        }

        if (inputMaster.InputMasterGetButtonDown(inputMaster.Button03))
        {
            btnPress = inputMaster.Button03;
        }
        
        if (inputMaster.InputMasterGetButtonDown(inputMaster.Button04))
        {
            btnPress = inputMaster.Button04;
        }
        
        // Display Button Action
        displayAction.text = btnPress;
        
        // Axis
        mover.x = inputMaster.InputMasterGetAxis(inputMaster.HorizontalAxis);
        mover.y = inputMaster.InputMasterGetAxis(inputMaster.VerticalAxis);
        
        // Move Player based on Axis Action
        transform.position += mover * Time.deltaTime;
    }    
}