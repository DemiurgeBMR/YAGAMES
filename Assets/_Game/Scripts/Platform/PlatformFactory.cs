using BaggageRush.Platform.Mock;
using UnityEngine;

namespace BaggageRush.Platform
{
    /// <summary>
    /// Выбирает реализацию площадки по define-символу сборки
    /// (YANDEX_GAMES, позже RUSTORE и GOOGLE_PLAY; меню YAGAMES → Платформа).
    /// Пока SDK не подключены, везде работают заглушки — игру можно собрать и запустить на любой площадке.
    /// </summary>
    public static class PlatformFactory
    {
        public const string SaveKey = "baggage_rush_save";

        public static PlatformServices Create(MonoBehaviour host)
        {
            return new PlatformServices
            {
                Platform = new MockPlatformService(),
                Ads = new MockAdsService(host),
                Purchases = new MockPurchaseService(),
                Leaderboards = new MockLeaderboardService(),
                Storage = new PlayerPrefsStorage(SaveKey),
            };
        }
    }
}
