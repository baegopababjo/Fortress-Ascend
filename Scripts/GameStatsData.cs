using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStatsData", menuName = "Game Data/Game Stats Data")]
public class GameStatsData : ScriptableObject
{
    //각 스텟들을 저장하는 장소
    //만약 스텟을 바꾸게 되면 Asset/GameData/GameStatsData.asset 파일을 삭제 하고 새로 만들어야 함.
    //추가 방법 : Asset/GameData 폴더 우클릭 -> Create → Game Data → Game Stats Data 추가. 이름 : GameStatsData
    public enum NPCClass { Warrior, Archer }
    public enum T_NPCclass { Crossbow, Cannon }

    [System.Serializable]
    public class WaveStats
    {
        public int waveNumber;  // 웨이브 번호
        public int enemyLevel;  // 적 레벨
        public int enemyCount;  // 적 수
        public int timeLimit;   // 제한 시간 (초)
    }


    [System.Serializable]
    public class NPCClassStats
    {
        public NPCClass npcClass;
        public int attackBonus;
        public int range;
        public float cooldown;
    }
    public class T_NPCclassStats
    {
        public T_NPCclass TnpcClass;
        public int attackBonus;
        public int range;
        public float cooldown;
    }

    [System.Serializable]
    public class LevelStats
    {
        public int level;
        public int health;
        public int baseAttackPower;
        public int buyCost;
    }

    [System.Serializable]
    public class WeaponStats
    {
        public PlayerStats.WeaponMode weapon;
        public int damage;
        public float range;
        public float cooldown;
    }

    [System.Serializable]
    public class MagicStats
    {
        public PlayerStats.MagicMode magic;
        public int bonusDamage;
        public float bonusRange;
        public float cooldown;
    }

    // 🎯 빌딩 레벨별 체력과 업그레이드 비용 저장
    [System.Serializable]
    public class BuildingLevel
    {
        public int health;       // 💟 레벨별 체력
        public int upgradeCost;  // 🪙 레벨 업그레이드 비용
        public int purchaseCost;
    }

    // 🎯 빌딩 정보를 저장하는 클래스
    [System.Serializable]
    public class BuildingStats
    {
        public string buildingName;
        public BuildingLevel[] levels;
    }

    // 🎯 웨이브별 적 레벨 및 수량
    public List<WaveStats> waveStatsList = new()
    {   
        new() { waveNumber = 1, enemyLevel = 1, enemyCount = 6, timeLimit = 45 },
        new() { waveNumber = 2, enemyLevel = 1, enemyCount = 8, timeLimit = 45 },
        new() { waveNumber = 3, enemyLevel = 1, enemyCount = 10, timeLimit = 45 },
        new() { waveNumber = 4, enemyLevel = 1, enemyCount = 12, timeLimit = 45 },
        new() { waveNumber = 5, enemyLevel = 1, enemyCount = 15, timeLimit = 45 },
        new() { waveNumber = 6, enemyLevel = 2, enemyCount = 18, timeLimit = 75 },
        new() { waveNumber = 7, enemyLevel = 2, enemyCount = 21, timeLimit = 75 },
        new() { waveNumber = 8, enemyLevel = 2, enemyCount = 24, timeLimit = 75 },
        new() { waveNumber = 9, enemyLevel = 2, enemyCount = 27, timeLimit = 75 },
        new() { waveNumber = 10, enemyLevel = 2, enemyCount = 31, timeLimit = 75 },
        new() { waveNumber = 11, enemyLevel = 3, enemyCount = 35, timeLimit = 115 },
        new() { waveNumber = 12, enemyLevel = 3, enemyCount = 39, timeLimit = 115 },
        new() { waveNumber = 13, enemyLevel = 3, enemyCount = 43, timeLimit = 115 },
        new() { waveNumber = 14, enemyLevel = 3, enemyCount = 47, timeLimit = 115 },
        new() { waveNumber = 15, enemyLevel = 3, enemyCount = 52, timeLimit = 115 },
        new() { waveNumber = 16, enemyLevel = 4, enemyCount = 57, timeLimit = 165 },
        new() { waveNumber = 17, enemyLevel = 4, enemyCount = 62, timeLimit = 165 },
        new() { waveNumber = 18, enemyLevel = 4, enemyCount = 67, timeLimit = 165 },
        new() { waveNumber = 19, enemyLevel = 4, enemyCount = 72, timeLimit = 165 },
        new() { waveNumber = 20, enemyLevel = 4, enemyCount = 77, timeLimit = 165 },
        new() { waveNumber = 21, enemyLevel = 5, enemyCount = 83, timeLimit = 225 },
        new() { waveNumber = 22, enemyLevel = 5, enemyCount = 89, timeLimit = 225 },
        new() { waveNumber = 23, enemyLevel = 5, enemyCount = 95, timeLimit = 225 },
        new() { waveNumber = 24, enemyLevel = 5, enemyCount = 101, timeLimit = 225 },
        new() { waveNumber = 25, enemyLevel = 5, enemyCount = 110, timeLimit = 225 }
    };



    // 기본 설치 빌딩
    public List<BuildingStats> defaultBuildingStatsList = new()
{
    new() {
        buildingName = "MainBuilding",
        levels = new []
        {
            new BuildingLevel { health = 2000, upgradeCost = 500 },
            new BuildingLevel { health = 2500, upgradeCost = 500 },
            new BuildingLevel { health = 3000, upgradeCost = 500 },
            new BuildingLevel { health = 3500, upgradeCost = 500 },
            new BuildingLevel { health = 5000, upgradeCost = 500 }
        }
    },
    new() {
        buildingName = "Building 1",    //외벽 타워
        levels = new []
        {
            new BuildingLevel { health = 1500, upgradeCost = 400 },
            new BuildingLevel { health = 2000, upgradeCost = 400 },
            new BuildingLevel { health = 2500, upgradeCost = 400 },
            new BuildingLevel { health = 3000, upgradeCost = 400 },
            new BuildingLevel { health = 4000, upgradeCost = 400 }
        }
    },
    new() {
        buildingName = "Building 2",    //성벽
        levels = new []
        {
            new BuildingLevel { health = 1000, upgradeCost = 400 },
            new BuildingLevel { health = 1200, upgradeCost = 400 },
            new BuildingLevel { health = 1400, upgradeCost = 400 },
            new BuildingLevel { health = 1600, upgradeCost = 400 },
            new BuildingLevel { health = 2000, upgradeCost = 400 }
        }
    }
};

    // 상점에서 구매 가능한 빌딩
    public List<BuildingStats> shopBuildingStatsList = new()
{
    new() {
        buildingName = "Building A", // 목조 타워
        levels = new []
        {
            new BuildingLevel { health = 650, upgradeCost = 300, purchaseCost = 110 },
            new BuildingLevel { health = 800, upgradeCost = 300, purchaseCost = 130 },
            new BuildingLevel { health = 950, upgradeCost = 300, purchaseCost = 150 },
            new BuildingLevel { health = 1100, upgradeCost = 300, purchaseCost = 170 },
            new BuildingLevel { health = 1400, upgradeCost = 300, purchaseCost = 210 }
        }
    },
    new() {
        buildingName = "Building B", // 바리케이트
        levels = new []
        {
            new BuildingLevel { health = 300, upgradeCost = 300, purchaseCost = 50 },
            new BuildingLevel { health = 400, upgradeCost = 300, purchaseCost = 65 },
            new BuildingLevel { health = 500, upgradeCost = 300, purchaseCost = 80 },
            new BuildingLevel { health = 600, upgradeCost = 300, purchaseCost = 95 },
            new BuildingLevel { health = 800, upgradeCost = 300, purchaseCost = 125 }
        }
    },
    new() {
        buildingName = "Building C", // 공격 도구
        levels = new []
        {
            new BuildingLevel { health = 200, upgradeCost = 200, purchaseCost = 100 },
            new BuildingLevel { health = 300, upgradeCost = 200, purchaseCost = 200 },
            new BuildingLevel { health = 400, upgradeCost = 200, purchaseCost = 400 },
            new BuildingLevel { health = 500, upgradeCost = 200, purchaseCost = 600 },
            new BuildingLevel { health = 700, upgradeCost = 200, purchaseCost = 1000 }
        }
    },
    new() {
        buildingName = "Building D", // 공격 도구
        levels = new []
        {
            new BuildingLevel { health = 200, upgradeCost = 200, purchaseCost = 100 },
            new BuildingLevel { health = 300, upgradeCost = 200, purchaseCost = 200 },
            new BuildingLevel { health = 400, upgradeCost = 200, purchaseCost = 400 },
            new BuildingLevel { health = 500, upgradeCost = 200, purchaseCost = 600 },
            new BuildingLevel { health = 700, upgradeCost = 200, purchaseCost = 1000 }
        }
    }
};




    // 🎯 플레이어 레벨별 기본 스탯 리스트 (최적화)
    public List<LevelStats> playerLevelStats = new()
    {
        new() { level = 1, health = 100, baseAttackPower = 0 },
        new() { level = 2, health = 125, baseAttackPower = 25 },
        new() { level = 3, health = 150, baseAttackPower = 50 },
        new() { level = 4, health = 175, baseAttackPower = 75 },
        new() { level = 5, health = 200, baseAttackPower = 100 }
    };

    // 🎯 적군(NPC) 레벨별 기본 스탯 리스트 (최적화)
    public List<LevelStats> enemyLevelStats = new()
    {
        new() { level = 1, health = 100, baseAttackPower = 0 },
        new() { level = 2, health = 103, baseAttackPower = 3 },
        new() { level = 3, health = 106, baseAttackPower = 6 },
        new() { level = 4, health = 109, baseAttackPower = 9 },
        new() { level = 5, health = 115, baseAttackPower = 15 }
    };

    // 🎯 아군(NPC) 레벨별 기본 스탯 리스트 (업그레이드 비용 포함)
    public List<LevelStats> allyLevelStats = new()
    {
        new() { level = 1, health = 100, baseAttackPower = 0, buyCost = 50 },
        new() { level = 2, health = 105, baseAttackPower = 5, buyCost = 100 },
        new() { level = 3, health = 110, baseAttackPower = 10, buyCost = 200 },
        new() { level = 4, health = 115, baseAttackPower = 15, buyCost = 400 },
        new() { level = 5, health = 130, baseAttackPower = 30, buyCost = 800 }
    };

    // 🎯 아군 공격 구조물(Tower_NPC) 레벨별 기본 스탯 리스트 (적용 버전)
    public List<LevelStats> towerCannonStats = new()
{
    new() { level = 1, health = 50, baseAttackPower = 0 },
    new() { level = 2, health = 55, baseAttackPower = 5 },
    new() { level = 3, health = 60, baseAttackPower = 10 },
    new() { level = 4, health = 65, baseAttackPower = 15 },
    new() { level = 5, health = 75, baseAttackPower = 25 }
};

    public List<LevelStats> towerCrossbowStats = new()
{
    new() { level = 1, health = 100, baseAttackPower = 0 },
    new() { level = 2, health = 115, baseAttackPower = 10 },
    new() { level = 3, health = 130, baseAttackPower = 20 },
    new() { level = 4, health = 145, baseAttackPower = 30 },
    new() { level = 5, health = 175, baseAttackPower = 50 }
};


    // 🎯 플레이어 무기 스탯 리스트 (최적화)
    public List<WeaponStats> weaponStatsList = new()
    {
        new() { weapon = PlayerStats.WeaponMode.Sword, damage = 90, range = 150f, cooldown = 0.5f },
        new() { weapon = PlayerStats.WeaponMode.Bow, damage = 70, range = 500f, cooldown = 2f }
    };

    // 🎯 플레이어 마법 스탯 리스트 (최적화)
    public List<MagicStats> magicStatsList = new()
    {
        new() { magic = PlayerStats.MagicMode.EmpoweredAttack, bonusDamage = 120, bonusRange = 0f, cooldown = 8f },
        new() { magic = PlayerStats.MagicMode.MeleeMagic, bonusDamage = 15, bonusRange = 300f, cooldown = 5f }
    };

    //NPC 직업별 스텟
    public List<NPCClassStats> npcClassStatsList = new()
    {
        new() { npcClass = NPCClass.Warrior, attackBonus = 80, range = 100, cooldown = 1f },
        new() { npcClass = NPCClass.Archer, attackBonus = 50, range = 400, cooldown = 3f }
    };

    //공격 구조물 종류별 스텟
    public List<T_NPCclassStats> TnpcClassStatsList = new()
    {
        new() { TnpcClass = T_NPCclass.Cannon, attackBonus = 90, range = 6000, cooldown = 10f },
        new() { TnpcClass = T_NPCclass.Crossbow, attackBonus = 100, range = 6500, cooldown = 12f }
    };

    // 💾 내부 Dictionary (빠른 검색을 위해 사용)
    private Dictionary<int, (int health, int baseAttackPower)> playerLevelStatsDict;
    private Dictionary<int, (int health, int baseAttackPower)> enemyLevelStatsDict;
    private Dictionary<int, (int health, int baseAttackPower)> allyLevelStatsDict;
    private Dictionary<T_NPCclass, Dictionary<int, (int health, int baseAttackPower)>> TnpcLevelStatsDict;
    private Dictionary<PlayerStats.WeaponMode, (int damage, float range, float cooldown)> weaponStatsDict;
    private Dictionary<PlayerStats.MagicMode, (int bonusDamage, float bonusRange, float cooldown)> magicStatsDict;
    private Dictionary<string, BuildingLevel[]> defaultBuildingStatsDict;
    private Dictionary<string, BuildingLevel[]> shopBuildingStatsDict;
    private Dictionary<NPCClass, (int attackBonus, int range, float cooldown)> npcClassStatsDict;
    private Dictionary<T_NPCclass, (int attackBonus, int range, float cooldown)> TnpcClassStatsDict;
    private Dictionary<int, WaveStats> waveStatsDict;

    public void Initialize()
    {
        // 🔹 웨이브 스탯 초기화
        waveStatsDict = new Dictionary<int, WaveStats>();
        foreach (var stats in waveStatsList)
        {
            waveStatsDict[stats.waveNumber] = stats;
        }

        // 🔹 플레이어 레벨별 스탯 초기화
        playerLevelStatsDict = new Dictionary<int, (int, int)>();
        foreach (var stats in playerLevelStats)
        {
            playerLevelStatsDict[stats.level] = (stats.health, stats.baseAttackPower);
        }

        // 🔹 적군(NPC) 레벨별 스탯 초기화
        enemyLevelStatsDict = new Dictionary<int, (int, int)>();
        foreach (var stats in enemyLevelStats)
        {
            enemyLevelStatsDict[stats.level] = (stats.health, stats.baseAttackPower);
        }

        // 🔹 적군(NPC) 레벨별 스탯 초기화
        allyLevelStatsDict = new Dictionary<int, (int, int)>();
        foreach (var stats in allyLevelStats)
        {
            allyLevelStatsDict[stats.level] = (stats.health, stats.baseAttackPower);
        }

        // 🔹 무기 스탯 초기화
        weaponStatsDict = new Dictionary<PlayerStats.WeaponMode, (int, float, float)>();
        foreach (var stats in weaponStatsList)
        {
            weaponStatsDict[stats.weapon] = (stats.damage, stats.range, stats.cooldown);
        }

        // 🔹 마법 스탯 초기화
        magicStatsDict = new Dictionary<PlayerStats.MagicMode, (int, float, float)>();
        foreach (var stats in magicStatsList)
        {
            magicStatsDict[stats.magic] = (stats.bonusDamage, stats.bonusRange, stats.cooldown);
        }

            // 🔹 빌딩 정보 초기화
        defaultBuildingStatsDict = new();
        foreach (var stats in defaultBuildingStatsList)
        {
            defaultBuildingStatsDict[stats.buildingName] = stats.levels;
        }
        shopBuildingStatsDict = new();
        foreach (var stats in shopBuildingStatsList)
        {
            shopBuildingStatsDict[stats.buildingName] = stats.levels;
        }

        // 🔹 NPC 직업별 스탯 초기화
        npcClassStatsDict = new Dictionary<NPCClass, (int, int, float)>();
        foreach (var stats in npcClassStatsList)
        {
            npcClassStatsDict[stats.npcClass] = (stats.attackBonus, stats.range, stats.cooldown);
        }

        // 🔹 공격 구조물 종류별 클래스 고정 스탯 초기화
        TnpcClassStatsDict = new Dictionary<T_NPCclass, (int, int, float)>();
        foreach (var stats in TnpcClassStatsList)
        {
            TnpcClassStatsDict[stats.TnpcClass] = (stats.attackBonus, stats.range, stats.cooldown);
        }

        // 🔹 공격 구조물 레벨별 스탯 초기화
        TnpcLevelStatsDict = new Dictionary<T_NPCclass, Dictionary<int, (int, int)>>();

        TnpcLevelStatsDict[T_NPCclass.Cannon] = new Dictionary<int, (int, int)>();
        foreach (var stats in towerCannonStats)
        {
            TnpcLevelStatsDict[T_NPCclass.Cannon][stats.level] = (stats.health, stats.baseAttackPower);
        }

        TnpcLevelStatsDict[T_NPCclass.Crossbow] = new Dictionary<int, (int, int)>();
        foreach (var stats in towerCrossbowStats)
        {
            TnpcLevelStatsDict[T_NPCclass.Crossbow][stats.level] = (stats.health, stats.baseAttackPower);
        }
    }

    // 웨이브 스텟 가져오기
    public WaveStats GetWaveStats(int waveNumber)
    {
        return waveStatsDict.TryGetValue(waveNumber, out var stats)
            ? stats
            : new WaveStats { waveNumber = waveNumber, enemyLevel = 1, enemyCount = 6, timeLimit = 45 }; // 기본값 포함
    }


    // 🎯 플레이어 레벨별 스탯 가져오기
    public (int health, int baseAttackPower) GetPlayerLevelStats(int level)
    {
        return playerLevelStatsDict.ContainsKey(level) ? playerLevelStatsDict[level] : (100, 0);
    }

    // 🎯 적군(NPC) 레벨별 스탯 가져오기
    public (int health, int baseAttackPower) GetEnemyLevelStats(int level)
    {
        return enemyLevelStatsDict.ContainsKey(level) ? enemyLevelStatsDict[level] : (100, 0);
    }

    // 🎯 아군(NPC) 레벨별 스탯 가져오기 (+ 업그레이드 비용 포함)
    public (int health, int baseAttackPower, int upgradeCost) GetAllyLevelStats(int level)
    {
        var stats = allyLevelStats.Find(s => s.level == level);
        return stats != null ? (stats.health, stats.baseAttackPower, stats.buyCost) : (100, 0, 50);
    }

    // 🎯 플레이어 무기 스탯 가져오기
    public (int damage, float range, float cooldown) GetWeaponStats(PlayerStats.WeaponMode weapon)
    {
        return weaponStatsDict.ContainsKey(weapon) ? weaponStatsDict[weapon] : (0, 100f, 1f);
    }

    // 🎯 플레이어 마법 스탯 가져오기
    public (int bonusDamage, float bonusRange, float cooldown) GetMagicStats(PlayerStats.MagicMode magic)
    {
        return magicStatsDict.ContainsKey(magic) ? magicStatsDict[magic] : (0, 300f, 1f);
    }

    // 🎯 NPC 직업별 스탯 가져오기
    public (int attackBonus, int range, float cooldown) GetNPCClassStats(NPCClass npcClass)
    {
        return npcClassStatsDict.ContainsKey(npcClass) ? npcClassStatsDict[npcClass] : (0, 100, 1f);
    }

    // 🎯 공격 구조물 종류별 스탯 가져오기
    public (int attackBonus, int range, float cooldown) GetTNPCclassStats(T_NPCclass TnpcClass)
    {
        return TnpcClassStatsDict.ContainsKey(TnpcClass) ? TnpcClassStatsDict[TnpcClass] : (0, 100, 1f);
    }

    public (int health, int baseAttackPower) GetTNPCLevelStats(T_NPCclass npcClass, int level)
    {
        if (TnpcLevelStatsDict.ContainsKey(npcClass) && TnpcLevelStatsDict[npcClass].ContainsKey(level))
            return TnpcLevelStatsDict[npcClass][level];
        return (0, 0); // 기본값
    }

    // 기본 설치된 빌딩 조회
    public (int health, int upgradeCost) GetDefaultBuildingStats(string buildingName, int level)
    {
        if (defaultBuildingStatsDict.TryGetValue(buildingName, out var levels) && level >= 1 && level <= levels.Length)
        {
            var stats = levels[level - 1];
            return (stats.health, stats.upgradeCost);
        }
        return (-1, -1);
    }

    // 상점에서 구매한 빌딩 조회
    public (int health, int upgradeCost, int purchaseCost) GetShopBuildingStats(string buildingName, int level)
    {
        if (shopBuildingStatsDict.TryGetValue(buildingName, out var levels) && level >= 1 && level <= levels.Length)
        {
            var stats = levels[level - 1];
            return (stats.health, stats.upgradeCost, stats.purchaseCost);
        }
        return (-1, -1, -1);
    }


}
