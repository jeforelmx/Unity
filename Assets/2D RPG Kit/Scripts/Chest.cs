using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Threading;

//Adds a BoxCollider2D component automatically to the game object
[RequireComponent(typeof(BoxCollider2D))]

public class Chest : MonoBehaviour
{
    public ChestObjectActivator openObject;
    public ChestObjectActivator closedObject;

    [Header("Initialization")]
    //Game objects used by this code
    public GameObject open;
    public GameObject closed;
    [Tooltip("Assign a unique ID to this chest. Also register this ID in the Chest Manager")]
    public string chestID;
    [Tooltip("Enter the number corresponding to the sound effect from the Audio Manager")]
    public int openSound;
    [Tooltip("Enter the number corresponding to the sound effect from the Audio Manager")]
    public int collectSound;

    public bool passwordenable;
    public string password;
    [HideInInspector]
    public int numberOfItemsHeld;
    [HideInInspector]
    public int numberOfEquipItemsHeld;

    [Header("Item Settings")]
    [Tooltip("Chest contains an item")]
    public bool item;
    [Tooltip("Drag and drop an item prefab")]
    public Item addItem;

    [Header("Gold Settings")]
    [Tooltip("Chest contains gold")]
    public bool gold;
    [Tooltip("The amount of gold found in this chest")]
    public int addGoldAmount;

    private bool isClosed = true;
    private bool canActivate;

    public UnityEvent onOpenChest;

    private void Awake()
    {
        closedObject.chestToCheck = chestID;
        openObject.chestToCheck = chestID;
        openObject.activeIfComplete = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("RPGConfirmPC") || Input.GetButtonDown("RPGConfirmJoy") || CrossPlatformInputManager.GetButton("RPGConfirmTouch") || CrossPlatformInputManager.GetButtonUp("RPGConfirmTouch"))
        {
            if (canActivate && !DialogManager.instance.dialogBox.activeInHierarchy && !Inn.instance.innMenu.activeInHierarchy && !GameMenu.instance.menu.activeInHierarchy)
            {
                if(isClosed && !open.activeInHierarchy && !GameManager.instance.battleActive)
                {
                    if (passwordenable)
                    {
                        Password.instance.PasswordPrefab.SetActive(true);
                        return;
                    }
                    else {
                        OpenChest();
                    }
                    //if(passwordpass)
                    //    OpenChest();
                }                
            }
        }
    }

    private void OpenChest()
    {
        onOpenChest?.Invoke();

        if (item)
        {
            //Take the reference for isItem/isWeapon/isArmour from shop instance
            Shop.instance.selectedItem = addItem;
        }

        //Calculate the amount of items / equipment held in inventory to prevent adding more items if inventory is full
        numberOfItemsHeld = 0;
        numberOfEquipItemsHeld = 0;

        for (int i = 0; i < GameManager.instance.itemsHeld.Length; i++)
        {
            if (GameManager.instance.itemsHeld[i] != "")
            {
                numberOfItemsHeld++;
            }
        }

        for (int i = 0; i < GameManager.instance.equipItemsHeld.Length; i++)
        {
            if (GameManager.instance.equipItemsHeld[i] != "")
            {
                numberOfEquipItemsHeld++;
            }
        }

        GameMenu.instance.gotItemMessageText.text = "得到 ";

        if (item)
        {
            if (Shop.instance.selectedItem.item)
            {
                if (numberOfItemsHeld < GameManager.instance.itemsHeld.Length)
                {
                    isClosed = false;
                    GameMenu.instance.gotItemMessageText.text += addItem.itemName;
                    StartCoroutine(gotItemMessageCo());
                    //spriteRenderer.sprite = open;
                    open.SetActive(true);
                    closed.SetActive(false);
                    GameManager.instance.AddItem(addItem.itemName);
                    AudioManager.instance.PlaySFX(openSound);
                    ChestManager.instance.MarkChestOpened(chestID);
                }
                else
                {
                    Shop.instance.promptText.text = "发现一个 " + Shop.instance.selectedItem.name + "." + "\n" + "但是背包已经满了!";
                    StartCoroutine(Shop.instance.PromptCo());
                }

            }

            if (Shop.instance.selectedItem.defense || Shop.instance.selectedItem.offense)
            {
                if (numberOfEquipItemsHeld < GameManager.instance.equipItemsHeld.Length)
                {
                    isClosed = false;
                    GameMenu.instance.gotItemMessageText.text += addItem.itemName;
                    StartCoroutine(gotItemMessageCo());
                    //spriteRenderer.sprite = open;
                    open.SetActive(true);
                    closed.SetActive(false);
                    GameManager.instance.AddItem(addItem.itemName);
                    AudioManager.instance.PlaySFX(openSound);
                    ChestManager.instance.MarkChestOpened(chestID);
                }
                else
                {
                    Shop.instance.promptText.text = "发现一个 " + Shop.instance.selectedItem.name + "." + "\n" + "但是背包已经满了!";
                    StartCoroutine(Shop.instance.PromptCo());
                }

            }
        }

        if (gold)
        {
            isClosed = false;
            GameMenu.instance.gotItemMessageText.text += ("  " +addGoldAmount + " 能量碎片!");
            StartCoroutine(gotItemMessageCo());
            //spriteRenderer.sprite = open;
            open.SetActive(true);
            closed.SetActive(false);
            GameManager.instance.currentGold += addGoldAmount;
            AudioManager.instance.PlaySFX(openSound);
            ChestManager.instance.MarkChestOpened(chestID);

        }
    }

    // 密码验证方法
    public void ValidatePassword(string input)
    {
        if (input == password)
        {
            // 密码正确
            Debug.Log("密码正确" + password + Password.instance.passwordTxt.text);
            Password.instance.PasswordPrefab.SetActive(false); // 隐藏密码输入界面
            OpenChest();
        }
        else
        {
            // 密码错误
            Debug.Log("密码错误，请重试。初始密码：" + password + "输入密码" + Password.instance.passwordTxt.text);
            // 可选：在 UI 中显示错误消息提示用户密码错误
            Password.instance.passwordTxt.text = ""; // 清空输入框
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canActivate = true;
            if(passwordenable)
                Password.instance.onPasswordChanged += ValidatePassword;
            if (DialogManager.instance == null)
                Debug.Log("null!!!");
            DialogManager.instance.dontOpenDialogAgain = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canActivate = false;
            if (passwordenable)
                Password.instance.onPasswordChanged -= ValidatePassword;
        }
    }

    public IEnumerator gotItemMessageCo()
    {
        //GameManager.instance.gameMenuOpen = true;
        yield return new WaitForSeconds(.5f);
        //GameManager.instance.gameMenuOpen = false;
        AudioManager.instance.PlaySFX(collectSound);
        GameMenu.instance.gotItemMessage.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        GameMenu.instance.gotItemMessage.SetActive(false);
        
    }
}
