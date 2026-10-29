namespace BaggageRush.Monetization
{
    /// <summary>Идентификаторы товаров. Должны совпадать с каталогом покупок в консоли площадки.</summary>
    public static class ProductIds
    {
        public const string NoAds = "no_ads";
        public const string StarterPack = "starter_pack";
        public const string CoinsSmall = "coins_small";
        public const string CoinsBig = "coins_big";
        public const string BoostersPack = "boosters_pack";
    }

    public static class BoosterIds
    {
        public const string Magnet = "magnet";
        public const string Freeze = "freeze";
        public const string ExtraSlot = "extra_slot";
        public const string Shuffle = "shuffle";

        public static readonly string[] All = { Magnet, Freeze, ExtraSlot, Shuffle };
    }
}
