using System;
using System.Collections.Generic;

namespace BaggageRush.Platform
{
    public enum DeviceKind { Desktop, Mobile, Tablet }

    /// <summary>Базовые функции площадки: язык, устройство, время и служебные сигналы.</summary>
    public interface IPlatformService
    {
        string Name { get; }
        /// <summary>Двухбуквенный код языка: "ru", "en", "tr"...</summary>
        string Language { get; }
        DeviceKind Device { get; }
        /// <summary>Серверное время, если площадка его даёт (защита офлайн-дохода от перевода часов).</summary>
        DateTime UtcNow { get; }

        void Initialize(Action onReady);
        /// <summary>Игра загрузилась и готова к игре (Яндекс: LoadingAPI.ready).</summary>
        void NotifyGameReady();
        /// <summary>Начался активный геймплей (Яндекс: GameplayAPI.start).</summary>
        void GameplayStart();
        /// <summary>Геймплей на паузе: меню, реклама, экран победы (Яндекс: GameplayAPI.stop).</summary>
        void GameplayStop();
    }

    public interface IAdsService
    {
        void ShowInterstitial(Action onClosed);
        /// <summary>onRewarded вызывается, только если игрок досмотрел; onClosed — всегда.</summary>
        void ShowRewarded(Action onRewarded, Action onClosed);
        void SetBannerVisible(bool visible);
    }

    public sealed class ProductInfo
    {
        public readonly string Id;
        public readonly string Title;
        /// <summary>Цена, уже отформатированная площадкой (с валютой).</summary>
        public readonly string Price;

        public ProductInfo(string id, string title, string price)
        {
            Id = id;
            Title = title;
            Price = price;
        }
    }

    public interface IPurchaseService
    {
        IReadOnlyList<ProductInfo> Products { get; }
        void Initialize(Action onReady);
        void Purchase(string productId, Action<bool> onComplete);
        /// <summary>
        /// Покупки, которые оплачены, но не выданы (игра закрылась во время оплаты).
        /// Для каждой вызывается grant, после чего покупка подтверждается на площадке.
        /// </summary>
        void ProcessPending(Action<string> grant);
    }

    public interface ILeaderboardService
    {
        void SubmitScore(string leaderboard, long score);
    }

    /// <summary>Хранилище строки сохранения: облако площадки или локально.</summary>
    public interface ISaveStorage
    {
        /// <summary>Передаёт null, если сохранения ещё нет.</summary>
        void Load(Action<string> onLoaded);
        void Save(string json);
    }

    public sealed class PlatformServices
    {
        public IPlatformService Platform;
        public IAdsService Ads;
        public IPurchaseService Purchases;
        public ILeaderboardService Leaderboards;
        public ISaveStorage Storage;
    }
}
