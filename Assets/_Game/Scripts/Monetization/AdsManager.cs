using System;
using BaggageRush.Core;
using BaggageRush.Platform;
using UnityEngine;

namespace BaggageRush.Monetization
{
    /// <summary>
    /// Правила показа рекламы поверх SDK площадки: когда межстраничную можно показать,
    /// пауза игры на время рекламы, выдача награды только после закрытия ролика.
    /// </summary>
    public sealed class AdsManager
    {
        /// <summary>Новичков не встречаем рекламой: первые уровни без межстраничной.</summary>
        public const int FirstInterstitialLevel = 4;
        /// <summary>Минимум секунд между любой рекламой и следующей межстраничной.</summary>
        public const float InterstitialCooldown = 90f;

        readonly IAdsService ads;
        readonly IPlatformService platform;
        readonly SaveSystem save;
        float lastAdEndTime = float.NegativeInfinity;

        public bool IsShowing { get; private set; }

        public AdsManager(IAdsService ads, IPlatformService platform, SaveSystem save)
        {
            this.ads = ads;
            this.platform = platform;
            this.save = save;
        }

        public bool CanShowInterstitial =>
            !IsShowing
            && !save.Data.noAds
            && save.Data.level >= FirstInterstitialLevel
            && Time.realtimeSinceStartup - lastAdEndTime >= InterstitialCooldown;

        /// <summary>Показывает межстраничную, если правила разрешают. onDone вызывается в любом случае.</summary>
        public bool TryShowInterstitial(Action onDone)
        {
            if (!CanShowInterstitial)
            {
                onDone?.Invoke();
                return false;
            }
            Begin();
            ads.ShowInterstitial(() =>
            {
                End();
                onDone?.Invoke();
            });
            return true;
        }

        /// <summary>
        /// Реклама за награду. Работает и после покупки «Отключить рекламу» — игрок смотрит добровольно.
        /// Награда выдаётся после закрытия ролика, когда игра уже снята с паузы.
        /// </summary>
        public void ShowRewarded(Action onRewarded, Action onClosed = null)
        {
            if (IsShowing)
            {
                onClosed?.Invoke();
                return;
            }
            var rewarded = false;
            Begin();
            ads.ShowRewarded(() => rewarded = true, () =>
            {
                End();
                if (rewarded) onRewarded?.Invoke();
                onClosed?.Invoke();
            });
        }

        /// <summary>Липкий баннер только на ПК: на телефоне он отнимает место у игры.</summary>
        public void RefreshBanner()
        {
            ads.SetBannerVisible(platform.Device == DeviceKind.Desktop && !save.Data.noAds);
        }

        void Begin()
        {
            IsShowing = true;
            GamePause.Push(PauseReason.Ad);
            platform.GameplayStop();
        }

        void End()
        {
            IsShowing = false;
            lastAdEndTime = Time.realtimeSinceStartup;
            GamePause.Pop(PauseReason.Ad);
        }
    }
}
