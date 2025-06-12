using UnityEngine;

public class FishingZone : MonoBehaviour
{
    [Header("��������")]
    public string regionName = "Hot"; // �������ƣ���Ӧ FishData.region �ֶ�
    public KeyCode fishingKey = KeyCode.F; // ��������İ���
    public bool canFishHere = true; // �Ƿ��������

    [Header("����")]
    public FishingManager fishingManager; // ������ĵ��������
    public Transform inventoryParent; // ��ѡ������ Item UI ��ʾ�ĸ�����

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
            // ���õ�ǰ����
            fishingManager.SetCurrentRegion(regionName);

            // ��������С��Ϸ
            fishingManager.StartCatch();
        }
        else
        {
            Debug.LogError("FishingManager δ��ֵ��");
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