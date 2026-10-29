using System;
using System.Collections.Generic;

namespace BaggageRush.Core
{
    /// <summary>
    /// Всё, что сохраняется между сессиями. Сериализуется через JsonUtility,
    /// поэтому только поля и списки (словари JsonUtility не умеет).
    /// При изменении формата увеличивать CurrentVersion и дописывать миграцию в SaveSystem.
    /// </summary>
    [Serializable]
    public sealed class SaveData
    {
        public const int CurrentVersion = 1;

        public int version = CurrentVersion;

        public int coins;
        public int level = 1;
        public long passengers;
        public bool noAds;

        public long lastSeenUtcTicks;
        public long lastDailyClaimUtcTicks;
        public int dailyStreak;

        public bool soundOn = true;
        public bool musicOn = true;

        public List<IntEntry> upgrades = new List<IntEntry>();
        public List<IntEntry> boosters = new List<IntEntry>();

        public int GetUpgrade(string id) => Get(upgrades, id);
        public void SetUpgrade(string id, int value) => Set(upgrades, id, value);
        public int GetBooster(string id) => Get(boosters, id);
        public void AddBooster(string id, int amount) => Set(boosters, id, Get(boosters, id) + amount);

        static int Get(List<IntEntry> list, string key)
        {
            foreach (var e in list)
                if (e.key == key) return e.value;
            return 0;
        }

        static void Set(List<IntEntry> list, string key, int value)
        {
            foreach (var e in list)
            {
                if (e.key != key) continue;
                e.value = value;
                return;
            }
            list.Add(new IntEntry { key = key, value = value });
        }
    }

    [Serializable]
    public sealed class IntEntry
    {
        public string key;
        public int value;
    }
}
