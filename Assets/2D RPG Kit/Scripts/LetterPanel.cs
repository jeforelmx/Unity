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
    public Button prevButton; // ����һҳ����ť
    public Button nextButton; // ����һҳ����ť

    private string[] pages; // �洢��ҳ����ı�����
    private int currentPageIndex = 0; // ��ǰ��ʾ��ҳ������
    // Start is called before the first frame updatemieishi
    void Start()
    {
        // ʾ���ı���Ӧ�滻Ϊ���ʵ���ı�����
        string longText = content.text;
        Debug.Log(longText);
        // ����ÿҳ��ʾһ���ַ���������ÿҳ100���ַ���
        pages = SplitTextIntoPages(longText, 200);

        // ��ʼ���ð�ť��״̬
        UpdateButtonStatus();

        // ��ʾ��һҳ���ı�
        if (pages.Length > 0)
        {
            content.text = pages[currentPageIndex];
        }
    }

    public void OnItemSelected()
    {
        string longText = content.text;
        Debug.Log(longText);
        // ����ÿҳ��ʾһ���ַ���������ÿҳ100���ַ���
        pages = SplitTextIntoPages(longText, 200);

        // ��ʼ���ð�ť��״̬
        UpdateButtonStatus();
        currentPageIndex = 0;
        // ��ʾ��һҳ���ı�
        if (pages.Length > 0)
        {
            content.text = pages[currentPageIndex];
        }
    }

    // �ָ��ı�Ϊ���ҳ��
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
            pagesList.Add(page); // ������һ��ҳ��
        }

        return pagesList.ToArray();
    }

    // ���°�ť������״̬
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
            // ʹ�� KeyCode ������Ҽ��İ���
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