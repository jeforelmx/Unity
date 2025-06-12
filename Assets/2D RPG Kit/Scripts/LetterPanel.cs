using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class LetterPanel : MonoBehaviour
{
    public GameObject panel;
    public Text content;
    public Button prevButton; // “上一页”按钮
    public Button nextButton; // “下一页”按钮

    private string[] pages; // 存储分页后的文本数组
    private int currentPageIndex = 0; // 当前显示的页面索引
    // Start is called before the first frame updatemieishi
    void Start()
    {
        // 示例文本，应替换为你的实际文本内容
        string longText = content.text;
        Debug.Log(longText);
        // 假设每页显示一定字符数（例如每页100个字符）
        pages = SplitTextIntoPages(longText, 200);

        // 初始设置按钮的状态
        UpdateButtonStatus();

        // 显示第一页的文本
        if (pages.Length > 0)
        {
            content.text = pages[currentPageIndex];
        }
    }

    public void OnItemSelected()
    {
        string longText = content.text;
        Debug.Log(longText);
        // 假设每页显示一定字符数（例如每页100个字符）
        pages = SplitTextIntoPages(longText, 200);

        // 初始设置按钮的状态
        UpdateButtonStatus();
        currentPageIndex = 0;
        // 显示第一页的文本
        if (pages.Length > 0)
        {
            content.text = pages[currentPageIndex];
        }
    }

    // 分割文本为多个页面
    private string[] SplitTextIntoPages(string text, int charsPerPage)
    {
        string[] words = text.Split('/');
        Debug.Log(words.Length);
        string page = "";
        var pagesList = new System.Collections.Generic.List<string>();
        if (words.Length == 1)
        {
            pagesList.Add(words[0]);
        }
        else
        {
            foreach (var word in words)
            {
                if (page.Length + word.Length < charsPerPage)
                {
                    page += word + " ";
                }
                else
                {
                    pagesList.Add(page);
                    page = word + " ";
                }
            }
            pagesList.Add(page); // 添加最后一个页面
        }

        return pagesList.ToArray();
    }

    // 更新按钮的启用状态
    private void UpdateButtonStatus()
    {
        prevButton.interactable = currentPageIndex > 0;
        nextButton.interactable = currentPageIndex < pages.Length - 1;
    }

    public void OnPrevPage()
    {
        StartCoroutine(PrevPage());
    }

    private IEnumerator PrevPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            content.text = pages[currentPageIndex];
            UpdateButtonStatus();
            yield return new WaitForSeconds(1.5f);
        }
        yield return null;
    }

    public void OnNextPage()
    {
        StartCoroutine(NextPageCo());
    }

    private IEnumerator NextPageCo()
    {
        Debug.Log($"Button clicked at {DateTime.Now}");
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            content.text = pages[currentPageIndex];
            UpdateButtonStatus();
            yield return new WaitForSeconds(1.5f);
        }
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the dialo box is shown by the DialogActivator script
        if (panel.activeInHierarchy)
        {
            // 使用 KeyCode 检测左右键的按下
            if (Input.GetKeyDown(KeyCode.LeftArrow) && prevButton.interactable)
            {
                OnPrevPage();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && nextButton.interactable)
            {
                Debug.Log($"Button clicked at {DateTime.Now}");
                OnNextPage();
            }
        }
    }
}