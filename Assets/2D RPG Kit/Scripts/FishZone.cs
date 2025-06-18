// LakeFishingSpot.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class LakeFishingSpot : MonoBehaviour
{
    private bool isPlayerNearby = false;
    public string currentRegion = "Normal";

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            GameManager.LoadFishingScene(currentRegion);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("��������㣬�� F ��ʼ����");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("�뿪�����");
        }
    }
}