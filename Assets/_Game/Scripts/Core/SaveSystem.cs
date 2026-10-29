using System;
using BaggageRush.Platform;
using UnityEngine;

namespace BaggageRush.Core
{
    /// <summary>
    /// Держит SaveData в памяти и пишет её в хранилище не чаще FlushInterval:
    /// облачные сохранения Яндекса ограничивают частоту записи.
    /// Код игры меняет Data и вызывает MarkDirty().
    /// </summary>
    public sealed class SaveSystem
    {
        const float FlushInterval = 3f;

        readonly ISaveStorage storage;
        bool dirty;
        float lastFlushTime = float.NegativeInfinity;

        public SaveData Data { get; private set; } = new SaveData();
        public event Action Changed;

        public SaveSystem(ISaveStorage storage)
        {
            this.storage = storage;
        }

        public void Load(Action onLoaded)
        {
            storage.Load(json =>
            {
                Data = Parse(json);
                Changed?.Invoke();
                onLoaded?.Invoke();
            });
        }

        public void MarkDirty()
        {
            dirty = true;
            Changed?.Invoke();
        }

        public void Tick()
        {
            if (dirty && Time.unscaledTime - lastFlushTime >= FlushInterval)
                Flush();
        }

        public void Flush()
        {
            dirty = false;
            lastFlushTime = Time.unscaledTime;
            storage.Save(JsonUtility.ToJson(Data));
        }

        public void ResetProgress()
        {
            Data = new SaveData();
            Flush();
            Changed?.Invoke();
        }

        static SaveData Parse(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new SaveData();
            try
            {
                var data = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
                data.version = SaveData.CurrentVersion;
                return data;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Сохранение повреждено, начинаем заново: {e.Message}");
                return new SaveData();
            }
        }
    }
}
