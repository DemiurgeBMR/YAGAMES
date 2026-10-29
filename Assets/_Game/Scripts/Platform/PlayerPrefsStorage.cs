using System;
using UnityEngine;

namespace BaggageRush.Platform
{
    /// <summary>Локальное сохранение. Используется в редакторе и как запасной вариант на любой площадке.</summary>
    public sealed class PlayerPrefsStorage : ISaveStorage
    {
        readonly string key;

        public PlayerPrefsStorage(string key)
        {
            this.key = key;
        }

        public void Load(Action<string> onLoaded)
        {
            onLoaded?.Invoke(PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null);
        }

        public void Save(string json)
        {
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
    }
}
