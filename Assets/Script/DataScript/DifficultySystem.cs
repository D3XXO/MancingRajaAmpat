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
    float GetDynamicMinSpawnInterval(int totalValueScore);
    float GetDynamicMaxSpawnInterval(int totalValueScore);
    int GetMaxStreakMultiplier();
}

public class EasyDifficulty : IDifficultyStrategy
{
    public float GetDynamicMoveSpeed(int totalValueScore)
    {
        if (totalValueScore <= 1000)
            return Mathf.Lerp(4f, 12f, Mathf.Clamp(totalValueScore, 0f, 1000f) / 1000f);
        return 12f + ((totalValueScore - 1000) / 50) * 0.1f;
    }

    public float GetDynamicMinSpawnInterval(int totalValueScore)
    {
        return Mathf.Lerp(0.8f, 0.4f, Mathf.Clamp(totalValueScore, 0f, 1000f) / 1000f);
    }

    public float GetDynamicMaxSpawnInterval(int totalValueScore)
    {
        return Mathf.Lerp(1.5f, 1.0f, Mathf.Clamp(totalValueScore, 0f, 1000f) / 1000f);
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
        if (totalValueScore <= 650)
            return Mathf.Lerp(5f, 12f, Mathf.Clamp(totalValueScore, 0f, 650f) / 650f);
        return 12f + ((totalValueScore - 650) / 50) * 0.2f;
    }

    public float GetDynamicMinSpawnInterval(int totalValueScore)
    {
        return Mathf.Lerp(0.5f, 0.3f, Mathf.Clamp(totalValueScore, 0f, 650f) / 650f);
    }

    public float GetDynamicMaxSpawnInterval(int totalValueScore)
    {
        return Mathf.Lerp(0.8f, 0.8f, Mathf.Clamp(totalValueScore, 0f, 650f) / 650f);
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
            return Mathf.Lerp(6f, 12f, Mathf.Clamp(totalValueScore, 0f, 500f) / 500f);
        return 12f + ((totalValueScore - 500) / 50) * 0.2f;
    }

    public float GetDynamicMinSpawnInterval(int totalValueScore)
    {
        return Mathf.Lerp(0.3f, 0.2f, Mathf.Clamp(totalValueScore, 0f, 500f) / 500f);
    }

    public float GetDynamicMaxSpawnInterval(int totalValueScore)
    {
        return Mathf.Lerp(0.5f, 0.3f, Mathf.Clamp(totalValueScore, 0f, 500f) / 500f);
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