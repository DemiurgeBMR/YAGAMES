using System;
using BaggageRush.Monetization;
using BaggageRush.Platform;
using UnityEngine;

namespace BaggageRush.Core
{
    /// <summary>
    /// Запуск игры: площадка → сохранение → покупки → сигнал «готово».
    /// Живёт всю сессию (DontDestroyOnLoad) и отвечает за сброс сохранений и паузу при потере фокуса.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public sealed class GameBootstrap : MonoBehaviour
    {
        static GameBootstrap instance;

        public static event Action Ready;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 60;

            Services.Init(PlatformFactory.Create(this));
            Services.Platform.Initialize(OnPlatformReady);
        }

        void OnPlatformReady()
        {
            Services.Save.Load(() =>
            {
                Services.Purchases.Initialize(() => Services.Purchases.ProcessPending(PurchaseRewards.Grant));
                Services.Ads.RefreshBanner();
                Services.MarkReady();
                Services.Platform.NotifyGameReady();
                Ready?.Invoke();
            });
        }

        void Update()
        {
            if (Services.IsReady)
                Services.Save.Tick();
        }

        void OnApplicationFocus(bool hasFocus)
        {
#if !UNITY_EDITOR
            // В редакторе фокус теряется при каждом клике вне Game View — там не мешаем.
            if (hasFocus) GamePause.Pop(PauseReason.Focus);
            else GamePause.Push(PauseReason.Focus);
#endif
            if (!hasFocus && Services.IsReady)
                Services.Save.Flush();
        }

        void OnApplicationQuit()
        {
            if (Services.IsReady)
                Services.Save.Flush();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            instance = null;
            Ready = null;
        }
    }
}
