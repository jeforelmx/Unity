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
            Debug.Log("靠近钓鱼点，按 F 开始钓鱼");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("离开钓鱼点");
        }
    }
}