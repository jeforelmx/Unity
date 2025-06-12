using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class Password : MonoBehaviour
{

    public GameObject PasswordPrefab;
    public TMP_InputField passwordTxt;
    public static Password instance;
    public event Action<string> onPasswordChanged;

    public void PasswordChanged()
    {
        if (onPasswordChanged != null)
        {
            onPasswordChanged(passwordTxt.text);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
