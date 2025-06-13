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
        GUILayout.Label("CSV �������ݵ�����", EditorStyles.boldLabel);
        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV �ļ�", csvFile, typeof(TextAsset), false);

        if (GUILayout.Button("��ʼ����"))
        {
            if (csvFile != null)
            {
                ImportCSV(csvFile);
                EditorUtility.DisplayDialog("���", "���� Item + FishData �ѳɹ����룡", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("����", "��ѡ��һ�� CSV �ļ���", "OK");
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

            // ���� GameObject
            GameObject fishGO = new GameObject(fishName);

            // ��� FishData �����ScriptableObject ʵ������
            FishData fishData = ScriptableObject.CreateInstance<FishData>();
            fishData.fishName = fishName;
            fishData.rarity = rarity;
            fishData.stamina = stamina;
            fishData.region = region;
            fishData.description = description;

            // �� FishData ���ӵ� GameObject �ϣ���ѡ����Ϊ��������ã�
            // ��������ֻ����Ϊ�ֶ�ֵ�� Item ʹ��
            // ��������Դ���һ�� FishDataHolder ���������

            // ��� Item ���
            Item item = fishGO.AddComponent<Item>();

            // ���� Item ����
            item.item = true;
            item.itemName = fishName;
            item.description = description;
            item.price = Mathf.RoundToInt(100 + rarity * 100);
            item.sellPrice = item.price / 2;

            item.raiseExp = true;
            item.amountToChange = CalculateExpByRarity(rarity);

            // �ر�����ѡ��
            item.affectHP = false;
            item.affectMP = false;
            item.raiseAgility = false;
            item.raiseOffense = false;
            item.raiseDefense = false;

            // ��������ļ��д洢
            string regionFolder = Path.Combine(dataPath, region);
            if (!Directory.Exists(regionFolder))
            {
                Directory.CreateDirectory(regionFolder);
            }

            string prefabPath = Path.Combine(regionFolder, fishName + ".prefab");

            // ����Ԥ����
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
            case 0: return Random.Range(10, 21);   // ��ͨ
            case 1: return Random.Range(30, 51);   // ϡ��
            case 2: return Random.Range(60, 101);  // ��ϡ��
            default: return 10;
        }
    }

    // ��������ŵ� CSV ��
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