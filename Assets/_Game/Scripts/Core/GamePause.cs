using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaggageRush.Core
{
    public enum PauseReason { Ad, Focus, Menu }

    /// <summary>
    /// Единая пауза игры. Несколько причин могут действовать одновременно
    /// (реклама началась, пока вкладка свёрнута) — игра продолжится, когда снята последняя.
    /// Требование Яндекса: во время рекламы и при сворачивании вкладки звук выключен.
    /// </summary>
    public static class GamePause
    {
        static readonly HashSet<PauseReason> reasons = new HashSet<PauseReason>();

        public static bool IsPaused => reasons.Count > 0;
        public static event Action<bool> Changed;

        public static void Push(PauseReason reason)
        {
            if (reasons.Add(reason) && reasons.Count == 1)
                Apply(true);
        }

        public static void Pop(PauseReason reason)
        {
            if (reasons.Remove(reason) && reasons.Count == 0)
                Apply(false);
        }

        public static bool Has(PauseReason reason) => reasons.Contains(reason);

        // Статика переживает перезапуск Play Mode, если в редакторе отключена перезагрузка домена.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            reasons.Clear();
            Changed = null;
        }

        static void Apply(bool paused)
        {
            Time.timeScale = paused ? 0f : 1f;
            AudioListener.pause = paused;
            Changed?.Invoke(paused);
        }
    }
}
