using UnityEngine;

public enum DifficultyLevel
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public interface IDifficultyStrategy
{
    float GetDynamicMoveSpeed(int totalValueScore);
    float GetBPM(int totalValueScore);
    int GetMaxStreakMultiplier();
}

public class EasyDifficulty : IDifficultyStrategy
{
    public float GetDynamicMoveSpeed(int totalValueScore)
    {
        if (totalValueScore <= 1000)
        return Mathf.Lerp(4f, 8f, Mathf.Clamp(totalValueScore, 0f, 1000f) / 1000f);
        return 8f + ((totalValueScore - 1000) / 50) * 0.1f;
    }

    public float GetBPM(int totalValueScore)
    {
        return Mathf.Lerp(120f, 180f, Mathf.Clamp(totalValueScore, 0f, 1000f) / 1000f);
    }

    public int GetMaxStreakMultiplier()
    {
        return 1;
    }
}

public class MediumDifficulty : IDifficultyStrategy
{
    public float GetDynamicMoveSpeed(int totalValueScore)
    {
        if (totalValueScore <= 750)
        return Mathf.Lerp(5f, 9f, Mathf.Clamp(totalValueScore, 0f, 750f) / 750f);
        return 9f + ((totalValueScore - 750) / 50) * 0.2f;
    }

    public float GetBPM(int totalValueScore)
    {
        return Mathf.Lerp(160f, 250f, Mathf.Clamp(totalValueScore, 0f, 750f) / 750f);
    }

    public int GetMaxStreakMultiplier()
    {
        return 5;
    }
}

public class HardDifficulty : IDifficultyStrategy
{
    public float GetDynamicMoveSpeed(int totalValueScore)
    {
        if (totalValueScore <= 500)
        return Mathf.Lerp(6f, 10f, Mathf.Clamp(totalValueScore, 0f, 500f) / 500f);
        return 10f + ((totalValueScore - 500) / 50) * 0.3f;
    }

    public float GetBPM(int totalValueScore)
    {
        return Mathf.Lerp(200f, 300f, Mathf.Clamp(totalValueScore, 0f, 500f) / 500f);
    }

    public int GetMaxStreakMultiplier()
    {
        return int.MaxValue;
    }
}

public static class DifficultyManager
{
    private const string DifficultyKey = "SelectedDifficulty";

    public static void SetDifficulty(DifficultyLevel level)
    {
        PlayerPrefs.SetInt(DifficultyKey, (int)level);
        PlayerPrefs.Save();
    }

    public static bool HasSelectedDifficulty()
    {
        return PlayerPrefs.HasKey(DifficultyKey);
    }

    public static IDifficultyStrategy GetCurrentStrategy()
    {
        DifficultyLevel level = (DifficultyLevel)PlayerPrefs.GetInt(DifficultyKey, (int)DifficultyLevel.Medium);
        switch (level)
        {
            case DifficultyLevel.Easy: return new EasyDifficulty();
            case DifficultyLevel.Hard: return new HardDifficulty();
            case DifficultyLevel.Medium:
            default: return new MediumDifficulty();
        }
    }

    public static DifficultyLevel GetCurrentLevel()
    {
        return (DifficultyLevel)PlayerPrefs.GetInt(DifficultyKey, (int)DifficultyLevel.Medium);
    }
}