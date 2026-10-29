using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaggageRush.Platform.Mock
{
    /// <summary>
    /// Заглушки площадки для редактора: реклама «показывается» секунду, покупки всегда успешны.
    /// Позволяют проверять всю логику монетизации без SDK.
    /// </summary>
    public sealed class MockPlatformService : IPlatformService
    {
        public string Name => "Mock";
        public string Language { get; } = DetectLanguage();
        public DeviceKind Device => Application.isMobilePlatform ? DeviceKind.Mobile : DeviceKind.Desktop;
        public DateTime UtcNow => DateTime.UtcNow;

        public void Initialize(Action onReady) => onReady?.Invoke();
        public void NotifyGameReady() => Debug.Log("[Mock] Игра готова (LoadingAPI.ready)");
        public void GameplayStart() => Debug.Log("[Mock] GameplayAPI.start");
        public void GameplayStop() => Debug.Log("[Mock] GameplayAPI.stop");

        static string DetectLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Russian:
                case SystemLanguage.Ukrainian:
                case SystemLanguage.Belarusian:
                    return "ru";
                case SystemLanguage.Turkish:
                    return "tr";
                default:
                    return "en";
            }
        }
    }

    public sealed class MockAdsService : IAdsService
    {
        const float FakeAdDuration = 1f;
        readonly MonoBehaviour host;

        public MockAdsService(MonoBehaviour host)
        {
            this.host = host;
        }

        public void ShowInterstitial(Action onClosed)
        {
            Debug.Log("[Mock] Межстраничная реклама");
            host.StartCoroutine(After(FakeAdDuration, onClosed));
        }

        public void ShowRewarded(Action onRewarded, Action onClosed)
        {
            Debug.Log("[Mock] Реклама за награду");
            host.StartCoroutine(After(FakeAdDuration, () =>
            {
                onRewarded?.Invoke();
                onClosed?.Invoke();
            }));
        }

        public void SetBannerVisible(bool visible) => Debug.Log($"[Mock] Баннер: {(visible ? "показан" : "скрыт")}");

        static IEnumerator After(float seconds, Action action)
        {
            // Realtime: во время рекламы Time.timeScale = 0.
            yield return new WaitForSecondsRealtime(seconds);
            action?.Invoke();
        }
    }

    public sealed class MockPurchaseService : IPurchaseService
    {
        readonly List<ProductInfo> products = new List<ProductInfo>
        {
            new ProductInfo(Monetization.ProductIds.NoAds, "Отключить рекламу", "199 ₽"),
            new ProductInfo(Monetization.ProductIds.StarterPack, "Стартовый набор", "99 ₽"),
            new ProductInfo(Monetization.ProductIds.CoinsSmall, "2 500 монет", "79 ₽"),
            new ProductInfo(Monetization.ProductIds.CoinsBig, "8 000 монет", "199 ₽"),
            new ProductInfo(Monetization.ProductIds.BoostersPack, "Набор бустеров", "149 ₽"),
        };

        public IReadOnlyList<ProductInfo> Products => products;

        public void Initialize(Action onReady) => onReady?.Invoke();

        public void Purchase(string productId, Action<bool> onComplete)
        {
            Debug.Log($"[Mock] Покупка {productId}");
            onComplete?.Invoke(true);
        }

        public void ProcessPending(Action<string> grant) { }
    }

    public sealed class MockLeaderboardService : ILeaderboardService
    {
        public void SubmitScore(string leaderboard, long score) => Debug.Log($"[Mock] Лидерборд {leaderboard}: {score}");
    }
}
