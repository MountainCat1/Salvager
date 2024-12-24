using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum Characteristics
{
    Strength,
    Dexterity,
    Intelligence,
    Constitution
}

public static class CharacteristicsConsts {
    public const float SpeedAdditiveMultiplierPerDexterity = 0.2f;
    public const float AttackSpeedAdditiveMultiplierPerDexterity = 0.1f;
    
    public const float MaxHealthPerConstitution = 10f;
    public const float DragReductionPerConstitution = 0.1f;
    
    public const float DamageAdditiveMultiplierPerStrength = 0.4f;
    public const float PushForceAdditiveMultiplierPerStrength = 0.4f;
}

public interface ILevelSystem
{
    event Action ChangedXp;
    event Action ChangedLevel;
    event Action CharacteristicsChanged;
    public int Level { get; }
    public float LevelProgress { get; }
    public int PointsToUse { get; }
    void UpgradeCharacteristic(Characteristics characteristic);
    Dictionary<Characteristics, int> CharacteristicsLevels { get; }
}

public class LevelSystem : ILevelSystem
{
    public const int PointsPerLevel = 1;
    
    public event Action ChangedXp;
    public event Action ChangedLevel;
    public event Action CharacteristicsChanged;
    public int Level { get; private set; }
    public float LevelProgress => GetLevelProgress();

    public int PointsToUse { get; private set; }

    public int Xp { get; private set; }

    public Dictionary<Characteristics, int> CharacteristicsLevels { get; } = new()
    {
        { Characteristics.Strength, 0 },
        { Characteristics.Dexterity, 0 },
        { Characteristics.Intelligence, 0 },
        { Characteristics.Constitution, 0 }
    };

    public void AddXp(int xp)
    {
        Xp += xp;

        for (int i = 0; i < LevelThresholds.Length; i++)
        {
            if (Xp < LevelThresholds[i])
                break;

            if (Level < i)
            {
                Level = i;
                PointsToUse += PointsPerLevel;
                ChangedLevel?.Invoke();
                Debug.Log($"Level up! New level: {Level}");
            }
        }
        
        ChangedXp?.Invoke();
    }
    
    public void UpgradeCharacteristic(Characteristics characteristic)
    {
        if (PointsToUse == 0)
        {
            Debug.LogError("No points to use");
            return;
        }
        
        CharacteristicsLevels[characteristic]++;
        PointsToUse--;

        Debug.Log($"Upgraded {characteristic} to {CharacteristicsLevels[characteristic]}");
        CharacteristicsChanged?.Invoke();
    }
    
    private int[] LevelThresholds = new int[]
    {
        0, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576
    };
    
    private float GetLevelProgress()
    {
        if(Level == 0)
            return Xp / (float)LevelThresholds[1];
        
        var a = Xp - LevelThresholds[Level];
        var b = (float)LevelThresholds[Level + 1] - LevelThresholds[Level];
        
        return a / b;
    }
}