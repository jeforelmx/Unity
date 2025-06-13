using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.SceneManagement;
public class FishingManager : MonoBehaviour
{
    // UI Elements
    public GameObject minigamePanel;
    public RectTransform targetArea;
    public RectTransform hookPointer;
    public Slider staminaBar;

    public GameObject fishItemPrefab;

    // Fish Settings
    private FishData[] availableFishes;
    //private bool isDataLoaded = false;
    private FishData currentFish;
    private float currentStamina;
    public string currentRegion = "Hot"; // 可以是 Cold / Normal / Dead

    // Control Settings
    public float fallSpeed = 200f;
    public float riseSpeed = 400f;
    private bool isRising = false;

    // Timer Settings
    public float maxOutOfZoneTime = 3f;
    private float outOfZoneTimer = 0f;

    // Target Area Movement
    private Vector2 screenCenter;
    private Rect targetRect;
    private float targetMoveRange; // 动态调整的目标区域移动范围
    private float targetMoveSpeed;
    private float targetMoveTimer = 0f;
    private float targetDirection = 1f;
    private float randomOffsetY;

    private bool isCatching = false;
    private bool hasEscaped = false; // 新增标志位
    private bool hasSucceeded = false; // 新增标志位

    // Stamina Drain 设置
    public float baseStaminaDrain = 10f;         // 基础精力消耗速度（每秒）
    public float accelerationRate = 0.5f;        // 每秒增加的倍率
    public float maxStaminaDrainMultiplier = 5f; // 最大加速倍数（最大 x5）

    private float staminaDrainMultiplier = 1f;   // 当前倍率

    private bool hasHookSunk = false; // 是否已触发“沉底”失败
    void Start()
    {
        LoadFishData();
        StartCatch();
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    void Update()
    {

        if (isCatching)
        {
            HandleHookMovement();
            MoveTargetArea();
            CheckHookInTarget();
            UpdateStamina();
            CheckHookSunk(); // 新增这一行
        }
    }

    public void SetCurrentRegion(string region)
    {
        currentRegion = region;
    }
    void LoadFishData()
    {
        TextAsset csvText = Resources.Load<TextAsset>("fishdata");
        if (csvText == null)
        {
            Debug.LogError("无法找到 CSV 文件！");
            return;
        }

        List<FishData> fishList = new List<FishData>();
        string[] lines = csvText.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] fields = SplitCsvLine(line);
            if (fields.Length < 5) continue;

            FishData fishData = ScriptableObject.CreateInstance<FishData>();
            fishData.fishName = fields[0];
            fishData.rarity = int.Parse(fields[1]);
            fishData.stamina = float.Parse(fields[2]);
            fishData.region = fields[3];
            fishData.description = fields.Length > 4 ? fields[4] : "";

            fishList.Add(fishData);
        }

        availableFishes = fishList.ToArray();
    }

    private string[] SplitCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        StringBuilder current = new StringBuilder();

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result.ToArray();
    }

    public void StartCatch()
    {
        // 根据当前区域筛选鱼类
        List<FishData> filteredFishes = new List<FishData>();
        foreach (var fish in availableFishes)
        {
            if (fish.region == currentRegion)
            {
                filteredFishes.Add(fish);
            }
        }

        if (filteredFishes.Count == 0)
        {
            Debug.LogWarning($"没有找到 {currentRegion} 区域的鱼类！");
            return;
        }

        currentFish = filteredFishes[Random.Range(0, filteredFishes.Count)];
        currentStamina = currentFish.stamina;

        // 获取 RectTransform 组件
        RectTransform hookRect = hookPointer.GetComponent<RectTransform>();
        RectTransform targetAreaRect = targetArea.GetComponent<RectTransform>();

        // 设置钓钩初始位置：Canvas 中心上方 100 像素
        hookRect.anchoredPosition = new Vector2(0, 100);

        // 获取钓钩宽度并设置目标区域宽度为两倍
        float hookWidth = hookRect.rect.width;
        Vector2 areaSize = currentFish.GetTargetAreaSize();
        areaSize.x = hookWidth * 2f; // 目标区域宽度设为钓钩的两倍
        targetAreaRect.sizeDelta = areaSize;

        // 设置目标区域与钓钩水平居中对齐
        targetAreaRect.anchoredPosition = new Vector2(hookRect.anchoredPosition.x, 0);

        // 初始化屏幕中心点和目标区域 Rect（用于逻辑判断）
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        float newY = screenCenter.y - areaSize.y / 2;
        targetRect = GetWorldScreenRect(targetArea.GetComponent<RectTransform>());

        // 设置目标区域移动参数
        targetMoveRange = Random.Range(50f, 150f); // 可根据稀有度调整
        targetMoveSpeed = currentFish.GetMaxMoveSpeed();
        randomOffsetY = Random.Range(-targetMoveRange / 2, targetMoveRange / 2);

        // 显示 UI
        minigamePanel.SetActive(true);
        staminaBar.maxValue = currentStamina;
        staminaBar.value = currentStamina;

        // 重置状态
        isCatching = true;
        outOfZoneTimer = 0f;
        staminaDrainMultiplier = 1f;
        hasSucceeded = false;
        hasEscaped = false;
        hasHookSunk = false;
    }

    void HandleHookMovement()
    {
        if (!isRising)
        {
            hookPointer.position -= new Vector3(0, fallSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButtonDown(0))
        {
            isRising = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isRising = false;
        }

        if (isRising)
        {
            hookPointer.position += new Vector3(0, riseSpeed * Time.deltaTime);
        }
    }

    // 获取 Panel 的世界屏幕矩形范围（考虑缩放和锚点）
    private Rect GetWorldScreenRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        float left = corners[0].x;
        float right = corners[2].x;
        float bottom = corners[0].y;
        float top = corners[2].y;

        return new Rect(left, bottom, right - left, top - bottom);
    }
    void MoveTargetArea()
    {
        // 1. 更新移动计时器
        targetMoveTimer += Time.deltaTime * targetDirection;

        // 2. 使用 PingPong 实现上下循环运动 + 随机偏移
        float y = Mathf.PingPong(targetMoveTimer * targetMoveSpeed, targetMoveRange) - targetMoveRange / 2 + randomOffsetY;

        // 3. 计算目标 Y 坐标
        float newY = screenCenter.y - targetArea.rect.height / 2 + y;

        // 4. 获取 MinigamePanel 的 Rect 尺寸（注意：使用 RectTransform 的 rect 属性）
        Rect panelRect = GetWorldScreenRect(minigamePanel.GetComponent<RectTransform>());

        // 5. 边界限制：确保 TargetArea 不会跑出 Panel 范围
        float targetHeight = targetArea.rect.height;

        float minY = panelRect.yMin + targetHeight / 2;
        float maxY = panelRect.yMax - targetHeight / 2;

        newY = Mathf.Clamp(newY, minY, maxY);

        // 6. 更新位置
        targetArea.anchoredPosition = new Vector2(0, newY - screenCenter.y); // 锚点相对定位

        // 更新 targetRect
        targetRect = GetWorldScreenRect(targetArea.GetComponent<RectTransform>());
    }

    void CheckHookInTarget()
    {
        Vector2 hookPos = hookPointer.position;

        if (targetRect.Contains(hookPos))
        {
            outOfZoneTimer = 0f;
        }
        else
        {
            outOfZoneTimer += Time.deltaTime;
        }

        // 防止重复触发逃脱
        if (outOfZoneTimer >= maxOutOfZoneTime && !hasEscaped)
        {
            hasEscaped = true; // 设置标志位，防止重复执行
            EscapeFish();
        }
    }

    void UpdateStamina()
    {
        if (targetRect.Contains(hookPointer.position))
        {
            // 钩子在目标区域内，持续加速精力消耗
            staminaDrainMultiplier += accelerationRate * Time.deltaTime;
            staminaDrainMultiplier = Mathf.Clamp(staminaDrainMultiplier, 1f, maxStaminaDrainMultiplier);

            // 计算当前实际消耗速度
            float currentDrain = baseStaminaDrain * staminaDrainMultiplier;

            currentStamina -= currentDrain * Time.deltaTime;
            staminaBar.value = currentStamina;

            if (currentStamina <= 0 && !hasSucceeded)
            {
                hasSucceeded = true;
                CatchSuccess();
            }
        }
        else
        {
            // 钩子不在区域内，重置加速倍率
            staminaDrainMultiplier = 1f;
        }
    }

    public IEnumerator gotItemMessageCo()
    {
        yield return new WaitForSeconds(.5f);
        GameMenu.instance.gotItemMessage.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        GameMenu.instance.gotItemMessage.SetActive(false);

    }
    void CatchSuccess()
    {
        GameMenu.instance.gotItemMessageText.text = "得到 " + currentFish.fishName + " 辣！！!";
        // 实例化一个新的 Item 对象（从预制体）
        GameObject fishGO = Instantiate(fishItemPrefab); // 克隆预制体（包括所有字段）
        Item fishItem = fishGO.GetComponent<Item>();

        // 只修改名称，保留原有属性
        fishItem.itemName = currentFish.fishName;

        bool itemExists = false;
        for (int i = 0; i < GameManager.instance.existingItems.Length; i++)
        {
            if (GameManager.instance.existingItems[i].itemName == currentFish.fishName)
            {
                itemExists = true;

                i = GameManager.instance.existingItems.Length;
            }
        }

        if (!itemExists)
        {
            // 将数组转换为List
            List<Item> strList = new List<Item>(GameManager.instance.existingItems);

            // 添加新元素
            strList.Add(fishItem);

            // 将List转换回数组
            GameManager.instance.existingItems = strList.ToArray();
        }

        Shop.instance.selectedItem = fishItem;
        GameManager.instance.AddItem(fishItem.itemName);
        StartCoroutine(gotItemMessageCo());
        hasSucceeded = true;
        hasEscaped = true; // 防止逃脱和成功同时触发
        Invoke(nameof(CloseMinigame), 3.5f);
    }

    void EscapeFish()
    {
        if (hasEscaped) return; // 防止重复触发
        GameMenu.instance.gotItemMessageText.text = currentFish.fishName + " 逃跑了！！！";
        StartCoroutine(gotItemMessageCo());
        Debug.Log(currentFish.fishName + " 逃跑了！");
        hasEscaped = true;
        hasSucceeded = true; // 防止成功和逃脱同时触发
        Invoke(nameof(CloseMinigame), 3.5f);
    }

    void CloseMinigame()
    {
        CancelInvoke();

        minigamePanel.SetActive(false);
        isCatching = false;
        hasSucceeded = false;
        hasEscaped = false;
        hasHookSunk = false; // 新增：重置“沉底”状态
        outOfZoneTimer = 0f;

        SceneManager.LoadScene(GameManager.previousSceneName);
    }

    void HookSunk()
    {
        GameMenu.instance.gotItemMessageText.text = "你的鱼钩沉底了！！！";
        StartCoroutine(gotItemMessageCo());
        Debug.Log("你的鱼钩沉底了！");

        hasEscaped = true; // 阻止其他失败/成功逻辑同时触发
        Invoke(nameof(CloseMinigame), 3.5f);
    }
    void CheckHookSunk()
    {
        if (hasHookSunk) return;

        // 获取 MinigamePanel 的实际屏幕范围
        Rect panelRect = GetWorldScreenRect(minigamePanel.GetComponent<RectTransform>());

        float hookY = hookPointer.position.y;

        // 判断钩子是否掉出 Panel 底部
        if (hookY < panelRect.yMin)
        {
            hasHookSunk = true;
            HookSunk();
        }
    }
}

