using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

public class DialogChoiceButton : MonoBehaviour
{
    public int index;
    private bool isPressed = false;
    public void Press()
    {
        if (!isPressed)
        {
            DialogManager.instance.SelectDialogChoice(index);
            isPressed = true;
        }        
    }

    private void OnEnable()
    {
        isPressed = false;
    }
}
