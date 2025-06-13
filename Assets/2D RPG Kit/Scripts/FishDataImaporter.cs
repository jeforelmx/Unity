using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class FishItemImporter : EditorWindow
{
    private TextAsset csvFile;
    private string outputFolder = "/2D RPG Kit/Prefabs/Items/New Fish";

    [MenuItem("Tools/Fish Item + Data Importer")]
    public static void ShowWindow()
    {
        GetWindow<FishItemImporter>("Fish Item + Data Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV 鱼类数据导入器", EditorStyles.boldLabel);
        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV 文件", csvFile, typeof(TextAsset), false);

        if (GUILayout.Button("开始导入"))
        {
            if (csvFile != null)
            {
                ImportCSV(csvFile);
                EditorUtility.DisplayDialog("完成", "鱼类 Item + FishData 已成功导入！", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "请选择一个 CSV 文件！", "OK");
            }
        }
    }

    private void ImportCSV(TextAsset csvText)
    {
        string dataPath = Path.Combine("Assets", outputFolder);
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        string[] lines = csvText.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] fields = SplitCsvLine(line);
            if (fields.Length < 5) continue;

            string fishName = fields[0];
            int rarity = int.Parse(fields[1]);
            float stamina = float.Parse(fields[2]);
            string region = fields[3];
            string description = fields.Length > 4 ? fields[4] : "";

            // 创建 GameObject
            GameObject fishGO = new GameObject(fishName);

            // 添加 FishData 组件（ScriptableObject 实例化）
            FishData fishData = ScriptableObject.CreateInstance<FishData>();
            fishData.fishName = fishName;
            fishData.rarity = rarity;
            fishData.stamina = stamina;
            fishData.region = region;
            fishData.description = description;

            // 将 FishData 附加到 GameObject 上（可选：作为组件或引用）
            // 这里我们只保存为字段值供 Item 使用
            // 或者你可以创建一个 FishDataHolder 组件来挂载

            // 添加 Item 组件
            Item item = fishGO.AddComponent<Item>();

            // 设置 Item 数据
            item.item = true;
            item.itemName = fishName;
            item.description = description;
            item.price = Mathf.RoundToInt(100 + rarity * 100);
            item.sellPrice = item.price / 2;

            item.raiseExp = true;
            item.amountToChange = CalculateExpByRarity(rarity);

            // 关闭其他选项
            item.affectHP = false;
            item.affectMP = false;
            item.raiseAgility = false;
            item.raiseOffense = false;
            item.raiseDefense = false;

            // 按区域分文件夹存储
            string regionFolder = Path.Combine(dataPath, region);
            if (!Directory.Exists(regionFolder))
            {
                Directory.CreateDirectory(regionFolder);
            }

            string prefabPath = Path.Combine(regionFolder, fishName + ".prefab");

            // 创建预制体
            PrefabUtility.SaveAsPrefabAsset(fishGO, prefabPath);
            DestroyImmediate(fishGO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private int CalculateExpByRarity(int rarity)
    {
        switch (rarity)
        {
            case 0: return Random.Range(10, 21);   // 普通
            case 1: return Random.Range(30, 51);   // 稀有
            case 2: return Random.Range(60, 101);  // 最稀有
            default: return 10;
        }
    }

    // 处理带引号的 CSV 行
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
}