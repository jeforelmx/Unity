using UnityEngine;

[CreateAssetMenu(fileName = "NewFishData", menuName = "Fishing/Fish Data", order = 0)]
public class FishData : ScriptableObject
{
    public string fishName;
    public int rarity; // 0=普通, 1=稀有, 2=最稀有
    public float stamina;
    public string region; // Hot / Cold / Normal / Dead
    public Sprite icon;
    [TextArea]
    public string description;

    public Vector2 GetTargetAreaSize()
    {
        switch (rarity)
        {
            case 0: return new Vector2(30, 100);
            case 1: return new Vector2(30, 60);
            case 2: return new Vector2(30, 30);
            default: return new Vector2(30, 100);
        }
    }

    public float GetMaxMoveSpeed()
    {
        switch (rarity)
        {
            case 0: return 50f;
            case 1: return 80f;
            case 2: return 120f;
            default: return 50f;
        }
    }
}