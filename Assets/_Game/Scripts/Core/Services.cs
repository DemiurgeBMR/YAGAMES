using BaggageRush.Monetization;
using BaggageRush.Platform;

namespace BaggageRush.Core
{
    /// <summary>Точка доступа к сервисам игры. Заполняется в GameBootstrap до загрузки игровых сцен.</summary>
    public static class Services
    {
        public static IPlatformService Platform { get; private set; }
        public static IPurchaseService Purchases { get; private set; }
        public static ILeaderboardService Leaderboards { get; private set; }
        public static AdsManager Ads { get; private set; }
        public static SaveSystem Save { get; private set; }
        public static bool IsReady { get; private set; }

        internal static void Init(PlatformServices services)
        {
            Platform = services.Platform;
            Purchases = services.Purchases;
            Leaderboards = services.Leaderboards;
            Save = new SaveSystem(services.Storage);
            Ads = new AdsManager(services.Ads, services.Platform, Save);
        }

        internal static void MarkReady() => IsReady = true;

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics() => IsReady = false;
    }
}
