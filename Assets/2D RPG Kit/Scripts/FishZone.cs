using UnityEngine;

public class FishingZone : MonoBehaviour
{
    [Header("区域设置")]
    public string regionName = "Hot"; // 区域名称，对应 FishData.region 字段
    public KeyCode fishingKey = KeyCode.F; // 触发钓鱼的按键
    public bool canFishHere = true; // 是否允许钓鱼

    [Header("引用")]
    public FishingManager fishingManager; // 引用你的钓鱼管理器
    public Transform inventoryParent; // 可选：用于 Item UI 显示的父物体

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(fishingKey) && canFishHere)
        {
            StartFishing();
        }
    }

    void StartFishing()
    {
        if (fishingManager != null)
        {
            // 设置当前区域
            fishingManager.SetCurrentRegion(regionName);

            // 启动钓鱼小游戏
            fishingManager.StartCatch();
        }
        else
        {
            Debug.LogError("FishingManager 未赋值！");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = canFishHere ? Color.blue : Color.gray;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}